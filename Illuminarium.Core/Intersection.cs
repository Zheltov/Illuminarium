using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public class Intersection
    {
        public IObj Obj { get; set; }
        public IFace Face { get; set; }
        public float Distance { get; set; }
        public Point3D Point { get; set; }

        public Intersection( IObj obj, IFace face, float distance, Point3D point )
        {
            this.Obj = obj;
            this.Face = face;
            this.Distance = distance;
            this.Point = point;
        }
    }
}
