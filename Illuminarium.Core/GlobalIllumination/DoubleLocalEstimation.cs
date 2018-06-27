using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using Illuminarium.Core.Debug;
using Illuminarium.Core.GlobalIllumination.Core;

namespace Illuminarium.Core.GlobalIllumination
{
    public class DoubleLocalEstimation : IGlobalIllumination
    {
        private Scene scene;

        public ILog Log { get; set; }
        public IRayTracer RayTracer { get; set; }
        public int NPackets { get; private set; }
        public int NRays { get; private set; }
        public float WMin { get; private set; }

        public DoubleLocalEstimation( IRayTracer rayTracer, int nPackets, int nRays, float wMin )
        {
            RayTracer = rayTracer;
            scene = rayTracer.Scene;
            NPackets = nPackets;
            NRays = nRays;
            WMin = wMin;
        }

        public DoubleLocalEstimation( IRayTracer rayTracer, int nPackets, int nRays, float wMin, ILog log )
            : this( rayTracer, nPackets, nRays, wMin )
        {
            this.Log = log;
        }

        public void Calculate( RenderPointsStructure renderPointsStructure )
        {
            // Непосредственно калькулятор, который будет просчитывать точки
            var calculator = new DoubleLocalEstimationCalculator( this.RayTracer.Scene, this.RayTracer, NRays, WMin, this.Log );

            // Формируем ускоряющую структуру на основе сетки
            var giRenderPointsStructure = this.GenerateCalculatedStructure( renderPointsStructure );

            foreach ( var item in giRenderPointsStructure.RenderPointsFace )
            {
                RayDebugStaticCollection.Add( new Ray( item.Vertexes[0].Position, item.Vertexes[1].Position ), Color.WhiteSmoke );
                RayDebugStaticCollection.Add( new Ray( item.Vertexes[1].Position, item.Vertexes[2].Position ), Color.WhiteSmoke );
                RayDebugStaticCollection.Add( new Ray( item.Vertexes[2].Position, item.Vertexes[0].Position ), Color.WhiteSmoke );
            }

            //const float nPackets = 3;
            //const float wMin = 0.01f;

            // главный цикл по пакетам
            var calculationPoints = giRenderPointsStructure.RenderPointsVertexes.Select( x => (CalculationPointDLE)x ).ToList();

            for ( var packet = 0; packet < NPackets; packet++ )
            {
                giRenderPointsStructure.RenderPointsVertexes.ForEach( x => x.Illuminance = new Spectrum() );
                calculator.Calculate( calculationPoints );

                //calculator.Calculate(giRenderPointsStructure.RenderPointsVertexes);

                giRenderPointsStructure.RenderPointsVertexes.ForEach( x => x.IlluminancePacket.Add( x.Illuminance ) );
            }

            this.IlluminanceBugsCorrection( giRenderPointsStructure );

            foreach ( var giface in giRenderPointsStructure.RenderPointsFace )
            {
                var e = new Spectrum[3];
                e[0] = giface.Vertexes[0].Illuminance;
                e[1] = giface.Vertexes[1].Illuminance;
                e[2] = giface.Vertexes[2].Illuminance;

                foreach ( var point in giface.RenderPoints )
                {
                    var bc = giface.GetBarycentricCoordinates( point );
                    //var bc = giface.GetBarycentricCoordinates_Old( point );

                    var x = e[0] * ( (float)bc[1] ) + e[1] * ( (float)bc[2] ) + e[2] * ( (float)bc[0] );

                    if ( float.IsNaN( x.R ) )
                    {
                        throw new Exception();
                        //x = new Spectrum();
                    }

                    //point.IlluminanceIndirect = x;
                    point.IlluminanceIndirect = x;
                    //point.IlluminanceDirect = x;
                }
            }

        }

