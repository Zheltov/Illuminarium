using System;
using System.Collections.Generic;

namespace SphericalHarmonics
{
    public static class SHMath
    {
        public static LegzoResult Legzo( int nMu )
        {
            var result = new LegzoResult( nMu );

            double n0 = ( (double)nMu + 1 ) / 2;
            double hn = 1.0 - 1.0 / (double)nMu;

            for ( double nr = 1; nr < n0; nr++ )
            {
                int nrIndex = (int)nr - 1;

                double z = Math.Cos( Math.PI * ( nr - 0.5 ) / nMu );
                double pd = 0;
                while ( true )
                {
                    double z0 = z;
                    double f0 = 1.0;

                    double fixn2 = Math.Floor( (double)nMu / 2 );
                    if ( nr == n0 && nMu != 2 * fixn2 )
                    {
                        z = 0.0;
                    }

                    double f1 = z;
                    double pf = 0;
                    for ( int k = 2; k <= nMu - 1; k++ )
                    {
                        double h = 1.0 - 1.0 / k;
                        pf = ( 1 + h ) * z * f1 - h * f0;
                        f0 = f1;
                        f1 = pf;
                    }

                    pf = ( 1 + hn ) * z * f1 - hn * f0;
                    pd = nMu * ( f1 - z * pf ) / ( 1.0 - z * z );
                    if ( z == 0.0 )
                    {
                        break;
                    }

                    double sum1p = 0;
                    for ( int index = 1; index <= nr - 1; index++ )
                    {
                        int index1 = index - 1;
                        double p = z - result.Mu[index1];
                        sum1p += 1.0 / p;
                    }

                    z = z - pf / ( pd - sum1p * pf );
                    if ( Math.Abs( z - z0 ) < Math.Abs( z ) * 1.0e-12 )
                    {
                        break;
                    }
                }

                int n1 = nMu - 1;
                result.Mu[nrIndex] = z;
                result.Mu[n1 - nrIndex] = -z;
                result.Weight[nrIndex] = 2.0 / ( ( 1.0 - z * z ) * pd * pd );
                result.Weight[n1 - nrIndex] = result.Weight[nrIndex];
            }

            return result;
        }
        public static double[][] Schmidt( double mu, int n )
        {
            var Qkm = new double[n][];
            for ( int i = 0; i < n; i++ )
                Qkm[i] = new double[n];

            double mu2 = Math.Pow( mu, 2 );
            double smu = Math.Sqrt( 1 - mu2 );

            Qkm[0][0] = 1;

            for ( int m = 0; m < n; m++ )
            {
                int m1 = m + 1;
                double m2 = Math.Pow( m, 2 );

                if ( m > 0 )
                {
                    double somx2 = Math.Pow( smu, m );
                    double prod = 1;
                    for ( int l = 2; l <= 2 * m; l = l + 2 )
                    {
                        prod *= 1.0 - 1.0 / (double)l;
                    }
                    double val = Math.Sqrt( prod ) * somx2;
                    Qkm[m][m] = val;
                }
                if ( m1 < n )
                {
                    double val = Math.Sqrt( 2 * m + 1 ) * mu * Qkm[m][m];
                    Qkm[m + 1][m] = val;
                }

                for ( int k = ( m + 2 ); k < n; k++ )
                {
                    double val = ( ( 2 * k - 1 ) * mu * Qkm[k - 1][m] - Math.Sqrt( Math.Pow( k - 1, 2 ) - m2 ) * Qkm[k - 2][m] ) / Math.Sqrt( Math.Pow( k, 2 ) - m2 );
                    Qkm[k][m] = val;
                }
            }



            return Qkm;


        }
        public static SHResult AFToSH( float[][] r, IList<float> mu, IList<float> phi, int n, IList<double> gaussWeights = null, IList<double[][]> qkm = null )
        {
            // mu задано в нулях полиномов Лежандра 

            if ( gaussWeights == null )
                gaussWeights = Legzo( mu.Count ).Weight;

            if ( qkm == null )
            {
                qkm = new List<double[][]>();
                foreach ( var m in mu )
                    qkm.Add( Schmidt( m, n ) );
            }

            var dphi = (float)( 2 * Math.PI / ( phi.Count - 1 ) );

            var result = new SHResult( n );

            for ( int k = 0; k < n; k++ )
            {
                float ksi = 1;
                float k2 = 2 * k + 1;

                for ( int m = 0; m < k + 1; m++ )
                {
                    float a = 0;
                    float b = 0;

                    for ( int imu = 0; imu < mu.Count; imu++ )
                    {
                        var w = (float)gaussWeights[imu];

                        for ( int iphi = 0; iphi < phi.Count; iphi++ )
                        {
                            float wi = ( iphi == 0 || iphi == phi.Count - 1 ) ? 0.5f : 1f;

                            a = a + ksi * (float)Math.Cos( m * phi[iphi] ) * r[imu][iphi] * (float)qkm[imu][k][m] * k2 * wi * w;
                            b = b + ksi * (float)Math.Sin( m * phi[iphi] ) * r[imu][iphi] * (float)qkm[imu][k][m] * k2 * wi * w;

                        }
                    }

                    result.A[k][m] = a * dphi * 0.25f / (float)Math.PI;
                    result.B[k][m] = b * dphi * 0.25f / (float)Math.PI;
                    if ( m == 0 )
                        ksi = ksi * 2;

                }
            }

            return result;
        }
        public static SHResult AFToSH( double[,] r, IList<float> mu, IList<float> phi, int n, IList<double> gaussWeights = null, IList<double[][]> qkm = null )
        {
            // mu задано в нулях полиномов Лежандра 

            if ( gaussWeights == null )
                gaussWeights = Legzo( mu.Count ).Weight;

            if ( qkm == null )
            {
                qkm = new List<double[][]>();
                foreach ( var m in mu )
                    qkm.Add( Schmidt( m, n ) );
            }

            var dphi = (float)( 2 * Math.PI / ( phi.Count - 1 ) );

            var result = new SHResult( n );

            for ( int k = 0; k < n; k++ )
            {
                float ksi = 1;
                float k2 = 2 * k + 1;

                for ( int m = 0; m < k + 1; m++ )
                {
                    float a = 0;
                    float b = 0;

                    for ( int imu = 0; imu < mu.Count; imu++ )
                    {
                        var w = (float)gaussWeights[imu];

                        for ( int iphi = 0; iphi < phi.Count; iphi++ )
                        {
                            float wi = ( iphi == 0 || iphi == phi.Count - 1 ) ? 0.5f : 1f;

                            a = a + ksi * (float)( Math.Cos( m * phi[iphi] ) * r[imu, iphi] * qkm[imu][k][m] * k2 * wi * w );
                            b = b + ksi * (float)( Math.Sin( m * phi[iphi] ) * r[imu, iphi] * qkm[imu][k][m] * k2 * wi * w );

                        }
                    }

                    result.A[k][m] = a * dphi * 0.25f / (float)Math.PI;
                    result.B[k][m] = b * dphi * 0.25f / (float)Math.PI;
                    if ( m == 0 )
                        ksi = ksi * 2;

                }
            }

            return result;
        }

