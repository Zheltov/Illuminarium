using System;
using System.Collections.Generic;
using System.Collections;

namespace Illuminarium.Core
{
    public class Scene
    {
        public Point3D PMax { get; set; }
        public Point3D PMin { get; set; }
        public Point3D PCenter { get; set; }
        public List<ILight> Lights { get; set; }
        public List<Obj> Objects { get; set; }
        public Camera Camera { get; set; }

        public Scene()
        {
            this.PMax = new Point3D( float.MinValue, float.MinValue, float.MinValue );
            this.PMin = new Point3D( float.MaxValue, float.MaxValue, float.MaxValue );
            this.Lights = new List<ILight>();
            this.Objects = new List<Obj>();
            this.Camera = new Camera();
        }

        /// <summary>
        /// Пересчет параметров сцены. max, min, center и т.п.
        /// Вызывать при перезагрузки сцены или значительных изменениях
        /// в сцене
        /// </summary>
        public void RecalcSceneParams()
        {
            var pmax = new float[] { float.MinValue, float.MinValue, float.MinValue };
            var pmin = new float[] { float.MaxValue, float.MaxValue, float.MaxValue };
            int N = 0;
            this.PCenter = new Point3D();
            foreach ( Obj o in this.Objects )
                foreach ( var f in o.Faces )
                    for ( int i = 0; i < 3; i++ )
                    {
                        Point3D p = f.Obj.Vertices[f.VertexIndexes[i]];
                        N += 1;
                        if ( p.X > pmax[0] )
                            pmax[0] = p.X;
                        if ( p.Y > pmax[1] )
                            pmax[1] = p.Y;
                        if ( p.Z > pmax[2] )
                            pmax[2] = p.Z;

                        if ( p.X < pmin[0] )
                            pmin[0] = p.X;
                        if ( p.Y < pmin[1] )
                            pmin[1] = p.Y;
                        if ( p.Z < pmin[2] )
                            pmin[2] = p.Z;

                        PCenter = PCenter + p;
                    }
            this.PCenter = new Point3D( PCenter.X / N, PCenter.Y / N, PCenter.Z / N );
            this.PMax = new Point3D( pmax[0], pmax[1], pmax[2] );
            this.PMin = new Point3D( pmin[0], pmin[1], pmin[2] );
        }

        public void CameraSetDefault()
        {
            this.Camera.Position = this.PMax * 2;
            this.Camera.Fov = (float)( Math.PI / 2 );
            this.Camera.Target = this.PCenter;
        }
    }
}