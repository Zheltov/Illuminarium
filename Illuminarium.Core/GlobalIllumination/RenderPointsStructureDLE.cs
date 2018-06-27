using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.GlobalIllumination
{
    class RenderPointsStructureDLE
    {
        //public IDictionary<RenderPoint, IList<RenderPoint>> RenderPointsSimple { get; set; }

        public List<TriangleFaceDLE> RenderPointsFace { get; set; }
        public List<TriangleVertexDLE> RenderPointsVertexes { get; set; }

        public RenderPointsStructureDLE()
        {
            //this.RenderPointsSimple = new Dictionary<RenderPoint, IList<RenderPoint>>();
            this.RenderPointsFace = new List<TriangleFaceDLE>();
            this.RenderPointsVertexes = new List<TriangleVertexDLE>();
        }
    }
}
