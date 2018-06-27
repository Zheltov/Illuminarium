using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.MeshViewIndependent
{
    public class MScene
    {
        public static void BuildStaticMinimalMesh( MScene scene )
        {
            foreach ( var obj in scene.Objects )
            {
                for ( int i = 0; i < obj.Faces.Count; i++ )
                {
                    var face = obj.Faces[i];

                    while ( face.Divide() != null )
                    { }
                }
            }
        }

        public IList<MObj> Objects { get; set; }
        public Scene Scene { get; set; }
        public float MaxIlluminanceDirect { get; set; }
        public float MaxIlluminanceIndirect { get; set; }

        public MeshSettings DefaultMeshSettings { get; set; }

        public MScene( Scene scene, MeshSettings defaultMeshSettings )
        {
            Scene = scene;

            Objects = new List<MObj>();
            foreach ( var obj in scene.Objects )
                Objects.Add( new MObj( obj, defaultMeshSettings ) );
        }

        public void UpdateStatistics()
        {
            MaxIlluminanceDirect = 0;
            foreach ( var obj in Objects )
                foreach ( var vertex in obj.Vertices )
                {
                    vertex.UpdateStatistics();

                    var tmpDirect = vertex.IlluminanceDirect.Select( x => x.ValueMax ).Max();
                    if ( tmpDirect > MaxIlluminanceDirect )
                        MaxIlluminanceDirect = tmpDirect;

                    var tmpIndirect = vertex.IlluminanceIndirect.Select( x => x.ValueMax ).Max();
                    if ( tmpIndirect > MaxIlluminanceIndirect )
                        MaxIlluminanceIndirect = tmpIndirect;

                    //foreach ( var illuminance in vertex.IlluminanceDirect )
                    //    if ( illuminance.ValueMax > MaxIlluminanceDirect )
                    //        MaxIlluminanceDirect = illuminance.ValueMax;
                }
        }
    }
}