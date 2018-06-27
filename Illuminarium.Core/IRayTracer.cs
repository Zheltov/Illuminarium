using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public interface IRayTracer: IDisposable
    {
        Scene Scene { get; }
        Intersection Trace( Point3D from, Vector direction, float near = 0, float far = float.PositiveInfinity, float time = 0 );
        bool Occluded( Point3D from, Vector direction, float near = 0, float far = float.PositiveInfinity, float time = 0 );
    }
}
