using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminarium.Core
{
    public class Obj : IObj
    {
        public string Name { get; set; }
        public IList<IFace> Faces { get; set; }
        public IList<Point3D> Vertices { get; set; } 

        public Obj(string name)
        {
            this.Name = name;
            this.Faces = new List<IFace>();
            this.Vertices = new List<Point3D>();
        }
    }
}