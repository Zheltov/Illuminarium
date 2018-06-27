//using alglibnet2;
using Illuminarium.Core.Materials;
using Illuminarium.Core.MeshViewIndependent;
using SphericalHarmonics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.MeshViewIndependent
{
    public class MVertex
    {
        alglib.spline2dinterpolant directIlluminanceSplineCoeffsR;
        alglib.spline2dinterpolant directIlluminanceSplineCoeffsG;
        alglib.spline2dinterpolant directIlluminanceSplineCoeffsB;

        alglib.spline2dinterpolant indirectIlluminanceSplineCoeffsR;
        alglib.spline2dinterpolant indirectIlluminanceSplineCoeffsG;
        alglib.spline2dinterpolant indirectIlluminanceSplineCoeffsB;

        SHResult directIlluminanceSHR;
        SHResult directIlluminanceSHG;
        SHResult directIlluminanceSHB;

        SHResult indirectIlluminanceSHR;
        SHResult indirectIlluminanceSHG;
        SHResult indirectIlluminanceSHB;


        public Point3D Point { get; set; }
        public IList<Spectrum> IlluminanceDirect { get; private set; }
        public IList<Spectrum> IlluminanceIndirect { get; private set; }
        public MFaceIlluminanceAngles IlluminanceAngles { get; private set; }
        public IFace Face { get; set; }

        public MVertex( IFace face, Point3D point, MFaceIlluminanceAngles illuminanceAngles )
        {
            if ( illuminanceAngles == null )
                throw new NotImplementedException();
            Face = face;
            Point = point;
            IlluminanceAngles = illuminanceAngles;

            IlluminanceDirect = new List<Spectrum>();
            IlluminanceIndirect = new List<Spectrum>();
            foreach ( var illAng in illuminanceAngles.Directions )
            {
                IlluminanceDirect.Add( new Spectrum() );
                IlluminanceIndirect.Add( new Spectrum() );
            }

        }

        public Spectrum GetIlluminance( Point3D point, MVertexIlluminanceMode mode, MVertexIlluminanceApproximationMode approximationMode )
        {
            return GetIlluminance( new Vector( Point, point, true ), mode, approximationMode );
        }

        public Spectrum GetIlluminance( Vector direction, MVertexIlluminanceMode mode, MVertexIlluminanceApproximationMode approximationMode )
        {
            Spectrum result = new Spectrum( 0 );
            if ( Face.Material.Reflectance is DiffuseReflectanceMaterial )
            {
                if ( mode == MVertexIlluminanceMode.Full || mode == MVertexIlluminanceMode.Direct )
                    result += IlluminanceDirect[0];

                if ( mode == MVertexIlluminanceMode.Full || mode == MVertexIlluminanceMode.Indirect )
                    result += IlluminanceIndirect[0];
            }
            else
            {
                switch ( approximationMode )
                {
                    case MVertexIlluminanceApproximationMode.Spline:
                        result = GetIlluminanceBySpline( direction, mode );
                        break;
                    case MVertexIlluminanceApproximationMode.SphericalHarmonics:
                        result = GetIlluminanceBySphericalHarmonics( direction, mode );
                        break;
                    default:
                        throw new NotImplementedException();

                }
            }

            /*
            if ( result.R < 0 )
            {
                //return new Spectrum();
                return new Spectrum( 1, 0, 0 );
            }
            */

            return result;

        }



        Spectrum GetIlluminanceNearest( Vector direction )
        {
            float maxDot = 0;
            Spectrum result = new Spectrum();

            for ( int i = 0; i < IlluminanceAngles.Directions.Count; i++ )
            {
                var va = Vector.Dot( IlluminanceAngles.Directions[i], direction );
                if ( va < 0 )
                    continue;
                va = Math.Abs( va );

                if ( va > maxDot )
                {
                    maxDot = va;
                    result = IlluminanceDirect[i];
                }
            }

            return result;
        }

        Spectrum GetIlluminanceBySpline( Vector direction, MVertexIlluminanceMode mode )
        {
            var directionProjected = Face.Normal.Projected( direction );
            directionProjected.Normalize();

            var sphCoord = directionProjected.ToSphCoord();

            //sphCoord.Theta += Math.PI / 2;

            double valR = 0;
            double valG = 0;
            double valB = 0;

            if ( mode == MVertexIlluminanceMode.Full || mode == MVertexIlluminanceMode.Direct )
            {
                valR += alglib.spline2dcalc( directIlluminanceSplineCoeffsR, sphCoord.Phi, sphCoord.Theta );
                valG += alglib.spline2dcalc( directIlluminanceSplineCoeffsG, sphCoord.Phi, sphCoord.Theta );
                valB += alglib.spline2dcalc( directIlluminanceSplineCoeffsB, sphCoord.Phi, sphCoord.Theta );
            }

            if ( mode == MVertexIlluminanceMode.Full || mode == MVertexIlluminanceMode.Indirect )
            {
                valR += alglib.spline2dcalc( indirectIlluminanceSplineCoeffsR, sphCoord.Phi, sphCoord.Theta );
                valG += alglib.spline2dcalc( indirectIlluminanceSplineCoeffsG, sphCoord.Phi, sphCoord.Theta );
                valB += alglib.spline2dcalc( indirectIlluminanceSplineCoeffsB, sphCoord.Phi, sphCoord.Theta );
            }

            return new Spectrum( (float)valR, (float)valG, (float)valB );
        }
        Spectrum GetIlluminanceBySphericalHarmonics( Vector direction, MVertexIlluminanceMode mode )
        {
            var directionProjected = Face.Normal.Projected( direction );
            var sphCoord = directionProjected.ToSphCoord();

            double valR = 0;
            double valG = 0;
            double valB = 0;

            var mu = (float)Math.Cos( sphCoord.Theta );
            var phi = (float)sphCoord.Phi;
            var qkm = SHMath.Schmidt( mu, IlluminanceAngles.NSH );

            if ( mode == MVertexIlluminanceMode.Full || mode == MVertexIlluminanceMode.Direct )
            {
                valR += SHMath.SHToAF( directIlluminanceSHR, mu, phi, qkm );
                valG += SHMath.SHToAF( directIlluminanceSHG, mu, phi, qkm );
                valB += SHMath.SHToAF( directIlluminanceSHB, mu, phi, qkm );
            }

            if ( mode == MVertexIlluminanceMode.Full || mode == MVertexIlluminanceMode.Indirect )
            {
                valR += SHMath.SHToAF( indirectIlluminanceSHR, mu, phi, qkm );
                valG += SHMath.SHToAF( indirectIlluminanceSHG, mu, phi, qkm );
                valB += SHMath.SHToAF( indirectIlluminanceSHB, mu, phi, qkm );
            }

            return new Spectrum( (float)valR, (float)valG, (float)valB );
        }

        public void UpdateStatistics()
        {
            var directIlluminanceArrays = IlluminanceAngles.IlluminanceToArrays( IlluminanceDirect );
            var indirectIlluminanceArrays = IlluminanceAngles.IlluminanceToArrays( IlluminanceIndirect );

            var arrT = IlluminanceAngles.Theta.Select( x => (double)x ).ToArray();
            var arrP = IlluminanceAngles.Phi.Select( x => (double)x ).ToArray();

            alglib.spline2dbuildbicubic( arrP, arrT, directIlluminanceArrays.R, arrT.Length, arrP.Length, out directIlluminanceSplineCoeffsR );
            alglib.spline2dbuildbicubic( arrP, arrT, directIlluminanceArrays.G, arrT.Length, arrP.Length, out directIlluminanceSplineCoeffsG );
            alglib.spline2dbuildbicubic( arrP, arrT, directIlluminanceArrays.B, arrT.Length, arrP.Length, out directIlluminanceSplineCoeffsB );

            alglib.spline2dbuildbicubic( arrP, arrT, indirectIlluminanceArrays.R, arrT.Length, arrP.Length, out indirectIlluminanceSplineCoeffsR );
            alglib.spline2dbuildbicubic( arrP, arrT, indirectIlluminanceArrays.G, arrT.Length, arrP.Length, out indirectIlluminanceSplineCoeffsG );
            alglib.spline2dbuildbicubic( arrP, arrT, indirectIlluminanceArrays.B, arrT.Length, arrP.Length, out indirectIlluminanceSplineCoeffsB );


            directIlluminanceSHR = SHMath.AFToSH( directIlluminanceArrays.R, IlluminanceAngles.Mu, IlluminanceAngles.Phi, IlluminanceAngles.NSH, IlluminanceAngles.GaussWeight, IlluminanceAngles.Qkm );
            directIlluminanceSHG = SHMath.AFToSH( directIlluminanceArrays.G, IlluminanceAngles.Mu, IlluminanceAngles.Phi, IlluminanceAngles.NSH, IlluminanceAngles.GaussWeight, IlluminanceAngles.Qkm );
            directIlluminanceSHB = SHMath.AFToSH( directIlluminanceArrays.B, IlluminanceAngles.Mu, IlluminanceAngles.Phi, IlluminanceAngles.NSH, IlluminanceAngles.GaussWeight, IlluminanceAngles.Qkm );

            indirectIlluminanceSHR = SHMath.AFToSH( indirectIlluminanceArrays.R, IlluminanceAngles.Mu, IlluminanceAngles.Phi, IlluminanceAngles.NSH, IlluminanceAngles.GaussWeight, IlluminanceAngles.Qkm );
            indirectIlluminanceSHG = SHMath.AFToSH( indirectIlluminanceArrays.G, IlluminanceAngles.Mu, IlluminanceAngles.Phi, IlluminanceAngles.NSH, IlluminanceAngles.GaussWeight, IlluminanceAngles.Qkm );
            indirectIlluminanceSHB = SHMath.AFToSH( indirectIlluminanceArrays.B, IlluminanceAngles.Mu, IlluminanceAngles.Phi, IlluminanceAngles.NSH, IlluminanceAngles.GaussWeight, IlluminanceAngles.Qkm );

            /*
            for ( int it = 0; it < arrT.Length; it++ )
                for ( int ip = 0; ip < arrP.Length; ip++ )
                {
                    var val = alglib.spline2dcalc( directIlluminanceSplineCoeffsR, arrP[ip], arrT[it] );
                    if ( val > 0.00001 )
                    {
                        var error = directIlluminanceArrays.R[it, ip] - val;
                        if ( Math.Abs( error ) > 0.000001 )
                            throw new Exception( "aaa" );
                    }
                }
            */
        }
    }

    public enum MVertexIlluminanceMode
    {
        Full,
        Direct,
        Indirect
    }

    public enum MVertexIlluminanceApproximationMode
    {
        Spline,
        SphericalHarmonics
    }
}
