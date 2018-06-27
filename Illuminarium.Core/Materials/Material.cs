using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public class Material : IMaterial
    {
        public IReflectanceMaterial Reflectance { get; set; }
        public IMirrorMaterial Mirror { get; set; }

        public Material( IReflectanceMaterial reflectance )
        {
            this.Reflectance = reflectance;
        }

        public Material( IMirrorMaterial mirror )
        {
            this.Mirror = mirror;
        }

        public Material( IReflectanceMaterial reflectance, IMirrorMaterial mirror )
        {
            this.Reflectance = reflectance;
            this.Mirror = mirror;
        }

    }
}
