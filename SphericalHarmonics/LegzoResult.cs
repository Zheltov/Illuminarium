using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SphericalHarmonics
{
    public class LegzoResult
    {
        public IList<double> Mu { get; set; }
        public IList<double> Weight { get; set; }

        public LegzoResult( int n )
        {
            Mu = new List<double>();
            Weight = new List<double>();

            for ( int i = 0; i < n; i++ )
            {
                Mu.Add( 0 );
                Weight.Add( 0 );
            }
        }
    }
}