        public static float[][] SHToAF( SHResult sh, IList<float> mu, IList<float> phi, IList<double[][]> qkm = null )
        {
            var result = new float[mu.Count][];
            for ( int i = 0; i < mu.Count; i++ )
                result[i] = new float[phi.Count];

            if ( qkm == null )
            {
                qkm = new List<double[][]>();
                foreach ( var m in mu )
                    qkm.Add( Schmidt( m, sh.N ) );
            }

            for ( int imu = 0; imu < mu.Count; imu++ )
                for ( int iphi = 0; iphi < phi.Count; iphi++ )
                {

                    double r = 0;
                    for ( int k = 0; k < sh.N; k++ )
                        for ( int m = 0; m < k + 1; m++ )
                            r += qkm[imu][k][m] * ( sh.A[k][m] * Math.Cos( m * phi[iphi] ) + sh.B[k][m] * Math.Sin( m * phi[iphi] ) );

                    result[imu][iphi] = (float)r;

                }

            return result;

        }

        public static float SHToAF( SHResult sh, float mu, float phi, double[][] qkm = null )
        {
            /*
            var result = new float[mu.Count][];
            for ( int i = 0; i < mu.Count; i++ )
                result[i] = new float[phi.Count];
                */
            if ( qkm == null )
                qkm = Schmidt( mu, sh.N );

            double r = 0;
            for ( int k = 0; k < sh.N; k++ )
                for ( int m = 0; m < k + 1; m++ )
                    r += qkm[k][m] * ( sh.A[k][m] * Math.Cos( m * phi ) + sh.B[k][m] * Math.Sin( m * phi ) );

            return (float)r;

        }
    }

    public class SHResult
    {
        public int N { get; private set; }
        public float[][] A { get; set; }
        public float[][] B { get; set; }

        public SHResult( int n )
        {
            N = n;
            A = new float[n][];
            B = new float[n][];
            for ( int i = 0; i < n; i++ )
            {
                A[i] = new float[n];
                B[i] = new float[n];
            }
        }
    }
}