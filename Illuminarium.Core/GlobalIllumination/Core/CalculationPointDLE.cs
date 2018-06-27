using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.GlobalIllumination.Core
{
    public class CalculationPointDLE : CalculationPoint
    {
        public int CounterRaysCos1Zero { get; set; }
        public int CounterRaysCos2Zero { get; set; }
        public int CounterOccluded { get; set; }
        public int CounterVisibleNormals { get; set; }

        public CalculationPointDLE( IFace face, Point3D position, Vector direction )
            : base(face, position, direction)
        {
            
        }

        public CalculationPointDLE( Intersection intersection, Vector direction ) 
            : base(intersection, direction)
        {
           
        }
    }
}
