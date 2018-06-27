using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public interface IMaterial
    {
        IReflectanceMaterial Reflectance { get; set; }

        IMirrorMaterial Mirror { get; set; }
    }
}
