using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.MeshViewIndependent
{
    public class MObj
    {
        public IObj Obj { get; set; }
        public IList<MFace> Faces { get; set; }
        public IList<MVertex> Vertices { get; set; }
        public MeshSettings MeshSettings { get; set; }

        public MObj( IObj obj, MeshSettings defaultMeshSettings )
        {
            Obj = obj;
            Faces = new List<MFace>();
            Vertices = new List<MVertex>();


            foreach ( var face in Obj.Faces.Where( x => x.Material.Reflectance != null ) )
            {
                var illuminanceAngles = new MFaceIlluminanceAngles( face.Normal, face.Material, defaultMeshSettings.SpectrumAngles.Theta, defaultMeshSettings.SpectrumAngles.Mu, defaultMeshSettings.SpectrumAngles.Phi, defaultMeshSettings.SpectrumAngles.GaussWeight, defaultMeshSettings.NSH );
                var vertices = new MVertex[3];
                for ( int i = 0; i < 3; i++ )
                {
                    var vertex = new MVertex( face, Obj.Vertices[face.VertexIndexes[i]], illuminanceAngles );
                    Vertices.Add( vertex );
                    vertices[i] = vertex;
                }

                Faces.Add( new MFace( face, this, vertices, defaultMeshSettings, illuminanceAngles ) );
            }
        }
    }
}