        void IlluminanceBugsCorrection( RenderPointsStructureDLE giRenderPointsStructure )
        {
            //var a = giRenderPointsStructure.RenderPointsVertexes.Where( x => x.Obj.Name == "Box2" && x.Position.Z < Constants.Epsilon ).ToList();

            this.IlluminanceBugsCorrectionEjections( giRenderPointsStructure );

            this.IlluminanceBugsCorrectionCos1Zero( giRenderPointsStructure );

            this.IlluminanceBugsCorrectionZeroSpectrum( giRenderPointsStructure );
        }

        void IlluminanceBugsCorrectionEjections( RenderPointsStructureDLE giRenderPointsStructure )
        {
            foreach ( var renderPoint in giRenderPointsStructure.RenderPointsVertexes )
            {
                var avg = renderPoint.IlluminancePacket.Average( x => x.Sum() );

                renderPoint.Illuminance = new Spectrum();
                int i = 0;
                foreach ( var packet in renderPoint.IlluminancePacket )
                {

                    if ( packet.Sum() > avg * 2 )
                    {
                        continue;
                    }
                    renderPoint.Illuminance.Add( packet );
                    i++;
                }
                renderPoint.Illuminance = renderPoint.Illuminance / i;

            }
        }

        void IlluminanceBugsCorrectionZeroSpectrum( RenderPointsStructureDLE giRenderPointsStructure )
        {
            foreach ( var giface in giRenderPointsStructure.RenderPointsFace )
            {
                float v1Max = giface.Vertexes[0].Illuminance.ValueMax;
                float v2Max = giface.Vertexes[1].Illuminance.ValueMax;
                float v3Max = giface.Vertexes[2].Illuminance.ValueMax;

                if ( ( v1Max < Constants.Epsilon || v2Max < Constants.Epsilon || v3Max < Constants.Epsilon ) && giface.Center.Illuminance.ValueMax > Constants.Epsilon )
                {
                    int i = 0;
                    List<TriangleVertexDLE> correctionVertexes = new List<TriangleVertexDLE>();
                    var normalVertexesIll = new Spectrum();
                    foreach ( var v in giface.Vertexes )
                        if ( v.Illuminance.ValueMax < Constants.Epsilon )
                            correctionVertexes.Add( v );
                        else
                        {
                            i++;
                            normalVertexesIll += v.Illuminance;
                        }

                    normalVertexesIll = normalVertexesIll / i;

                    foreach ( var v in correctionVertexes )
                    {
                        v.Illuminance = ( giface.Center.Illuminance + normalVertexesIll ) / 2;
                    }
                }


                //


            }
            //foreach ( var v in giface.Vertexes )
            //    
            //    {
            //        v.Illuminance = giface.Center.Illuminance;
            //    }

        }

