using SphericalHarmonics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.MeshViewIndependent
{
    public class MeshSettings
    {
        public MSpectrumAngles SpectrumAngles { get; set; }
        public int MaxDivideDeep { get; set; }
        public int NSH { get; set; }

        public MeshSettings( int maxDivideDeep, int nTheta, int nPhi, int nSH )
        {
            MaxDivideDeep = maxDivideDeep;
            NSH = nSH;

            var phi0 = -Math.PI;
            var phi = new List<float>();
            for ( int i = 0; i < nPhi; i++ )
                phi.Add( (float)( phi0 + ( i * 2 * Math.PI / ( (double)nPhi - 1 ) ) ) );

            /*
            var theta0 = 0;
            var theta = new List<float>();
            for ( int i = 0; i < nTheta; i++ )
                theta.Add( (float)( theta0 + i * ( Math.PI / 2 ) / ( (double)nTheta - 1 ) ) );
            */

            var theta = new List<float>();
            var weights = new List<double>();
            var legzoResult = SHMath.Legzo( nTheta * 2 );
            for( int i = 0; i < nTheta; i++ )
            {
                theta.Add( (float) Math.Acos( legzoResult.Mu[i] ) );
                weights.Add( legzoResult.Weight[i] );
            }


            SpectrumAngles = new MSpectrumAngles( theta, phi, weights );
        }
    }
}
