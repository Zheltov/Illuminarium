using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminarium.Core
{
    public class Face : IFace
    {
        public IObj Obj { get; set; }
        public IMaterial Material { get; set; }
        public int[] VertexIndexes { get; set; }
        public Vector Normal { get; set; }

        public Face( IObj obj, int[] vertexIndexes )
        {
            Obj = obj;
            VertexIndexes = vertexIndexes;
            Init();
        }

        /// <summary>
        /// Пересчет всех параметров (Нормаль и т.п.)
        /// </summary>
        private void Init()
        {
            Point3D p1 = this.Obj.Vertices[this.VertexIndexes[1]] - this.Obj.Vertices[this.VertexIndexes[0]];
            Point3D p2 = this.Obj.Vertices[this.VertexIndexes[2]] - this.Obj.Vertices[this.VertexIndexes[0]];
            this.Normal = new Vector( Point3D.Cross( p1, p2 ), true );
        }

    }
}