using SphericalHarmonics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.MeshViewIndependent
{
    public class MFaceIlluminanceAngles
    {
        public Vector Normal { get; set; }
        public IList<float> Theta { get; set; }
        public IList<float> Mu { get; set; }
        public IList<float> Phi { get; set; }
        public IList<double> GaussWeight { get; set; }
        public IList<Vector> Directions { get; set; }
        public IList<double[][]> Qkm { get; set; }
        public int NSH { get; set; }

        public MFaceIlluminanceAngles( Vector normal, IMaterial material, IList<float> theta, IList<float> mu, IList<float> phi, IList<double> gaussWeight, int nSH )
        {
            Normal = normal;
            Theta = theta;
            Mu = mu;
            Phi = phi;
            GaussWeight = gaussWeight;
            NSH = nSH;
            Qkm = new List<double[][]>();
            foreach ( var m in Mu )
                Qkm.Add( SHMath.Schmidt( m, nSH ) );

            Directions = new List<Vector>();

            foreach ( var t in theta )
                foreach ( var p in phi )
                {
                    Vector v = new Vector( t, p );

                    v = v.ChangeCoordinateSystem( normal );
                    Directions.Add( v );
                }


            //Тут создаем реальные направления
        }

        public Spectrum2DArraysOfDouble IlluminanceToArrays( IList<Spectrum> illuminance )
        {
            var result = new Spectrum2DArraysOfDouble( Theta.Count, Phi.Count );

            for ( int it = 0; it < Theta.Count; it++ )
                for ( int ip = 0; ip < Phi.Count; ip++ )
                {
                    var index = it * Phi.Count + ip;
                    result.R[it, ip] = illuminance[index].R;
                    result.G[it, ip] = illuminance[index].G;
                    result.B[it, ip] = illuminance[index].B;
                }

            return result;
        }
    }

    public class Spectrum2DArraysOfDouble
    {
        public double[,] R { get; set; }
        public double[,] G { get; set; }
        public double[,] B { get; set; }

        public Spectrum2DArraysOfDouble( int i, int j )
        {
            R = new double[i, j];
            G = new double[i, j];
            B = new double[i, j];
        }
    }
}
