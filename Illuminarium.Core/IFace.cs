using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public interface IFace
    {
        IObj Obj { get; set; }
        IMaterial Material { get; set; }
        int[] VertexIndexes { get; set; }
        Vector Normal { get; set; }
    }
}
