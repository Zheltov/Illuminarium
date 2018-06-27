using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.Materials
{

    public class DiffuseReflectanceMaterial : IReflectanceMaterial
    {
        Random randStatic = new Random();

        public Spectrum Reflectance { get; set; }

        public DiffuseReflectanceMaterial( Spectrum reflectance )
        {
            this.Reflectance = reflectance;
        }

        public Spectrum BRDF( Vector fall, Vector normal, Vector view )
        {
            return this.Reflectance;
        }

        public Vector RandomReflectedDirection( Vector fall, Vector normal )
        {
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

            double sqrtD = Math.Sqrt( d );
            double cTheta = u / sqrtD;
            double sTheta = v / sqrtD;
            double sPhi = Math.Sqrt( this.randStatic.NextDouble() );
            double cPhi = Math.Sqrt( 1 - sPhi * sPhi );

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

    }
}
