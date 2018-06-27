using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Illuminarium.Graph
{
    public class XYGraphInfo
    {
        public string Title { get; set; }
        public IList<float> X { get; set; }
        public IList<float> Y { get; set; }
        public Color Color { get; set; }

        public XYGraphInfo()
        {
            this.Color = Color.Black;
            this.X = new List<float>();
            this.Y = new List<float>();
        }

        public void Add( float x, float y)
        {
            this.X.Add( x );
            this.Y.Add( y );
        }
    }
}
