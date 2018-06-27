using Illuminarium.Core;
using Illuminarium.Core.Lights;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Illuminarium.IO
{
    public class SceneLoader3ds : ISceneLoader
    {

        float scale;
        Scene baseScene = new Scene();

        public IMaterial DefaultMaterial { get; set; }

        public SceneLoader3ds( float scale, IMaterial defaultMaterial )
        {
            this.scale = scale;
            this.DefaultMaterial = defaultMaterial;
        }

        public Scene LoadFromFile( string fileName )
        {
            baseScene = new Scene();

            FileStream fs = new FileStream( fileName, FileMode.Open );
            BinaryReader br = new BinaryReader( fs, Encoding.ASCII );

            short Chank_ID;
            int Chank_Len;
            string name = "";

            try
            {
                Chank_ID = br.ReadInt16();
                Chank_Len = br.ReadInt32();
                while ( br.BaseStream.Position + 10 < br.BaseStream.Length && br.PeekChar() != -1 /*& Chank_ID >= 0*/ )
                {
                    switch ( Chank_ID )
                    {
                        case 0x4D4D:    // Сцена
                            break;
                        case 0x3D3D:    // Начало всех объектов
                            break;
                        case 0x4000:    // Некий объект (Объект, источник света), читаем его имя
                            byte b = br.ReadByte();
                            name = string.Empty;
                            while ( b != 0 )
                            {
                                name += (char)b;
                                b = br.ReadByte();
                            }
                            break;
                        case 0x4600:    // Источник света
                            readLight( name, br );
                            break;
                        case 0x4100:    // Объект
                            readObject( name, br );
                            break;
                        default:        // Не известные чанки
                            fs.Seek( Chank_Len - 6, SeekOrigin.Current );
                            break;
                    }
                    Chank_ID = br.ReadInt16();
                    Chank_Len = br.ReadInt32();
                }
            }
            catch
            {
                //throw e;
            }
            finally
            {
                br.Close();
                fs.Close();
            }

            return baseScene;
        }
        private void waitChunk( short expectChank_ID, BinaryReader br )
        {
            short Chank_ID = br.ReadInt16();
            int Chank_Len = br.ReadInt32();
            while ( Chank_ID != expectChank_ID )
            {
                br.BaseStream.Seek( Chank_Len - 6, SeekOrigin.Current );
                Chank_ID = br.ReadInt16();
                Chank_Len = br.ReadInt32();
            }
        }
        private void readObject( string name, BinaryReader br )
        {
            // Ожидаем соответствующий чанк
            waitChunk( 0x4110, br );

            Obj o = new Obj( name );
            baseScene.Objects.Add( o );

            // Читаем все вершины из файла
            short n = br.ReadInt16();
            Point3D[] points = new Point3D[n];
            for ( int i = 0; i < n; i++ )
            {
                var p = new Point3D(
                    br.ReadSingle() * scale,
                    br.ReadSingle() * scale,
                    br.ReadSingle() * scale);

                points[i] = p;
                o.Vertices.Add( p );
            }
            

            waitChunk( 0x4120, br );

            // 
            n = br.ReadInt16();
            short[,] order = new short[n, 3];
            for ( int i = 0; i < n; i++ )
            {
                Point3D[] p = new Point3D[3];

                //Face f = new Face();
                int[] vertexIndexes = new int[3];
                for ( int j = 0; j < 3; j++ )
                {
                    int index = br.ReadInt16();
                    p[j] = points[index];
                    vertexIndexes[j] = index;
                    //f.Vertexes.Add(p);
                }
                Face f = new Face( o, vertexIndexes );
                f.Material = this.DefaultMaterial;
                //f.VertexIndexes = vertexIndexes;
                o.Faces.Add( f );
                br.ReadInt16();
            }
        }
        private void readLight( string name, BinaryReader br )
        {
            PointLight l = new PointLight( );

            Point3D p = new Point3D(
                br.ReadSingle() * scale,
                br.ReadSingle() * scale,
                br.ReadSingle() * scale);

            l.Position = p;

            baseScene.Lights.Add( l );
        }

    }
}