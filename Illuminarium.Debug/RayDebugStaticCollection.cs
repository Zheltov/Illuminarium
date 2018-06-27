using Illuminarium.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Illuminarium.Debug
{
    public static class RayDebugStaticCollection
    {
        public static IList<RayDebug> Rays = new List<RayDebug>(); 
        public static void Init()
        {
            Rays.Clear();
        }

        public static void Add( Ray ray, Color color )
        {
            Rays.Add( new RayDebug( ray, color ) );
        }
    }
}
