using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.Materials
{
    public class MirrorMaterial : IMirrorMaterial
    {
        public Spectrum Reflectance { get; set; }

        public MirrorMaterial( Spectrum reflectance )
        {
            this.Reflectance = reflectance;
        }
    }
}
