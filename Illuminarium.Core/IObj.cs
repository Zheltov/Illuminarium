using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public interface IObj
    {
        string Name { get; set; }
        IList<IFace> Faces { get; set; }
        IList<Point3D> Vertices { get; set; } 
    }
}
