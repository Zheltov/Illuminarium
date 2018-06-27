using Illuminarium.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Illuminarium.IO
{
    public interface ISceneLoader
    {
        IMaterial DefaultMaterial { get; set; }
        Scene LoadFromFile( string fileName );
    }
}
