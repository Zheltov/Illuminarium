using Illuminarium.Core.Debug;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.MeshViewIndependent.Renderer
{
    public class MRenderDoubleLocalEst
    {
        public MScene Scene { get; private set; }
        public ILog Log { get; set; }
        public IRayTracer RayTracer { get; private set; }
        public int NRays { get; private set; }
        public float WMin { get; private set; }
        public MRenderDoubleLocalEst( MScene scene, IRayTracer rayTracer, int nRays, float wMin, ILog log = null )
        {
            NRays = nRays;
            WMin = wMin;
            Scene = scene;
            RayTracer = rayTracer;
            Log = log;
        }

        public void Calculate()
        {
            
            // главный цикл по лучам
            for ( var iteration = 0; iteration < NRays; iteration++ )
            {
                // определяем источник
                var light = this.RouletteLight();

                // розыгрыш луча от источника
                var ray = light.RandomRay();

                //ray = new LightRay( light.Position, new Vector( -0f, -0f, -1.0f, true ), new Spectrum( 1f ) );

                var wMinFact = ray.Illuminance.ValueMax * WMin;

                // основной блуждания луча
                int rayIteration = 0;
                while ( ray.Illuminance.ValueMax > wMinFact && rayIteration < 8 )
                {
                    rayIteration++;

                    // ищем пересечение с элементом сцены
                    var intersection = RayTracer.Trace( ray.From, ray.Direction, Constants.Epsilon );
                    while ( intersection != null && intersection.Face.Material.Reflectance == null )
                    {
                        if ( intersection.Face.Material.Mirror != null )
                        {
                            var mirrorRay = Vector.Reflect( ray.Direction, intersection.Face.Normal );
                            mirrorRay.Normalize();
                            ray.From = intersection.Point;
                            ray.Direction = mirrorRay;
                            intersection = this.RayTracer.Trace( ray.From, ray.Direction, Constants.Epsilon );
                        }
                        else
                            throw new NotImplementedException();
                    }
                    if ( intersection == null )
                        break;

                    //RayDebugStaticCollection.Add( new Ray( ray.From, intersection.Point ), Color.Blue );
                    //this.Log.Message( string.Format( "Ray.From = {0}, Ray.To = {1}", ray.From, intersection.Point ) );

                    var intersectionNormal = intersection.Face.Normal;
                    var intesectionPointOcclude = intersection.Point + intersectionNormal * Constants.Epsilon;

                    // для всех расчетных точек производимы вычисления
                    foreach ( var obj in Scene.Objects )
                        foreach ( var vertex in obj.Vertices )
                        {
                            var renderPointNormal = vertex.Face.Normal;

                            if ( vertex.Face == intersection.Face )
                                continue;

                            var dnormals = intersection.Face.Normal - renderPointNormal;
                            if ( Math.Abs( dnormals.X ) < Constants.Epsilon && Math.Abs( dnormals.Y ) < Constants.Epsilon && Math.Abs( dnormals.Z ) < Constants.Epsilon )
                                continue;


                            var mr = ( new Vector( intersection.Point, vertex.Point ).Reflect( vertex.Face.Normal ) );
                            //RayDebugStaticCollection.Add( new Ray( intersection.Point, vertex.Point ), Color.Green );
                            //RayDebugStaticCollection.Add( new Ray( vertex.Point,mr  ), Color.Red );

                            //if (point.Obj.Name == "Box2")
                            //{
                            //    point.Illuminance = new Spectrum(1f, 0, 0);
                            //    continue;
                            //}

                            //if ( Vector.Dot( intersectionNormal, renderPointNormal ) > Constants.Epsilon )
                            //{
                            //    vertex.CounterVisibleNormals++;
                            //    continue;
                            //}


                            var pointOcclude = vertex.Point;

                            // направление от точки до точки рендеринга
                            var r2 = Point3D.LenghtSquared( intesectionPointOcclude, pointOcclude );

                            var coreDirection = new Vector( intesectionPointOcclude, pointOcclude, true );
                            //var occludeRayLength = (float)Math.Sqrt( r2 ) - Constants.Epsilon;

                            //r2 = Point3D.LenghtSquared(intesectionPointOcclude, point.Position );
                            var occludeRayLength = (float)Math.Sqrt( r2 ) - Constants.Epsilon;


                            if ( this.RayTracer.Occluded( intesectionPointOcclude, coreDirection, 0, occludeRayLength ) )
                            {
                                //vertex.CounterOccluded++;
                                continue;
                            }



                            //if ( this.RayTracer.Occluded( intersection.Point, coreDirection, Constants.Epsilon, occludeRayLength ) )
                            //    continue;

                            var cos1 = ( Vector.Dot( coreDirection, intersectionNormal ) );
                            var cos2 = Math.Abs( Vector.Dot( coreDirection, renderPointNormal ) );

                            /*
                            if ( cos1 < Constants.Epsilon2 )
                                vertex.CounterRaysCos1Zero++;
                            if ( cos2 < Constants.Epsilon )
                                vertex.CounterRaysCos2Zero++;
                                */

                            cos1 = Math.Abs( cos1 );
                            cos2 = Math.Abs( cos2 );

                            var sigma1 = intersection.Face.Material.Reflectance.BRDF( ray.Direction, intersectionNormal, coreDirection );

                            if ( sigma1.ValueMax > 0 )
                                cos1 = Math.Abs( cos1 );

                            var kernel1 = sigma1 * cos1 * cos2 / r2;
                            //var kernel1 = intersection.Face.Material.BRDF( ray.Direction, intersection.Face.Normal,
                            //    core1Direction );

                            //var kernel1 = new Spectrum( sigma1 );

                            //kernel1.Multiplication( cos1 * cos2 / r2 );

                            // Ядро 2. Переход из суб. точки в искомое направление
                            for ( int i = 0; i < vertex.IlluminanceAngles.Directions.Count; i++ )
                            {
                                var sigma2 = vertex.Face.Material.Reflectance.BRDF( coreDirection, renderPointNormal, vertex.IlluminanceAngles.Directions[i] );

                                var kernel2 = sigma2;
                                var q = ray.Illuminance * kernel1 * kernel2;
                                if ( float.IsNaN( q.R ) || float.IsNaN( q.G ) || float.IsNaN( q.B ) )
                                    q = new Spectrum( );

                                vertex.IlluminanceIndirect[i] += q;
                            }

                        }

                    var newDirection = intersection.Face.Material.Reflectance.RandomReflectedDirection( ray.Direction,
                            intersectionNormal );
                    var brdf = intersection.Face.Material.Reflectance.BRDF( ray.Direction, intersectionNormal, newDirection );

                    ray = new LightRay( intersection.Point, newDirection, ray.Illuminance * brdf );

                    /*
                    if (rayIteration == 1)
                        ray = new LightRay(ray.From, new Vector(1f, 0, 1f), ray.Illuminance);
                    else if ( rayIteration == 2 )
                        ray = new LightRay( ray.From, new Vector( -0.85f, 1.5f, 1.5f, true ), ray.Illuminance );
                    
                     * */
                }

                if ( iteration % 10 == 0 )
                    Log.Message( string.Format( "Render {0} rays", iteration ), 1 );

            }


            const float XXXX = 0.25f;
            const float norm = 4 * Constants.PI * XXXX;
            foreach ( var obj in Scene.Objects )
                foreach ( var vertex in obj.Vertices )
                    for ( int i = 0; i < vertex.IlluminanceAngles.Directions.Count; i++ )
                    {
                        vertex.IlluminanceIndirect[i] *= ( norm / ( NRays * Constants.PI * Constants.PI ) );
                    }
        }

        ILight RouletteLight()
        {
            // TODO: Сделать нормальный розыгрыш источника
            if ( Scene.Scene.Lights.Count > 1 )
                throw new NotImplementedException();

            return Scene.Scene.Lights[0];
        }
    }
}
