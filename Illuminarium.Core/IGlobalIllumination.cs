using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public interface IGlobalIllumination
    {
        ILog Log { get; set; }
        IRayTracer RayTracer { get; set; }
        void Calculate( RenderPointsStructure renderPointsStructure );
    }
}