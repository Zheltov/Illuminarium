using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Illuminarium.Core.Debug;

namespace Illuminarium.Core.GlobalIllumination.Core
{
    public class DoubleLocalEstimationCalculator
    {
        public ILog Log { get; set; }
        public Scene Scene { get; set; }
        public IRayTracer RayTracer { get; set; }
        public int NRays { get; private set; }
        public float WMin { get; private set; }

        public DoubleLocalEstimationCalculator( Scene scene, IRayTracer rayTracer, int nRays, float wMin )
        {
            Scene = scene;
            RayTracer = rayTracer;
            NRays = nRays;
            WMin = wMin;
        }

        public DoubleLocalEstimationCalculator( Scene scene, IRayTracer rayTracer, int nRays, float wMin, ILog log )
            : this( scene, rayTracer, nRays, wMin )
        {
            this.Log = log;
        }

        public void Calculate( IList<CalculationPointDLE> points )
        {
            // главный цикл по лучам
            for ( var iteration = 0; iteration < NRays; iteration++ )
            {
                // определяем источник
                var light = this.RouletteLight();

                // розыгрыш луча от источника
                var ray = light.RandomRay();


                // DEBUG
                // [0,000 ms] - Ray.From = x = 0,06968741, y = 0,1730685, z = 4,9, Ray.To = x = -2,499, y = 0,1456028, z = 2,175853
                // [0,000 ms] - Ray.From = x = 0,7848736, y = -2,312128, z = 0,0009999275, Ray.To = x = 0,785834, y = -0,3403686, z = 1,224729
                //ray = new LightRay(ray.From, new Vector( -0.1f, -0.25f, -1, true ), ray.Illuminance );
                //ray.To = new Point3D()


                var wMinFact = ray.Illuminance.ValueMax * WMin;

                // основной блуждания луча
                int rayIteration = 0;
                while ( ray.Illuminance.ValueMax > wMinFact && rayIteration < 8 )
                {
                    rayIteration++;

                    // ищем пересечение с элементом сцены
                    var intersection = this.RayTracer.Trace( ray.From, ray.Direction, Constants.Epsilon );
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
                    foreach ( var point in points )
                    {
                        var renderPointNormal = point.Face.Normal;


                        //if (point.Obj.Name == "Box2")
                        //{
                        //    point.Illuminance = new Spectrum(1f, 0, 0);
                        //    continue;
                        //}

                        if ( Vector.Dot( intersectionNormal, renderPointNormal ) > Constants.Epsilon )
                        {
                            point.CounterVisibleNormals++;
                            continue;
                        }


                        var pointOcclude = point.Position;

                        // направление от точки до точки рендеринга
                        var r2 = Point3D.LenghtSquared( intesectionPointOcclude, pointOcclude );

                        var coreDirection = new Vector( intesectionPointOcclude, pointOcclude, true );
                        //var occludeRayLength = (float)Math.Sqrt( r2 ) - Constants.Epsilon;

                        //r2 = Point3D.LenghtSquared(intesectionPointOcclude, point.Position );
                        var occludeRayLength = (float)Math.Sqrt( r2 ) - Constants.Epsilon;


                        if ( this.RayTracer.Occluded( intesectionPointOcclude, coreDirection, 0, occludeRayLength ) )
                        {
                            point.CounterOccluded++;
                            continue;
                        }



                        //if ( this.RayTracer.Occluded( intersection.Point, coreDirection, Constants.Epsilon, occludeRayLength ) )
                        //    continue;

                        var cos1 = ( Vector.Dot( coreDirection, intersectionNormal ) );
                        var cos2 = Math.Abs( Vector.Dot( coreDirection, renderPointNormal ) );

                        if ( cos1 < Constants.Epsilon2 )
                            point.CounterRaysCos1Zero++;
                        if ( cos2 < Constants.Epsilon )
                            point.CounterRaysCos2Zero++;

                        cos1 = Math.Abs( cos1 );
                        cos2 = Math.Abs( cos2 );

                        var sigma1 = intersection.Face.Material.Reflectance.BRDF( ray.Direction, intersectionNormal,
                            coreDirection );

                        var kernel1 = sigma1 * cos1 * cos2 / r2;
                        //var kernel1 = intersection.Face.Material.BRDF( ray.Direction, intersection.Face.Normal,
                        //    core1Direction );

                        //var kernel1 = new Spectrum( sigma1 );

                        //kernel1.Multiplication( cos1 * cos2 / r2 );

                        // Ядро 2. Переход из суб. точки в искомое направление
                        var sigma2 = point.Face.Material.Reflectance.BRDF( coreDirection, renderPointNormal,
                            point.Direction );

                        var kernel2 = sigma2;

                        point.Illuminance += ray.Illuminance * kernel1 * kernel2;

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

                if ( iteration % 100 == 0 )
                    this.Log.Message( string.Format( "Render {0} rays", iteration ), 1 );

            }


            const float norm = 4 * Constants.PI;
            foreach ( var point in points )
            {
                point.Illuminance *= ( norm / ( NRays * Constants.PI * Constants.PI ) );
            }
        }

        ILight RouletteLight()
        {
            // TODO: Сделать нормальный розыгрыш источника
            if ( this.Scene.Lights.Count > 1 )
                throw new NotImplementedException();

            return this.Scene.Lights[0];
        }
    }
}