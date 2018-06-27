using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Illuminarium.Core
{
    public interface IReflectanceMaterial
    {
        Spectrum BRDF( Vector fall, Vector normal, Vector view );

        Vector RandomReflectedDirection( Vector fall, Vector normal );
    }
}