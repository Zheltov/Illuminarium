using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public interface IMirrorMaterial
    {
        Spectrum Reflectance { get; set; }
    }
}
