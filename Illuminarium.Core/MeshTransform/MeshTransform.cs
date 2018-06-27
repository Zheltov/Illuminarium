using Illuminarium.Core.Debug;
using System;

namespace Illuminarium.Core.MeshTransform
{
    public static class MeshTransform
    {
        public static AngleFunc MeshToAngleFunc( IRayTracer rayTracer, Point3D point, int nMu, int nPhi )
        {
            var result = new AngleFunc( nMu, nPhi, true );
            var pi2 = Math.PI / 2;

            for ( int iMu = 0; iMu < result.Mu.Count; iMu++ )
                for ( int iPhi = 0; iPhi < nPhi; iPhi++ )
                {
                    var theta = (float)( pi2 - Math.Acos( result.Mu[iMu] ) );
                    var phi = result.Phi[iPhi];
                    Vector direction = new Vector( theta, phi );

                    var intersection = rayTracer.Trace( point, direction, Constants.Epsilon );

                    if ( intersection == null )
                    {
                        //RayDebugStaticCollection.Add( new Ray( point, direction ), System.Drawing.Color.Red );
                    }
                    else
                    {
                        var r = ( intersection.Point - point ).Length;
                        result.R[iMu][iPhi] = r;
                        //RayDebugStaticCollection.Add( new Ray( point, intersection.Point ), System.Drawing.Color.Green );
                    }
                }

            return result;
        }
    }
}
