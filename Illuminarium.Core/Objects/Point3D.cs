using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminarium.Core
{
    /// <summary>
    /// —труктура описывающа€ точку в 3D пространстве
    /// </summary>
    public struct Point3D
    {
        float? length;
        float? length2;

        public float X; //{ get; private set; }
        public float Y; //{ get; private set; }
        public float Z;// { get; private set; }

        public float Length
        {
            get
            {
                if ( length == null )
                    this.length = (float)Math.Sqrt( X * X + Y * Y + Z * Z );

                return length.Value;
            }
        }
        public float Length2
        {
            get
            {
                if ( length2 == null )
                    this.length2 = X * X + Y * Y + Z * Z;

                return length2.Value;
            }
        }

        public Point3D( float x, float y, float z )
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static float LenghtSquared( Point3D p1, Point3D p2 )
        {
            float x = p2.X - p1.X;
            float y = p2.Y - p1.Y;
            float z = p2.Z - p1.Z;

            return x * x + y * y + z * z;
        }

        /// <summary>
        /// ¬ычисление нормализованного вектора
        /// </summary>
        /// <param name="p">¬ектор</param>
        /// <returns></returns>
        public static Point3D Normalize( Point3D p )
        {
            float n = (float)Math.Sqrt( p.X * p.X + p.Y * p.Y + p.Z * p.Z );
            return p / n;
        }
        /// <summary>
        /// ¬екторное произведение двух векторов
        /// </summary>
        /// <param name="p1">ѕервый вектор</param>
        /// <param name="p2">¬торой вектор</param>
        /// <returns></returns>
        public static Point3D Cross( Point3D p1, Point3D p2 )
        {
            Point3D p = new Point3D();
            p.X = p1.Y * p2.Z - p1.Z * p2.Y;
            p.Y = p1.Z * p2.X - p1.X * p2.Z;
            p.Z = p1.X * p2.Y - p1.Y * p2.X;
            return p;
        }

        public static float Dot( Point3D v1, Point3D v2 )
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }

        public static Point3D operator +( Point3D p1, Point3D p2 )
        {
            Point3D p = new Point3D( p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z );
            return p;
        }
        public static Point3D operator +( Point3D p, Vector vector )
        {
            return new Point3D( p.X + vector.X, p.Y + vector.Y, p.Z + vector.Z );
        }
        public static Point3D operator -( Point3D p1, Point3D p2 )
        {
            Point3D p = new Point3D( p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z );
            return p;
        }
        public static Point3D operator -( Point3D p, Vector vector )
        {
            return new Point3D( p.X - vector.X, p.Y - vector.Y, p.Z - vector.Z );
        }
        public static Point3D operator +( Point3D p1, float p2 )
        {
            Point3D p = new Point3D( p1.X - p2, p1.Y - p2, p1.Z - p2 );
            return p;
        }
        public static Point3D operator *( Point3D p1, float p2 )
        {
            Point3D p = new Point3D( p1.X * p2, p1.Y * p2, p1.Z * p2 );
            return p;
        }
        public static Point3D operator *( Point3D p1, Point3D p2 )
        {
            Point3D p =

                new Point3D( p1.Y * p2.Z - p1.Z * p2.Y,
                      p1.Z * p2.X - p1.X * p2.Z,
                      p1.X * p2.Y - p1.Y * p2.X );
            return p;
        }
        public static Point3D operator /( Point3D p1, float p2 )
        {
            Point3D p = new Point3D( p1.X / p2, p1.Y / p2, p1.Z / p2 );
            return p;
        }

        public Point3D ChangeCoordinateSystem( Vector normal )
        {
            Vector v = new Vector( this.X, this.Y, this.Z, true );

            // направл€ющие векторы точки в мировой системе координат
            var cosElev = v.Z;
            var sinElev = Math.Sqrt( 1 - cosElev * cosElev );
            var cosAz = v.X / sinElev;
            var sinAz = v.Y / sinElev;


            // новый направл€ющий вектор в системе координат нормали
            var sqrtNcPhi = Math.Sqrt( ( 1 - cosElev * cosElev ) / ( 1 - normal.Z * normal.Z ) );

            var nv = new Vector(
                (float)( cosElev * normal.X - ( normal.Y * sinAz + normal.X * normal.Z * cosAz ) * sqrtNcPhi ),
                (float)( cosElev * normal.Y + ( normal.X * sinAz - normal.Y * normal.Z * cosAz ) * sqrtNcPhi ),
                (float)( cosElev * normal.Z + ( 1 - normal.Z * normal.Z ) * cosAz * sqrtNcPhi ),
                true );

            var result = nv.ToPoint3D() * this.Length;

            return result;
        }

        public override string ToString()
        {
            return string.Format( "x = {0}, y = {1}, z = {2}", this.X, this.Y, this.Z );
        }
    }
}
