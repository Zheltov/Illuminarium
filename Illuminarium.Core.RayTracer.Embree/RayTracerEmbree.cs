using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Illuminarium.Core;
using Embree;
using System.Runtime.InteropServices;

namespace Illuminarium.Core.RayTracer.Embree
{
    public class RayTracerEmbree : IRayTracer
    {
        IntPtr scenePtr;
        public Scene Scene { get; private set; }

        IDictionary<int, IObj> geometryIdLights = new Dictionary<int, IObj>();

        public RayTracerEmbree(Scene scene)
        {
            this.Scene = scene;

            this.Init(scene);
        }

        public unsafe Intersection Trace( Point3D from, Vector direction, float near = 0, float far = float.PositiveInfinity, float time = 0 )
        {
            var p = RTC.RayInterop.Packet1;

            p->orgX = from.X; p->orgY = from.Y; p->orgZ = from.Z;
            p->dirX = direction.X; p->dirY = direction.Y; p->dirZ = direction.Z;

            p->geomID = RTC.InvalidGeometryID;
            p->primID = RTC.InvalidGeometryID;
            p->instID = RTC.InvalidGeometryID;

            p->time = time;
            p->tnear = near;
            p->tfar = far;

            RTC.Intersect1( scenePtr, p );

            if ( p->geomID == RTC.InvalidGeometryID )
                return null;
            else
            {
                int geomId = (int)p->geomID;
                IObj obj = null;
                if ( geomId < this.Scene.Objects.Count )
                    obj = this.Scene.Objects[(int)p->geomID];
                else
                {
                    obj = geometryIdLights[geomId];
                }

                var face = obj.Faces[(int)p->primID];

                float distance = p->tfar;

                var dr = direction * distance;
                var r = from + dr;
                return new Intersection( obj, face, distance, r );
            }
        }

        public unsafe bool Occluded( Point3D from, Vector direction, float near = 0, float far = float.PositiveInfinity, float time = 0 )
        {
            var p = RTC.RayInterop.Packet1;

            p->orgX = from.X; p->orgY = from.Y; p->orgZ = from.Z;
            p->dirX = direction.X; p->dirY = direction.Y; p->dirZ = direction.Z;

            p->geomID = RTC.InvalidGeometryID;
            p->primID = RTC.InvalidGeometryID;
            p->instID = RTC.InvalidGeometryID;

            p->time = time;
            p->tnear = near;
            p->tfar = far;

            RTC.Occluded1( scenePtr, p );

            return p->geomID == 0;
        }

        public void Dispose()
        {
            RTC.DeleteScene( scenePtr );
            //RTC.Unregister();
        }

        int AddObj( IObj obj )
        {
            var indexes = new List<int>();
            foreach ( var face in obj.Faces )
                indexes.AddRange( face.VertexIndexes );

            var geomId = RTC.NewTriangleMesh( scenePtr, MeshFlags.Static, obj.Faces.Count, obj.Vertices.Count, 1 );
            RTC.CheckLastError();

            var indexBuffer = RTC.MapBuffer( scenePtr, geomId, RTC.BufferType.IndexBuffer );
            RTC.CheckLastError();
            Marshal.Copy( indexes.ToArray(), 0, indexBuffer, indexes.Count );
            RTC.UnmapBuffer( scenePtr, geomId, RTC.BufferType.IndexBuffer );

            var vertexBuffer = RTC.MapBuffer( scenePtr, geomId, RTC.BufferType.VertexBuffer );
            RTC.CheckLastError();

            unsafe
            {
                float* ptr = (float*)vertexBuffer;
                foreach ( var vertex in obj.Vertices )
                {
                    *( ptr++ ) = vertex.X;
                    *( ptr++ ) = vertex.Y;
                    *( ptr++ ) = vertex.Z;
                    *( ptr++ ) = 1.0f;
                }
            }

            RTC.UnmapBuffer( scenePtr, geomId, RTC.BufferType.VertexBuffer );

            //RTC.Commit( scenePtr );
            //RTC.EnableGeometry( scenePtr, geomId );
            RTC.CheckLastError();

            return (int)geomId;
        }

        void Init( Scene scene )
        {
            RTC.Register();
            scenePtr = RTC.NewScene( SceneFlags.Static | SceneFlags.Coherent | SceneFlags.Incoherent | SceneFlags.Robust,
                                     TraversalFlags.Single | TraversalFlags.Packet4 );

            foreach ( var obj in scene.Objects )
            {
                this.AddObj( obj );
            }

            foreach ( var light in scene.Lights )
                if ( light is IObj )
                {
                    var geomId = this.AddObj( (IObj)light );
                    this.geometryIdLights.Add( geomId, (IObj)light );
                }


            RTC.Commit( scenePtr );
            RTC.CheckLastError();
        }
    }
}
