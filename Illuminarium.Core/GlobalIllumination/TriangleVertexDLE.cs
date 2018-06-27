using Illuminarium.Core.GlobalIllumination.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.GlobalIllumination
{
    public class TriangleVertexDLE : CalculationPointDLE
    {
        public IList<Spectrum> IlluminancePacket { get; set; }
        //public int CounterRaysCos1Zero { get; set; }
        //public int CounterRaysCos2Zero { get; set; }
        //public int CounterRaysShadow { get; set; }

        public List<TriangleFaceDLE> GIFaces { get; set; }

        public TriangleVertexDLE( IFace face, Point3D position, Vector direction  )
            : base( face, position, direction )
        {
            this.Illuminance = new Spectrum();
            this.GIFaces = new List<TriangleFaceDLE>();
            this.IlluminancePacket = new List<Spectrum>();
        }

        public TriangleVertexDLE( IFace face, Point3D position, Vector direction, TriangleFaceDLE giFace )
            : this( face, position, direction )
        {
            this.GIFaces.Add( giFace );
        }

    }
}
