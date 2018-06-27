using Illuminarium.Core.Debug;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.Lights
{
    public class RectangleLight : LightBase, ILight, IObj
    {
        protected readonly Random randStatic = new Random();

        protected IList<LightSamplePoint> LightSamplePoints;

        public Vector Direction { get; set; }
        public Spectrum Illuminance { get; set; }
        public float Length { get; private set; }
        public float Width { get; private set; }
        public int Samples { get; set; }

        public string Name { get; set; }
        public IList<IFace> Faces { get; set; }
        public IList<Point3D> Vertices { get; set; }

        public RectangleLight( Point3D position, float length, float width, Spectrum illuminance, IMaterial material, int samples = 32 )
        {
            this.Length = length;
            this.Width = width;
            this.Position = position;
            this.Illuminance = illuminance;
            this.Samples = samples;
            this.Direction = new Vector( 0, 0, -1, true );

            // create light geometry
            this.Faces = new List<IFace>();
            this.Vertices = new List<Point3D>();

            this.Vertices.Add( new Point3D( this.Position.X - this.Length / 2, this.Position.Y + this.Width / 2, this.Position.Z ) );
            this.Vertices.Add( new Point3D( this.Position.X + this.Length / 2, this.Position.Y + this.Width / 2, this.Position.Z ) );
            this.Vertices.Add( new Point3D( this.Position.X - this.Length / 2, this.Position.Y - this.Width / 2, this.Position.Z ) );
            this.Vertices.Add( new Point3D( this.Position.X + this.Length / 2, this.Position.Y - this.Width / 2, this.Position.Z ) );

            this.Faces.Add( new Face( this, new int[3] { 0, 1, 2 } ) { Material = material } );
            this.Faces.Add( new Face( this, new int[3] { 2, 1, 3 } ) { Material = material } );
        }

        public virtual Spectrum GetIlluminanceSourceSurface( Point3D point, Vector direction )
        {
            // !!!!!!!! TO DO COSIN
            var cosa = Math.Abs( Vector.Dot( this.Direction, direction ) );

            return this.Illuminance * cosa;
        }

        public virtual LightRay RandomRay()
        {
            // get random point on light
            float dx = ( 0.5f - (float)this.randStatic.NextDouble() ) * this.Length / 2;
            float dy = ( 0.5f - (float)this.randStatic.NextDouble() ) * this.Width / 2;
            float x = this.Position.X + dx;
            float y = this.Position.Y + dy;
            float z = this.Position.Z;

            // get random direction
            double d = 2;
            double u = 0;
            double v = 0;
            while ( d > 1 )
            {
                u = 1 - 2 * this.randStatic.NextDouble();
                v = 1 - 2 * this.randStatic.NextDouble();
                d = u * u + v * v;
            }

            double Sqrt_d = Math.Sqrt( d );
            double cTheta = u / Sqrt_d;
            double sTheta = v / Sqrt_d;
            double sPhi = Math.Sqrt( this.randStatic.NextDouble() );
            double cPhi = Math.Sqrt( 1 - sPhi * sPhi );

            // get light ray
            var point = new Point3D( x, y, z );
            var direction = new Vector( -(float)( sPhi * cTheta ), -(float)( sPhi * sTheta ), -(float)cPhi );

            return new LightRay( point, direction, this.GetIlluminanceSourceSurface( point, direction ) );
        }

        public float GetFormFactor( Point3D point )
        {
            var sa1 = Math3D.SolidAngle(
                    this.Vertices[this.Faces[0].VertexIndexes[0]],
                    this.Vertices[this.Faces[0].VertexIndexes[1]],
                    this.Vertices[this.Faces[0].VertexIndexes[2]],
                    point );

            var sa2 = Math3D.SolidAngle(
                    this.Vertices[this.Faces[1].VertexIndexes[0]],
                    this.Vertices[this.Faces[1].VertexIndexes[1]],
                    this.Vertices[this.Faces[1].VertexIndexes[2]],
                    point );

            // косинус угла на источник от точки
            var cosa = Math.Abs( Vector.Dot( this.Direction, new Vector( this.Position, point, true ) ) );

            return ( sa1 + sa2 ) * cosa;
        }

        public float GetFormFactorSmirnov( Point3D point, Vector normal )
        {
            // вектора на углы источника
            // порядок соединения берем с учетом формирования вершин при создании геометрии источника
            Vector[] vectors = new Vector[4]
            {
                new Vector( point, this.Vertices[0], true),
                new Vector( point, this.Vertices[1], true),
                new Vector( point, this.Vertices[3], true),
                new Vector( point, this.Vertices[2], true)
            };

            // спроецированне вектора
            Vector[] vectorsProjected = new Vector[4]
            {
                normal.Projected( vectors[0] ),
                normal.Projected( vectors[1] ),
                normal.Projected( vectors[2] ),
                normal.Projected( vectors[3] )
            };

            Vector[] vectorsProjectedNormalized = new Vector[4]
            {
                Vector.Normalize( vectorsProjected[0] ),
                Vector.Normalize( vectorsProjected[1] ),
                Vector.Normalize( vectorsProjected[2] ),
                Vector.Normalize( vectorsProjected[3] )
            };

            // углы между нормалью и векторами на источник
            double[] cosx = new double[4]
            {
                Vector.Dot( normal, vectors[0] ),
                Vector.Dot( normal, vectors[1] ),
                Vector.Dot( normal, vectors[2] ),
                Vector.Dot( normal, vectors[3] )
            };

            double[] sinx = new double[4]
            {
                Math.Sqrt( 1 - cosx[0] * cosx[0] ),
                Math.Sqrt( 1 - cosx[1] * cosx[1] ),
                Math.Sqrt( 1 - cosx[2] * cosx[2] ),
                Math.Sqrt( 1 - cosx[3] * cosx[3] )
            };

            // cos(a - b) = cosa * cosb + sina * sinb;
            // получаем "плоскостные по нормали" углы между векторами на источник
            double[] cos = new double[4]
            {
                cosx[0]*cosx[1] + sinx[0]*sinx[1],
                cosx[1]*cosx[2] + sinx[1]*sinx[2],
                cosx[2]*cosx[3] + sinx[2]*sinx[3],
                cosx[3]*cosx[0] + sinx[3]*sinx[0]
            };


            // нормали к 4 секторам
            Vector[] normals = new Vector[4]
            {
                Vector.Cross( vectors[0], vectors[1], true ),
                Vector.Cross( vectors[1], vectors[2], true ),
                Vector.Cross( vectors[2], vectors[3], true ),
                Vector.Cross( vectors[3], vectors[0], true ),
            };


            // определяем знаки для площадей секторов
            float[] signs = new float[4]
            {
                Vector.Dot( normal, normals[0] ) > 0 ? 1 : -1,
                Vector.Dot( normal, normals[1] ) > 0 ? 1 : -1,
                Vector.Dot( normal, normals[2] ) > 0 ? 1 : -1,
                Vector.Dot( normal, normals[3] ) > 0 ? 1 : -1
            };

            double[] dot = new double[4]
            {
                Math.Abs( Vector.Dot( vectorsProjectedNormalized[0], vectorsProjectedNormalized[1] ) ),
                Math.Abs( Vector.Dot( vectorsProjectedNormalized[1], vectorsProjectedNormalized[2] ) ),
                Math.Abs( Vector.Dot( vectorsProjectedNormalized[2], vectorsProjectedNormalized[3] ) ),
                Math.Abs( Vector.Dot( vectorsProjectedNormalized[3], vectorsProjectedNormalized[0] ) )
            };


            // аркосинусы углов между направляющими секторов
            double[] acos = new double[4]
            {
                Math.Acos( Math.Abs( dot[0] > 1 ? 1 : dot[0] ) ),
                Math.Acos( Math.Abs( dot[1] > 1 ? 1 : dot[1] ) ),
                Math.Acos( Math.Abs( dot[2] > 1 ? 1 : dot[2] ) ),
                Math.Acos( Math.Abs( dot[3] > 1 ? 1 : dot[3] ) )
            };

            //var f = acos[0] + acos[1] + acos[2] + acos[3];

            double[] lenght2 = new double[4]
            {
                vectorsProjected[0].Length2 > vectorsProjected[1].Length2 ? vectorsProjected[0].Length2 : vectorsProjected[1].Length2,
                vectorsProjected[1].Length2 > vectorsProjected[2].Length2 ? vectorsProjected[1].Length2 : vectorsProjected[2].Length2,
                vectorsProjected[2].Length2 > vectorsProjected[3].Length2 ? vectorsProjected[2].Length2 : vectorsProjected[3].Length2,
                vectorsProjected[3].Length2 > vectorsProjected[0].Length2 ? vectorsProjected[3].Length2 : vectorsProjected[0].Length2,
            };

            //double[] lenght2 = new double[4]
            //{
            //    ( vectorsProjected[0].Length2 + vectorsProjected[1].Length2 ) / 2,
            //    ( vectorsProjected[1].Length2 + vectorsProjected[2].Length2 ) / 2,
            //    ( vectorsProjected[2].Length2 + vectorsProjected[3].Length2 ) / 2,
            //    ( vectorsProjected[3].Length2 + vectorsProjected[0].Length2 ) / 2,
            //};


            // площади секторов
            double[] areas = new double[4]
            {
               signs[0] * ( acos[0] / 2 ) * lenght2[0] * cos[0],
               signs[1] * ( acos[1] / 2 ) * lenght2[1] * cos[1],
               signs[2] * ( acos[2] / 2 ) * lenght2[2] * cos[2],
               signs[3] * ( acos[3] / 2 ) * lenght2[3] * cos[3]
            };



            // через площади треугольников
            double[] edges = new double[4]
            {
                (vectorsProjected[0] - vectorsProjected[1]).Length,
                (vectorsProjected[1] - vectorsProjected[2]).Length,
                (vectorsProjected[2] - vectorsProjected[3]).Length,
                (vectorsProjected[3] - vectorsProjected[0]).Length
            };

            double[] halfp = new double[4]
            {
                (vectorsProjected[0].Length + vectorsProjected[1].Length + edges[0]) / 2,
                (vectorsProjected[1].Length + vectorsProjected[2].Length + edges[1]) / 2,
                (vectorsProjected[2].Length + vectorsProjected[3].Length + edges[2]) / 2,
                (vectorsProjected[3].Length + vectorsProjected[0].Length + edges[3]) / 2
            };

            double[] areas2 = new double[4]
            {
               signs[0] * Math.Sqrt(halfp[0] * ( halfp[0] - vectorsProjected[0].Length ) * ( halfp[0] - vectorsProjected[1].Length ) * ( halfp[0] - edges[0] )  ),
               signs[1] * Math.Sqrt(halfp[1] * ( halfp[1] - vectorsProjected[1].Length ) * ( halfp[1] - vectorsProjected[2].Length ) * ( halfp[1] - edges[1] )  ),
               signs[2] * Math.Sqrt(halfp[2] * ( halfp[2] - vectorsProjected[2].Length ) * ( halfp[2] - vectorsProjected[3].Length ) * ( halfp[2] - edges[2] )  ),
               signs[3] * Math.Sqrt(halfp[3] * ( halfp[3] - vectorsProjected[3].Length ) * ( halfp[3] - vectorsProjected[0].Length ) * ( halfp[3] - edges[3] )  ),
            };

            for ( int i = 0; i < 4; i++ )
                if ( double.IsNaN( areas2[i] ) )
                    areas2[i] = 0;
            var result = Math.Abs( areas[0] + areas[1] + areas[2] + areas[3] );

            RayDebugStaticCollection.Add( new Ray( point, vectors[0] ), Color.Red );
            RayDebugStaticCollection.Add( new Ray( point, vectors[1] ), Color.Green );
            RayDebugStaticCollection.Add( new Ray( point, vectors[2] ), Color.Blue );
            RayDebugStaticCollection.Add( new Ray( point, vectors[3] ), Color.Yellow );

            //RayDebugStaticCollection.Add( new Ray( point, normalsN[2] ), Color.Blue );
            //RayDebugStaticCollection.Add( new Ray( point, normalsN[3] ), Color.Yellow );

            //RayDebugStaticCollection.Add( new Ray( point, normalsNProjected[0] ), Color.Cyan );
            //RayDebugStaticCollection.Add( new Ray( point, normalsNProjected[1] ), Color.Cyan );
            //RayDebugStaticCollection.Add( new Ray( point, normalsNProjected[2] ), Color.Cyan );
            //RayDebugStaticCollection.Add( new Ray( point, normalsNProjected[3] ), Color.Cyan );

            //RayDebugStaticCollection.Add( new Ray( point, vectorsProjected[0] ), Color.Red );
            //RayDebugStaticCollection.Add( new Ray( point, vectorsProjected[1] ), Color.Green );
            //RayDebugStaticCollection.Add( new Ray( point, vectorsProjected[2] ), Color.Blue );
            //RayDebugStaticCollection.Add( new Ray( point, vectorsProjected[3] ), Color.Yellow );

            //RayDebugStaticCollection.Add( new Ray( point, normals[0] ), signs[0] > 0 ? Color.Red : Color.Black );
            //RayDebugStaticCollection.Add( new Ray( point, normals[1] ), signs[1] > 0 ? Color.Green : Color.Black );
            //RayDebugStaticCollection.Add( new Ray( point, normals[2] ), signs[2] > 0 ? Color.Blue : Color.Black );
            //RayDebugStaticCollection.Add( new Ray( point, normals[3] ), signs[3] > 0 ? Color.Yellow : Color.Black );

            //return 0;


            // соотносим площадь к площади всего круга
            result = result / Math.PI;

            if ( double.IsNaN( result ) )
                result = 1;

            //if ( result == float.NaN )
            //    result = 0;

            return (float)result;
        }

        public float GetFormFactorApprox( Point3D point, Vector normal )
        {
            //var xxx = randStatic.Next( 10000 ) == 500;
            // определяем форм-фактор источника
            Vector[] ffVertexes = new Vector[4];
            for ( int i = 0; i < 4; i++ )
            {
                var vertex = this.Vertices[i];

                // единичный вектор на угол источника
                var vertexVector = new Vector( point, vertex, true );

                // проецируем вектор на плоскость в которой находится точка
                var projectedVector = normal.Projected( vertexVector ); //Vector.Multiply( vertexVector, normal.ProjectionMatrix );

                ffVertexes[i] = projectedVector;

            }

            // Вычисляем формфактор как площадь усредненного прямоугольника
            // индексы идут в соответствии с процессом формирования геометрии светильника
            float[] edgesLength = new float[4]
            {
                (ffVertexes[0] - ffVertexes[1]).ToPoint3D().Length,
                (ffVertexes[1] - ffVertexes[3]).ToPoint3D().Length,
                (ffVertexes[2] - ffVertexes[3]).ToPoint3D().Length,
                (ffVertexes[2] - ffVertexes[0]).ToPoint3D().Length
            };

            // сортируем
            edgesLength = edgesLength.OrderBy( x => x ).ToArray();

            float area = ( edgesLength[0] + edgesLength[1] ) * ( edgesLength[2] + edgesLength[3] ) / 2;

            // форм фактор как отношение площади к полной площади круга
            float formFactor = area / (float)Math.PI;

            return formFactor;
        }

        public virtual Spectrum GetIlluminance( IRayTracer rayTracer, IMaterial material, Point3D point, Vector normal, Vector view )
        {

            // проверяем что нормали сонаправлены

            var xx = Vector.Dot( normal, this.Direction );
            if ( xx > Constants.Epsilon || point.Z > this.Position.Z )
                return new Spectrum();

            var formFactor = this.GetFormFactor( point );

            // если нет расчетных точек, то генерим их на источнике
            if ( this.LightSamplePoints == null )
            {
                this.GenerateLightSamplePoints();
            }

            var result = new Spectrum();

            // цикл по всем расчетным точкам на источнике
            int raysHited = 0;
            foreach ( var lightSamplePoint in this.LightSamplePoints )
            {
                var toLight = lightSamplePoint.Point - point;
                var toLightVector = new Vector( toLight, true );
                //var toLightRay = new Ray( point, lightSamplePoint.Point );
                if ( !rayTracer.Occluded( point, toLightVector, Constants.Epsilon, toLight.Length - Constants.Epsilon ) )
                {
                    // источник косинусный
                    var cosR = Math.Abs( -Vector.Dot( toLightVector, this.Direction ) );
                    result += material.Reflectance.BRDF( toLightVector.Reverse(), normal, view ) * cosR;
                    raysHited++;
                }
            }

            if ( raysHited == 0 )
                return new Spectrum();

            // усредняем BRDF
            result = result / (float)raysHited;

            // форм фактор уменьшаяется за счет не попавших лучей
            // (float) Math.PI - из ядра уравнения
            result = this.Illuminance * result * formFactor * ( raysHited / (float)( this.Samples ) );

            //result = result * this.Illuminance / (float)( this.Samples );
            return result;
        }
        public virtual IList<Spectrum> GetIlluminance( IRayTracer rayTracer, IMaterial material, Point3D point, Vector normal, IList<Vector> view )
        {

            var results = new List<Spectrum>();
            foreach ( var item in view )
                results.Add( new Spectrum() );


            // проверяем что нормали сонаправлены
            var xx = Vector.Dot( normal, this.Direction );
            if ( xx > Constants.Epsilon || point.Z > this.Position.Z )
                return results;


            var formFactor = this.GetFormFactor( point );

            // если нет расчетных точек, то генерим их на источнике
            if ( this.LightSamplePoints == null )
            {
                this.GenerateLightSamplePoints();
            }

            // цикл по всем расчетным точкам на источнике
            int raysHited = 0;
            foreach ( var lightSamplePoint in this.LightSamplePoints )
            {
                var toLight = lightSamplePoint.Point - point;
                var toLightVector = new Vector( toLight, true );
                //var toLightRay = new Ray( point, lightSamplePoint.Point );
                if ( !rayTracer.Occluded( point, toLightVector, Constants.Epsilon, toLight.Length - Constants.Epsilon ) )
                {
                    // источник косинусный
                    var cosR = Math.Abs( -Vector.Dot( toLightVector, this.Direction ) );
                    for ( int i = 0; i < view.Count; i++ )
                        results[i] += material.Reflectance.BRDF( toLightVector.Reverse(), normal, view[i] ) * cosR;
                    raysHited++;
                }
            }

            if ( raysHited == 0 )
                return results;

            // усредняем BRDF
            for ( int i = 0; i < view.Count; i++ )
                results[i] = results[i] / (float)raysHited;

            // форм фактор уменьшаяется за счет не попавших лучей (float) Math.PI - из ядра уравнения
            for ( int i = 0; i < view.Count; i++ )
                results[i] = this.Illuminance * results[i] * formFactor * ( raysHited / (float)( this.Samples ) );

            return results;
        }

        /// <summary>
        /// Равномерно по источнику генерим точки
        /// </summary>
        protected void GenerateLightSamplePoints()
        {
            LightSamplePoints = new List<LightSamplePoint>();

            var n = (int)Math.Sqrt( Samples ) + 1;
            var l2 = Length / 2;
            var w2 = Width / 2;

            for ( float dx = -l2; dx <= l2; dx += Length / ( n - 1 ) )
                for ( float dy = -w2; dy <= w2; dy += Width / ( n - 1 ) )
                {
                    var point = new Point3D( Position.X + dx, Position.Y + dy, Position.Z );
                    LightSamplePoints.Add( new LightSamplePoint( point, Direction ) );
                    //RayDebugStaticCollection.Add( new Ray( point, Direction ), Color.Blue );
                }
        }

        protected void GenerateLightSamplePointsOld()
        {
            Random rand = new Random();
            this.LightSamplePoints = new List<LightSamplePoint>();
            for ( int i = 0; i < this.Samples; i++ )
            {
                var dl = (float)( this.Length / 2f - this.Length * rand.NextDouble() );
                var dw = (float)( this.Width / 2f - this.Width * rand.NextDouble() );
                this.LightSamplePoints.Add( new LightSamplePoint( new Point3D( this.Position.X + dl, this.Position.Y + dw, this.Position.Z ), this.Direction ) );
            }
        }


    }

    public class LightSamplePoint
    {
        public Point3D Point { get; set; }
        public Vector Normal { get; set; }

        public LightSamplePoint( Point3D point, Vector normal )
        {
            this.Point = point;
            this.Normal = normal;
        }
    }
}