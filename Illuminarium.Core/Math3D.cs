using Illuminarium.Core.Debug;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public static class Math3D
    {
        /// <summary>
        /// Телесный угол треугольника относительно начала координат
        /// </summary>
        /// <param name="p1">Вершина треугольника</param>
        /// <param name="p2">Вершина треугольника</param>
        /// <param name="p3">Вершина треугольника</param>
        /// <returns>Телесный угол</returns>
        public static float SolidAngle( Point3D p1, Point3D p2, Point3D p3 )
        {
            float result = 0;

            // numerator
            var crossp2p3 = Point3D.Cross( p2, p3 );
            var numerator = Point3D.Dot( p1, crossp2p3 );

            // denominator
            var denominator = p1.Length * p2.Length * p3.Length + Point3D.Dot( p1, p2 ) * p3.Length + Point3D.Dot( p2, p3 ) * p1.Length + Point3D.Dot( p3, p1 ) * p2.Length;

            result = 2 * (float)Math.Abs( Math.Atan( numerator / denominator ) );

            return result;
        }

        /// <summary>
        /// Телесный угол треугольника относительно точки viewPoint
        /// </summary>
        /// <param name="p1">Вершина треугольника</param>
        /// <param name="p2">Вершина треугольника</param>
        /// <param name="p3">Вершина треугольника</param>
        /// <param name="viewPoint"></param>
        /// <returns></returns>
        public static float SolidAngle( Point3D p1, Point3D p2, Point3D p3, Point3D viewPoint )
        {
            var r1 = p1 - viewPoint;
            var r2 = p2 - viewPoint;
            var r3 = p3 - viewPoint;

            return SolidAngle( r1, r2, r3 );
        }

        public static double TriangleAreaByGeron( double a, double b, double c )
        {
            var p = ( a + b + c ) / 2;

            return Math.Sqrt( Math.Abs( p * ( p - a ) * ( p - b ) * ( p - c ) ) );
        }
    }
}