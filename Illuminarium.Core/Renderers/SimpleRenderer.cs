using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Illuminarium.Core.Debug;

namespace Illuminarium.Core.Renderers
{
    public class SimpleRenderer : IRenderer
    {
        public Scene Scene { get { return RayTracer.Scene; } }
        public ILog Log { get; set; }
        public IGlobalIllumination GlobalIllumination { get; set; }
        public IRayTracer RayTracer { get; private set; }

        public SimpleRenderer( IRayTracer rayTracer )
        {
            this.RayTracer = rayTracer;
        }

        public void RenderDirectIllumination( RenderPointsStructure renderPointsStructure )
        {
            int i = 0;
            foreach ( RenderPoint renderPoint in renderPointsStructure.RenderPoints )
            {
                Spectrum e = new Spectrum();
                if ( renderPoint.Obj is ILight )
                {
                    e = ( (ILight)renderPoint.Obj ).GetIlluminanceSourceSurface( renderPoint.Position, renderPoint.Direction ); // Specturm.Zero; //new Specturm( 0.25f );
                }
                else if ( renderPoint.Face.Material.Reflectance != null )
                {
                    foreach ( ILight light in renderPointsStructure.Scene.Lights )
                    {
                        //if ( renderPoint.MirrorRenderPoint != null )
                        //    renderPoint.IsPrimaryPoint = true;

                        var toLightVector = new Vector( renderPoint.Position, light.Position, true );
                        var normal = renderPoint.Face.Normal;
                        e += light.GetIlluminance( this.RayTracer, renderPoint.Face.Material, renderPoint.Position, normal, renderPoint.Direction );
                    }
                }

                //Specturm sp = new Specturm( e );
                renderPoint.IlluminanceDirect = e;

                i++;
                if ( i % 50000 == 0 )
                    this.Log.Message( string.Format( "Render {0} points of {1}", i, renderPointsStructure.RenderPoints.Count ), 1 );
            }
        }
        public RenderPointsStructure RenderDirectIllumination( ICamera camera, int width, int height )
        {
            var result = this.GenerateRenderPoints( camera, width, height );
            this.RenderDirectIllumination( result );
            return result;
        }
        public void RenderGlobalIllumination( RenderPointsStructure renderPointsStructure )
        {
            this.GlobalIllumination.Calculate( renderPointsStructure );
        }
        public RenderPointsStructure RenderGlobalIllumination( ICamera camera, int width, int height )
        {
            var result = this.GenerateRenderPoints( camera, width, height );
            this.RenderGlobalIllumination( result );
            return result;
        }

        public RenderPointsStructure GenerateRenderPoints( ICamera camera, int width, int height )
        {
            var tick = Environment.TickCount;
            var result = new RenderPointsStructure( this.Scene, width, height );
            this.Log.Message( string.Format( "Start generationg render points. Estimated count {0}", width * height ) );

            for ( int x = 0; x < width; x++ )
            {
                for ( int y = 0; y < height; y++ )
                {
                    Ray ray = camera.GetTracingRay( x, y, width, height );

                    var intersection = this.RayTracer.Trace( ray.From, ray.Direction );
                    if ( intersection == null )
                        continue;

                    // выбраковка не лицевых граней
                    var cosa = Vector.Dot( ray.Direction, intersection.Face.Normal );
                    if ( cosa > 0 )
                        continue;

                    var renderPoint = new RenderPoint( x, y, intersection, ray.Direction * -1f, true );
                    result.Add( renderPoint );

                    // материал зеркальный
                    var currentRenderPoint = renderPoint;
                    int iteration = 0;
                    while ( intersection.Face.Material.Mirror != null )
                    {
                        var reflectedRayDirection = ray.Direction.Reflect( intersection.Face.Normal );
                        ray = new Ray( intersection.Point, reflectedRayDirection );

                        //if ( rand.Next(100) == 50 )
                        //    RayDebugStaticCollection.Add( new Ray( intersection.Point, reflectedRayDirection ), Color.Red );

                        intersection = this.RayTracer.Trace( ray.From, ray.Direction, Constants.Epsilon );
                        if ( intersection == null )
                            break;

                        var mirrorRenderPoint = new RenderPoint( x, y, intersection, ray.Direction * -1f, false );
                        result.Add( mirrorRenderPoint );
                        currentRenderPoint.MirrorRenderPoint = mirrorRenderPoint;
                        currentRenderPoint = mirrorRenderPoint;
                        iteration++;
                        if ( iteration > 5 )
                            break;
                    }
                }
            }

            if ( this.Log != null )
            {
                tick = Environment.TickCount - tick;
                Log.Message( string.Format( "GetRenderPoints: Rays = {0}, Time = {1} s", result.RenderPoints.Count, (float)tick / 1000 ) );
            }

            return result;
        }

        public void FinalGathering( RenderPointsStructure renderPointsStructure )
        {
            // Final gathering
            this.Log.Message( "Start final gathering" );

            this.Log.Message( "Start final gathering mirror", 1 );
            foreach ( var renderPoint in renderPointsStructure.RenderPoints.Where( x => x.IsPrimaryPoint && x.MirrorRenderPoint != null ).ToList() )
            {
                var currentRenderPoint = renderPoint;
                //var mirrorPoint = renderPoint.MirrorRenderPoint;
                while ( currentRenderPoint.MirrorRenderPoint != null )
                {
                    if ( !( currentRenderPoint.MirrorRenderPoint.Obj is ILight ) )
                    {
                        renderPoint.IlluminanceDirect += currentRenderPoint.MirrorRenderPoint.IlluminanceDirect * currentRenderPoint.Face.Material.Mirror.Reflectance;
                        renderPoint.IlluminanceIndirect += currentRenderPoint.MirrorRenderPoint.IlluminanceIndirect * currentRenderPoint.Face.Material.Mirror.Reflectance;
                    }

                    currentRenderPoint = currentRenderPoint.MirrorRenderPoint;
                }
            }
            this.Log.Message( "End final gathering mirror", 1 );
            this.Log.Message( "End final gathering" );
        }
    }
}