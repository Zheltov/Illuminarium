using Illuminarium.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Illuminarium.Debug
{
    [Serializable]
    public class RayDebug
    {
        public Ray Ray { get; set; }
        public Color Color { get; set; }

        public RayDebug( Ray ray, Color color )
        {
            this.Ray = ray;
            this.Color = color;
        }
    }
}
