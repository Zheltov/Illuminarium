using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public struct Vector
    {
        float[,] projectionMatrix;
        float? length;
        float? length2;

        public float X;// { get; private set; }
        public float Y;// { get; private set; }
        public float Z;// { get; private set; }

        public float[,] ProjectionMatrix
        {
            get
            {
                if ( projectionMatrix == null )
                {
                    // CalculateProjectionMatrix
                    var l = this.X;
                    var m = this.Y;
                    var n = this.Z;

                    var l2 = l * l;
                    var m2 = m * m;
                    var n2 = n * n;

                    var c = ( l2 + m2 + n2 );

                    //int[,] n4 = new int[3, 2] { { 1, 2 }, { 3, 4 }, { 5, 6 } };

                    this.projectionMatrix = new float[3, 3] { { ( m2 + n2 ), ( -m * l ), ( -n * l ) }, { ( -l * m ), ( l2 + n2 ), ( -n * m ) }, { ( -l * n ), ( -m * n ), ( l2 + m2 ) } };
                }

                return this.projectionMatrix;
            }
        }

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

        public Vector( float theta, float phi )
            : this()
        {
            double rcoselev = Math.Cos( theta );

            this.X = (float)( rcoselev * Math.Cos( phi ) );
            this.Y = (float)( rcoselev * Math.Sin( phi ) );
            this.Z = (float)Math.Sin( theta );

            this.Normalize();
        }
        public Vector( float x, float y, float z, bool normalize = false )
            : this()
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            if ( normalize )
                this.Normalize();
        }
        public Vector( Point3D point, bool normalize = false )
            : this()
        {
            this.X = point.X;
            this.Y = point.Y;
            this.Z = point.Z;
            if ( normalize )
                this.Normalize();
        }
        public Vector( Point3D from, Point3D to, bool normalize = false )
            : this()
        {
            this.X = to.X - from.X;
            this.Y = to.Y - from.Y;
            this.Z = to.Z - from.Z;
            if ( normalize )
                this.Normalize();
        }

        public void Normalize()
        {
            float n = (float)Math.Sqrt( this.X * this.X + this.Y * this.Y + this.Z * this.Z );
            this.X = this.X / n;
            this.Y = this.Y / n;
            this.Z = this.Z / n;
        }
        public Vector Reflect( Vector n )
        {
            return Vector.Reflect( this, n );
        }
        public Vector Projected( Vector vector )
        {
            if ( Math.Abs( X ) < Constants.Epsilon && Math.Abs( Y ) < Constants.Epsilon )
            {
                if ( Z > 0 )
                    return new Vector( vector.X, vector.Y, vector.Z );
                else
                    return new Vector( vector.X, vector.Y, -vector.Z );
            }
            else
                return Vector.Multiply3( vector, this.ProjectionMatrix );
        }
        public Point3D ToPoint3D()
        {
            return new Point3D( this.X, this.Y, this.Z );
        }
        public Vector ChangeCoordinateSystem( Vector normal )
        {
            if ( Math.Abs( normal.X ) < Constants.Epsilon2 && Math.Abs( normal.Y ) < Constants.Epsilon2 )
            {
                if ( normal.Z > 0 )
                    return this;
                else
                    return new Vector( X, Y, -Z );
            }

            // направляющие векторы точки в мировой системе координат
            var cosElev = Z;
            var sinElev = Math.Sqrt( 1 - cosElev * cosElev );
            
            var cosAz = ( Math.Abs( sinElev ) < Constants.Epsilon2 ) ? 0 : X / sinElev;
            var sinAz = ( Math.Abs( sinElev ) < Constants.Epsilon2 ) ? 1 : Y / sinElev;


            // новый направляющий вектор в системе координат нормали
            var sqrtNcPhi = Math.Sqrt( ( 1 - cosElev * cosElev ) / ( 1 - normal.Z * normal.Z ) );

            var nv = new Vector(
                (float)( cosElev * normal.X - ( normal.Y * sinAz + normal.X * normal.Z * cosAz ) * sqrtNcPhi ),
                (float)( cosElev * normal.Y + ( normal.X * sinAz - normal.Y * normal.Z * cosAz ) * sqrtNcPhi ),
                (float)( cosElev * normal.Z + ( 1 - normal.Z * normal.Z ) * cosAz * sqrtNcPhi ),
                true );

            //var result = nv.ToPoint3D() * this.Length;

            if ( float.IsNaN( nv.X ) )
                nv.X = 0;

            return nv;
        }
        public SphCoord ToSphCoord()
        {
            var hypotxy = Math.Sqrt( X * X + Y * Y );

            var theta = Math.Atan2( Z, hypotxy );
            var phi = Math.Atan2( Y, X );

            return new SphCoord( theta, phi );
        }
        public Vector Reverse()
        {
            return new Vector( -X, -Y, -Z );
        }


        public static Vector Cross( Vector p1, Vector p2, bool normalize = false )
        {
            Vector p = new Vector();
            p.X = p1.Y * p2.Z - p1.Z * p2.Y;
            p.Y = p1.Z * p2.X - p1.X * p2.Z;
            p.Z = p1.X * p2.Y - p1.Y * p2.X;

            if ( normalize )
                p.Normalize();

            return p;
        }
        /// <summary>
        /// See http://stackoverflow.com/questions/18904153/matrix-multiplication-function
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static Vector Multiply3( Vector vector, float[,] matrix )
        {
            Vector aux = new Vector(
                 vector.X * matrix[0, 0] + vector.Y * matrix[1, 0] + vector.Z * matrix[2, 0],
                 vector.X * matrix[0, 1] + vector.Y * matrix[1, 1] + vector.Z * matrix[2, 1],
                 vector.X * matrix[0, 2] + vector.Y * matrix[1, 2] + vector.Z * matrix[2, 2] );
            return aux;
        }
        public static Vector Reflect( Vector i, Vector n )
        {
            return i - 2 * n * Dot( i, n );
        }
        public static float Dot( Vector v1, Vector v2 )
        {
            return v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        }
        public static Vector Normalize( Vector vector )
        {
            float n = (float)Math.Sqrt( vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z );
            return vector / n;
        }

        public static Vector operator /( Vector vector, float f )
        {
            Vector p = new Vector( vector.X / f, vector.Y / f, vector.Z / f );
            return p;
        }
        public static Vector operator *( Vector vector, float f )
        {
            return new Vector( vector.X * f, vector.Y * f, vector.Z * f );
        }
        public static Vector operator *( float f, Vector vector )
        {
            return new Vector( vector.X * f, vector.Y * f, vector.Z * f );
        }
        public static Vector operator -( Vector vector1, Vector vector2 )
        {
            return new Vector( vector1.X - vector2.X, vector1.Y - vector2.Y, vector1.Z - vector2.Z );
        }
        public static Vector operator +( Vector p1, Vector p2 )
        {
            return new Vector( p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z );
        }


        public override string ToString()
        {
            return string.Format( "x = {0}, y = {1}, z = {2}", this.X, this.Y, this.Z );
        }
    }

    public class SphCoord
    {
        public double Theta { get; set; }
        public double Phi { get; set; }

        public SphCoord( double theta, double phi )
        {
            Theta = theta;
            Phi = phi;
        }

        public override string ToString()
        {
            return string.Format( "Theta = {0}; Phi = {1}", Theta, Phi );
        }
    }

}