        void IlluminanceBugsCorrectionCos1Zero( RenderPointsStructureDLE giRenderPointsStructure, float threshold = 0.3f )
        {
            // список откорректированных вершин
            var correctedVertexes = new List<TriangleVertexDLE>();

            foreach ( var giface in giRenderPointsStructure.RenderPointsFace )
            {

                // контраст между параметрами CounterRaysCos1Zero для всех трех точек
                var counterRaysCos1Contrasts = new float[]
                {
                    ((float) (giface.Vertexes[0].CounterRaysCos1Zero - giface.Vertexes[1].CounterRaysCos1Zero) / (giface.Vertexes[0].CounterRaysCos1Zero + giface.Vertexes[1].CounterRaysCos1Zero)),
                    ((float) (giface.Vertexes[1].CounterRaysCos1Zero - giface.Vertexes[2].CounterRaysCos1Zero) / (giface.Vertexes[1].CounterRaysCos1Zero + giface.Vertexes[2].CounterRaysCos1Zero)),
                    ((float) (giface.Vertexes[2].CounterRaysCos1Zero - giface.Vertexes[0].CounterRaysCos1Zero) / (giface.Vertexes[2].CounterRaysCos1Zero + giface.Vertexes[0].CounterRaysCos1Zero))
                };


                // если контраст бесконечность, то делаем равным 0
                for ( var i = 0; i < 3; i++ )
                    if ( float.IsNaN( counterRaysCos1Contrasts[i] ) )
                        counterRaysCos1Contrasts[i] = 0;


                var correctionVertexes = new List<TriangleVertexDLE>();

                // на основе контраста определяем корректируемые точки в треугольнике
                if ( counterRaysCos1Contrasts[0] > threshold || counterRaysCos1Contrasts[2] < -threshold )
                    correctionVertexes.Add( giface.Vertexes[0] );
                if ( counterRaysCos1Contrasts[0] < -threshold || counterRaysCos1Contrasts[1] > threshold )
                    correctionVertexes.Add( giface.Vertexes[1] );
                if ( counterRaysCos1Contrasts[1] < -threshold || counterRaysCos1Contrasts[2] > threshold )
                    correctionVertexes.Add( giface.Vertexes[2] );

                foreach ( var vertex in correctionVertexes )
                {
                    if ( correctedVertexes.Contains( vertex ) )
                        continue;

                    // рассчитываем результирующую яркость для корректируемой точки
                    var centeredIll = new Spectrum();
                    var fullLength = vertex.GIFaces.Where( x => x.Center != null ).Sum( x => ( x.Center.Position - vertex.Position ).Length );
                    foreach ( var face in vertex.GIFaces.Where( x => x.Center != null ) )
                    {
                        centeredIll += face.Center.Illuminance * ( face.Center.Position - vertex.Position ).Length / fullLength;
                    }

                    var coeff = 0.85f;
                    vertex.Illuminance = vertex.Illuminance * ( 1 - coeff ) + centeredIll * coeff;
                    correctedVertexes.Add( vertex );
                }
            }
        }


        RenderPointsStructureDLE GenerateCalculatedStructure( RenderPointsStructure renderPointsStructure )
        {
            var tick = Environment.TickCount;
            var result = new RenderPointsStructureDLE();

            foreach ( var obj in this.scene.Objects )
                foreach ( var face in obj.Faces )
                {
                    if ( face.Material.Reflectance == null )
                        continue;

                    if ( !renderPointsStructure.FaceRenderPoints.ContainsKey( face ) )
                        continue;

                    var a = new TriangleFaceDLE( face, renderPointsStructure.FaceRenderPoints[face] );
                    GenerateRenderPointsFace( a, result );
                }


            // Generate center points && set shadow points to vertexes
            foreach ( var face in result.RenderPointsFace )
            {
                var center = ( face.Vertexes[0].Position + face.Vertexes[1].Position + face.Vertexes[2].Position ) / 3;
                var direction = ( face.Vertexes[0].Direction + face.Vertexes[1].Direction + face.Vertexes[2].Direction ) / 3;
                direction.Normalize();
                var vertexCenter = new TriangleVertexDLE( face.Face, center, direction, face );
                face.Center = vertexCenter;
                result.RenderPointsVertexes.Add( vertexCenter );

            }


            if ( this.Log != null )
            {
                tick = Environment.TickCount - tick;
                this.Log.Message( string.Format( "GiDoubleLocalEstimation. GenerateCalculatedStructure: SubMeshes = {0}, CalcPoints = {1}, Time = {2}", result.RenderPointsFace.Count, result.RenderPointsVertexes.Count, tick ) );
            }
            return result;
        }

