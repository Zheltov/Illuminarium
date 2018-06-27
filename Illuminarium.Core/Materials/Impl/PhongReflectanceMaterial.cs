using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.Materials
{
    public class PhongReflectanceMaterial : IReflectanceMaterial
    {
        public Spectrum Reflectance { get; set; }

        public float Shininess { get; set; }

        public float SpecularCoeff { get; set; }

        public PhongReflectanceMaterial( Spectrum reflectance, float shininess, float specularCoeff )
        {
            this.Reflectance = reflectance;
            this.Shininess = shininess;
            this.SpecularCoeff = specularCoeff;
        }


        // (Vector v, Vector l, Vector n)
        public Spectrum BRDF( Vector fall, Vector normal, Vector view )
        {
            var mirror = fall.Reflect( normal );
            var cosR = Vector.Dot( view, mirror );            
            if ( cosR < 0 )
                return new Spectrum();

            cosR = Math.Abs( cosR );
            if ( cosR > 1 )
                cosR = 1;

            //var result = Reflectance * (float)Math.Pow( cosR, this.Shininess );

            var result = this.Reflectance * ( 1 - this.SpecularCoeff ) + this.Reflectance * this.SpecularCoeff * (float)Math.Pow( cosR, this.Shininess );

            if ( float.IsNaN( result.R ) || float.IsNaN( result.G ) || float.IsNaN( result.B ) )
                throw new Exception();

            //var cosR = -Vector.Dot( v, l.Reflect( n ) );

            //return albedo * ( 1 - specularCoeff ) + new Vector( 1, 1, 1 ) * specularCoeff * (float)Math.Pow( cosR, shininess );

            //if ( result.Values[0] > 1 || result.Values[1] > 1 || result.Values[2] > 1 )
            //    throw new NotImplementedException();


            return result; /*this.Reflectance * ( 1 - this.SpecularCoeff ) + */

            //var cosa = Math.Abs( Vector.Dot( view, normal ) );
            //return this.Reflectance * cosa;
        }

        public Vector RandomReflectedDirection( Vector fall, Vector normal )
        {
            double d = 2;
            double u = 0;
            double v = 0;
            while ( d > 1 )
            {
                u = 1 - 2 * _random.NextDouble();
                v = 1 - 2 * _random.NextDouble();
                d = u * u + v * v;
            }

            double sqrtD = Math.Sqrt( d );
            double cPhi = u / sqrtD;
            double sPhi = v / sqrtD;
            var a = _random.NextDouble();
            double sTheta = Math.Pow( a, 1 / ( Shininess + 1 ) );
            double cTheta = Math.Sqrt( 1 - sTheta * sTheta );

            var direction = new Vector( (float)( cTheta * cPhi ), (float)( cTheta * sPhi ), (float)sTheta, true );
            var mirror = fall.Reflect( normal );
            return direction.ChangeCoordinateSystem( mirror );

            /*
            

            if ( Math.Abs( mirror.Z ) > ( 1 - Constants.Epsilon ) )

                return direction.Projected( mirror );
                */
        }

        public Vector RandomReflectedDirectionOld(Vector fall, Vector normal)
        {
            //Розыгранный луч нужно прожектировать не по нормали, а по зеркально отраженному лучу!!!!!

            // get random direction
            double d = 2;
            double u = 0;
            double v = 0;
            while ( d > 1 )
            {
                u = 1 - 2 * _random.NextDouble();
                v = 1 - 2 * _random.NextDouble();
                d = u * u + v * v;
            }

            double sqrtD = Math.Sqrt( d );
            double cTheta = u / sqrtD;
            double sTheta = v / sqrtD;

            //( rand ) ^ ( 1 / ( cosN + 1 ) );

            //var rnd = this.randStatic.NextDouble();
            double sPhi = Math.Pow( _random.NextDouble(), 1 / ( Shininess + 1 ) );
            double cPhi = Math.Sqrt( 1 - sPhi * sPhi );

            var a = sPhi;
            var b = cPhi;

            sPhi = cPhi;
            cPhi = a;


            // to normal system coordinate
            if ( Math.Abs( normal.Z ) > ( 1 - Constants.Epsilon ) )
            {
                if ( normal.Z > 0 )
                    return new Vector( (float)( sPhi * cTheta ), (float)( sPhi * sTheta ), (float)cPhi );
                else
                    return new Vector( -(float)( sPhi * cTheta ), -(float)( sPhi * sTheta ), -(float)cPhi );
            }

            var sqrtNcPhi = Math.Sqrt( ( 1 - cPhi * cPhi ) / ( 1 - normal.Z * normal.Z ) );

            return new Vector(
                (float)( cPhi * normal.X - ( normal.Y * sTheta + normal.X * normal.Z * cTheta ) * sqrtNcPhi ),
                (float)( cPhi * normal.Y + ( normal.X * sTheta - normal.Y * normal.Z * cTheta ) * sqrtNcPhi ),
                (float)( cPhi * normal.Z + ( 1 - normal.Z * normal.Z ) * cTheta * sqrtNcPhi ) );
        }


        readonly Random _random = new Random();

    }
}
