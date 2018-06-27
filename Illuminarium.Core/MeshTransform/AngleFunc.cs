using SphericalHarmonics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Illuminarium.Core.MeshTransform
{
    public class AngleFunc
    {
        public Color Color { get; set; }
        public IList<float> Mu { get; set; }
        public IList<float> GaussWeights { get; set; }
        public IList<float> Phi { get; set; }
        public float[][] R { get; set; }
        public Point3D Point { get; set; }
        public AngleFunc( int nMu, int nPhi, bool nMuHalf )
        {
            R = new float[nMu][];
            for ( int i = 0; i < nMu; i++ )
                R[i] = new float[nPhi];

            if ( !nMuHalf )
            {
                var legzoResult = SHMath.Legzo( nMu );
                Mu = legzoResult.Mu.Select( x => (float)x ).ToList();
                GaussWeights = legzoResult.Weight.Select( x => (float)x ).ToList();
            }
            else
            {
                var legzoResult = SHMath.Legzo( nMu * 2 );
                Mu = new List<float>();
                GaussWeights = new List<float>();
                for ( int i = 0; i < nMu; i++ )
                {
                    Mu.Add( (float)legzoResult.Mu[i] );
                    GaussWeights.Add( (float)legzoResult.Weight[i] );
                }
            }


            Phi = new List<float>();
            for ( int i = 0; i < nPhi; i++ )
                Phi.Add( (float)( i * 2 * Math.PI / ( (double)nPhi - 1 ) ) );
        }

    }
}