        static void GenerateRenderPointsFace( TriangleFaceDLE face, RenderPointsStructureDLE giRenderPointsStructure )
        {
            // TODO: переменная будет использована просто для поддержки цикла, но лучше переделать на нормальный while ( newFace != null )
            var result = new List<TriangleFaceDLE> { face };

            // добавляем грань в расчетные грани и добавляем 3 вершины грани в расчетные вершины, 
            // далее будем добавлять только новую грань и вершину
            giRenderPointsStructure.RenderPointsFace.Add( face );


            giRenderPointsStructure.RenderPointsVertexes.AddRange( face.Vertexes );
            //foreach (var vertex in face.Vertexes)
            //{
            //    if ( !giRenderPointsStructure.RenderPointsVertexes.Contains( vertex ) )
            //        giRenderPointsStructure.RenderPointsVertexes.Add( vertex );
            //    else
            //        throw new Exception("xx");
            //}




            for ( int i = 0; i < result.Count; i++ )
            {
                var currentFace = result[i];

                //if ( currentFace.RenderPoints.Count > 1000 )
                while ( currentFace.RenderPoints.Count > 64 )
                {

                    // divide face
                    // Ищем самую длинную сторону
                    Point3D v12 = currentFace.Vertexes[0].Position - currentFace.Vertexes[1].Position;
                    Point3D v23 = currentFace.Vertexes[1].Position - currentFace.Vertexes[2].Position;
                    Point3D v31 = currentFace.Vertexes[2].Position - currentFace.Vertexes[0].Position;

                    int[] k;
                    if ( v12.Length2 > v23.Length2 && v12.Length2 > v31.Length2 )
                        k = new int[] { 0, 1, 2 };
                    else if ( v23.Length2 > v12.Length2 && v23.Length2 > v31.Length2 )
                        k = new int[] { 1, 2, 0 };
                    else
                        k = new int[] { 2, 0, 1 };

                    // Вычисляю координату новой вершины - центра гиппотренузы
                    Point3D p = ( currentFace.Vertexes[k[0]].Position - currentFace.Vertexes[k[1]].Position ) / 2
                        + currentFace.Vertexes[k[1]].Position;
                    var newVertexDirection = TriangleFaceDLE.GetNearestRenderPoint( currentFace.RenderPoints, p ).Direction;
                    TriangleVertexDLE newVertex = new TriangleVertexDLE( currentFace.Face, p, newVertexDirection, currentFace );

                    // Создаю клон разбиваемого элемента
                    //MeshFace mf = (MeshFace)this[index].Clone();


                    // Меняем вершину у делимого элемента


                    var newPoints = new TriangleVertexDLE[3];
                    newPoints[k[1]] = currentFace.Vertexes[k[2]];
                    newPoints[k[2]] = currentFace.Vertexes[k[0]];
                    newPoints[k[0]] = newVertex;

                    // changed vertex
                    var changedVertex = currentFace.Vertexes[k[0]];
                    changedVertex.GIFaces.Remove( currentFace );

                    currentFace.Vertexes[k[0]] = newVertex;

                    var newFace = new TriangleFaceDLE( currentFace.Face, newPoints, currentFace.Face.Normal );
                    newPoints[0].GIFaces.Add( newFace );
                    newPoints[1].GIFaces.Add( newFace );
                    newPoints[2].GIFaces.Add( newFace );

                    IList<RenderPoint> oldPoints = new List<RenderPoint>();
                    foreach ( var renderPoint in currentFace.RenderPoints )
                    {
                        if ( !currentFace.IsPointInTriangleFace2( renderPoint ) )
                        {
                            //currentFace.RenderPoints.Remove( renderPoint );
                            newFace.RenderPoints.Add( renderPoint );
                        }
                        else
                            oldPoints.Add( renderPoint );
                    }

                    currentFace.RenderPoints = oldPoints;

                    //foreach ( var renderPoint in newFace.RenderPoints )
                    //    currentFace.RenderPoints.Remove( renderPoint );

                    if ( newFace.RenderPoints.Count > 0 )
                    {
                        // новая грань пригодна для расчетов, так как в ней есть расчетные точки
                        result.Add( newFace );
                        giRenderPointsStructure.RenderPointsFace.Add( newFace );
                        giRenderPointsStructure.RenderPointsVertexes.Add( newVertex );
                    }
                }
            }

            //return result;
        }
    }
}