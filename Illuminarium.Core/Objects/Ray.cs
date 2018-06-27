using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public class Ray
    {
        public Point3D From { get; set; }
        public Point3D To {get;set;}
        public Vector Direction { get; set; }

        public Ray( Point3D from, Point3D to)
        {
            this.From = from;
            this.To = to;
            this.Direction = new Vector( from, to, true );
        }

        public Ray( Point3D from, Vector direction)
        {
            this.From = from;
            this.Direction = direction;
            this.To = from + direction;
        }
        
        public override string ToString()
        {
            return string.Format( "From = {0}, To = {1}", this.From.ToString(), this.To.ToString() );
        }
    }
}
