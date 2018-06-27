using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminarium.Core.MeshViewIndependent
{
    public class MFace
    {
        public IFace Face { get; set; }
        public MObj Obj { get; set; }
        public MVertex[] Vertices { get; set; }
        public int Deep { get; set; }
        public MeshSettings MeshSettings { get; set; }
        public MFaceIlluminanceAngles IlluminanceAngles { get; set; }

        public MFace( MFace original, MVertex[] vertices )
        {
            MeshSettings = original.MeshSettings;
            Face = original.Face;
            Obj = original.Obj;
            Deep = original.Deep + 1;
            Vertices = vertices;
        }
        public MFace( IFace face, MObj obj, MVertex[] vertices, MeshSettings meshSettings, MFaceIlluminanceAngles illuminanceAngles )
        {
            MeshSettings = meshSettings;
            Face = face;
            Obj = obj;
            Deep = 0;            
            IlluminanceAngles = illuminanceAngles;
            Vertices = vertices;

            /*
            Vertices = new MVertex[3];
            for( int i = 0; i < 3; i++ )
                Vertices[i] = Obj.Vertices[face.VertexIndexes[i]];
            */

        }

        public MFaceDivideResult Divide()
        {
            if ( !CanDivide() )
                return null;

            var result = new MFaceDivideResult();

            // divide face
            // Ищем самую длинную сторону
            Point3D v12 = Vertices[0].Point - Vertices[1].Point;
            Point3D v23 = Vertices[1].Point - Vertices[2].Point;
            Point3D v31 = Vertices[2].Point - Vertices[0].Point;

            int[] k;
            if ( v12.Length2 > v23.Length2 && v12.Length2 > v31.Length2 )
                k = new int[] { 0, 1, 2 };
            else if ( v23.Length2 > v12.Length2 && v23.Length2 > v31.Length2 )
                k = new int[] { 1, 2, 0 };
            else
                k = new int[] { 2, 0, 1 };

            // Вычисляю координату новой вершины - центра гиппотренузы
            Point3D p = ( Vertices[k[0]].Point - Vertices[k[1]].Point ) / 2 + Vertices[k[1]].Point;

            // Новая вершина
            result.NewVertex = new MVertex( Vertices[0].Face, p, Vertices[0].IlluminanceAngles );
            
            // Создаем новую грань
            var newVertices = new MVertex[3];
            newVertices[k[1]] = Vertices[k[2]];
            newVertices[k[2]] = Vertices[k[0]];
            newVertices[k[0]] = result.NewVertex;

            result.NewFace = new MFace( this, newVertices );
            Obj.Faces.Add( result.NewFace );
            Obj.Vertices.Add( result.NewVertex );

            // Меняем вершину у сещуствующей
            Vertices[k[0]] = result.NewVertex;
            Deep++;
            
            return result;
        }

        bool CanDivide()
        {
            var result =
                Deep < MeshSettings.MaxDivideDeep; // &&
                //( Face.Material.Reflectance != null );

            return result;
        }
    }

    public class MFaceDivideResult
    {
        public MFace NewFace { get; set; }
        public MVertex NewVertex { get; set; }


    }
}