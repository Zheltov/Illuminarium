using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.Lights
{
    public class RectangleLightFakeForSobolev : RectangleLight
    {
        public RectangleLightFakeForSobolev( Point3D position, float length, float width, Spectrum illuminance, IMaterial material, int samples = 32 )
            : base( position, length, width, illuminance, material, samples ) { }

        /// <summary>
        /// Вычисление форм-фактора с учетом фейковой точки.
        /// Для fakePoint производится вычисление непосредственно телесного угла, а косинус учитывается по основной точке.
        /// Например: Для тестирования Соболева при прямоугольном источнике, расчитываем фейк точку как точку под источником на 
        /// эквивалентном расстоянии как и point.
        /// </summary>
        /// <param name="point">Расчетная точка</param>
        /// <param name="fakePoint">Фейк точка для расчета непосредственно самого телесного угла</param>
        /// <returns>Форм-фактор</returns>
        public float GetFormFactor( Point3D point, Point3D fakePoint )
        {
            var sa1 = Math3D.SolidAngle(
                    this.Vertices[this.Faces[0].VertexIndexes[0]],
                    this.Vertices[this.Faces[0].VertexIndexes[1]],
                    this.Vertices[this.Faces[0].VertexIndexes[2]],
                    fakePoint );

            var sa2 = Math3D.SolidAngle(
                    this.Vertices[this.Faces[1].VertexIndexes[0]],
                    this.Vertices[this.Faces[1].VertexIndexes[1]],
                    this.Vertices[this.Faces[1].VertexIndexes[2]],
                    fakePoint );


            // косинус угла на источник от точки
            var cosa = Math.Abs( Vector.Dot( this.Direction, new Vector( this.Position, point, true ) ) );


            return ( sa1 + sa2 ) * cosa;
        }

        public override Spectrum GetIlluminance( IRayTracer rayTracer, IMaterial material, Point3D point, Vector normal, Vector view )
        {
            var fakePoint = new Point3D( 0, 0, ( this.Position - point ).Length );

            var formFactor = this.GetFormFactor( point, fakePoint );

            // если нет расчетных точек, то генерим их на источнике
            if ( this.LightSamplePoints == null )
            {
                this.GenerateLightSamplePoints();
            }

            var result = new Spectrum();

            var toLightVector = new Vector( point, this.Position );
            //var toLightRay = new Ray( point, this.Position );

            var cosR = Math.Abs( -Vector.Dot( toLightVector, this.Direction ) );
            result += material.Reflectance.BRDF( toLightVector, normal, view ) * cosR;

            // форм фактор уменьшаяется за счет не попавших лучей
            // (float) Math.PI - из ядра уравнения
            result = this.Illuminance * result * formFactor / Constants.PI;

            //result = result * this.Illuminance / (float)( this.Samples );
            return result;
        }

        public override Spectrum GetIlluminanceSourceSurface(Point3D point, Vector direction)
        {
            //return this.IlluminanceLocalEstimationHigh;
            return this.Illuminance;
        }

        public override LightRay RandomRay()
        {
            // get random point on light
            //float dx = ( 0.5f - (float)this.randStatic.NextDouble() ) * this.Length / 2;
            //float dy = ( 0.5f - (float)this.randStatic.NextDouble() ) * this.Width / 2;
            float x = this.Position.X;
            float y = this.Position.Y;
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
            double cPhi = 2 * this.randStatic.NextDouble() - 1;
            double sPhi = Math.Sqrt( 1 - cPhi * cPhi );

            // get light ray
            var point = new Point3D( x, y, z );
            var direction = new Vector( -(float)( sPhi * cTheta ), -(float)( sPhi * sTheta ), -(float)cPhi );

            return new LightRay( point, direction, this.GetIlluminanceSourceSurface( point, direction ) );
        }
    }
}
