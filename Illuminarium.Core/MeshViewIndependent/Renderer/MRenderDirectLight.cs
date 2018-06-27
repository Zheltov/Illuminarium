using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.MeshViewIndependent.Renderer
{
    public class MRenderDirectLight
    {
        public MScene Scene { get; private set; }
        public ILog Log { get; set; }
        public IRayTracer RayTracer { get; private set; }
        public MRenderDirectLight( MScene scene, IRayTracer rayTracer, ILog log = null )
        {
            Scene = scene;
            RayTracer = rayTracer;
            Log = log;
        }

        public void Calculate()
        {
            int iteration = 0;
            int totalPoints = 0;
            foreach ( var obj in Scene.Objects )
                totalPoints += obj.Vertices.Count;

            foreach ( var obj in Scene.Objects )
                foreach ( var vertex in obj.Vertices )
                {

                    //Spectrum e = new Spectrum();
                    if ( obj is ILight )
                    {
                        //e = ( (ILight)renderPoint.Obj ).GetIlluminanceSourceSurface( renderPoint.Position, renderPoint.Direction ); // Specturm.Zero; //new Specturm( 0.25f );
                    }


                    foreach ( var light in Scene.Scene.Lights )
                    {
                        //if ( renderPoint.MirrorRenderPoint != null )
                        //    renderPoint.IsPrimaryPoint = true;

                        //var toLightVector = new Vector( renderPoint.Position, light.Position, true );
                        //var normal = renderPoint.Face.Normal;
                        var e = light.GetIlluminance( RayTracer, vertex.Face.Material, vertex.Point, vertex.Face.Normal, vertex.IlluminanceAngles.Directions );

                        for ( int i = 0; i < vertex.IlluminanceAngles.Directions.Count; i++ )
                            vertex.IlluminanceDirect[i] += e[i];
                    }

                    //Specturm sp = new Specturm( e );
                    //renderPoint.IlluminanceDirect = e;

                    iteration++;
                    if ( iteration % 50 == 0 )
                        Log.Message( string.Format( "Render {0} points of {1}", iteration, totalPoints ), 1 );
                }

        }
    }
}
