using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core.GlobalIllumination
{
    public class TriangleFaceDLE
    {
        public TriangleVertexDLE[] Vertexes { get; set; }

        public TriangleVertexDLE Center { get; set; }
        public IList<RenderPoint> RenderPoints { get; set; }
        public IFace Face { get; set; }
        public TriangleFaceDLE( IFace face, TriangleVertexDLE[] vertexes, Vector normal )
        {
            this.Vertexes = vertexes;
            this.RenderPoints = new List<RenderPoint>();
            this.Face = face;
        }

        public TriangleFaceDLE( IFace face, IList<RenderPoint> renderPoints )
        {
            // TODO: в вершины добавляем направления на которые производится рендеринг как направления ближайших точек рендеринга
            var p0 = GetNearestRenderPoint(renderPoints, face.Obj.Vertices[face.VertexIndexes[0]]);
            var p1 = GetNearestRenderPoint(renderPoints, face.Obj.Vertices[face.VertexIndexes[1]]);
            var p2 = GetNearestRenderPoint(renderPoints, face.Obj.Vertices[face.VertexIndexes[2]]);



            var vertexes = new Point3D[]
            {
                face.Obj.Vertices[face.VertexIndexes[0]],
                face.Obj.Vertices[face.VertexIndexes[1]],
                face.Obj.Vertices[face.VertexIndexes[2]],
            };

            var directions = new Vector[]
            {
                p0.Direction,
                p1.Direction,
                p2.Direction
            };


            this.Vertexes = new TriangleVertexDLE[3];
            this.Vertexes[0] = new TriangleVertexDLE( face, vertexes[0], directions[0], this );
            this.Vertexes[1] = new TriangleVertexDLE( face, vertexes[1], directions[1], this );
            this.Vertexes[2] = new TriangleVertexDLE( face, vertexes[2], directions[2], this );
            this.Face = face;
            this.RenderPoints = renderPoints;
        }

        public bool IsPointInTriangleFace( RenderPoint point )
        {
            var v0 = this.Vertexes[0].Position;
            var v1 = this.Vertexes[1].Position;
            var v2 = this.Vertexes[2].Position;

            if ( this.Face.Normal.Z != 0 )
            {
                var q1 = Q( v0.X, v0.Y, v1.X, v1.Y, point.Position.X, point.Position.Y );
                var q2 = Q( v1.X, v1.Y, v2.X, v2.Y, point.Position.X, point.Position.Y );
                var q3 = Q( v2.X, v2.Y, v0.X, v0.Y, point.Position.X, point.Position.Y );
                return ( ( q1 >= 0 ) && ( q2 >= 0 ) && ( q3 >= 0 ) ) || ( ( q1 < 0 ) && ( q2 < 0 ) && ( q3 < 0 ) );
            }
            else if ( this.Face.Normal.X != 0 )
            {
                var q1 = Q( v0.Z, v0.Y, v1.Z, v1.Y, point.Position.Z, point.Position.Y );
                var q2 = Q( v1.Z, v1.Y, v2.Z, v2.Y, point.Position.Z, point.Position.Y );
                var q3 = Q( v2.Z, v2.Y, v0.Z, v0.Y, point.Position.Z, point.Position.Y );
                return ( ( q1 >= 0 ) && ( q2 >= 0 ) && ( q3 >= 0 ) ) || ( ( q1 < 0 ) && ( q2 < 0 ) && ( q3 < 0 ) );
            }
            else
            {
                var q1 = Q( v0.X, v0.Z, v1.X, v1.Z, point.Position.X, point.Position.Z );
                var q2 = Q( v1.X, v1.Z, v2.X, v2.Z, point.Position.X, point.Position.Z );
                var q3 = Q( v2.X, v2.Z, v0.X, v0.Z, point.Position.X, point.Position.Z );
                return ( ( q1 >= 0 ) && ( q2 >= 0 ) && ( q3 >= 0 ) ) || ( ( q1 < 0 ) && ( q2 < 0 ) && ( q3 < 0 ) );
            }
        }

        public double[] GetBarycentricCoordinates( RenderPoint point )
        {
            var l01 = ( this.Vertexes[0].Position - this.Vertexes[1].Position ).Length;
            var l12 = ( this.Vertexes[1].Position - this.Vertexes[2].Position ).Length;
            var l20 = ( this.Vertexes[2].Position - this.Vertexes[0].Position ).Length;

            var lp0 = ( point.Position - this.Vertexes[0].Position ).Length;
            var lp1 = ( point.Position - this.Vertexes[1].Position ).Length;
            var lp2 = ( point.Position - this.Vertexes[2].Position ).Length;

            var area = Math3D.TriangleAreaByGeron( l01, l12, l20 );

            var a01 = Math3D.TriangleAreaByGeron( l01, lp0, lp1 ) / area;
            var a12 = Math3D.TriangleAreaByGeron( l12, lp1, lp2 ) / area;

            return new double[] { a01, a12, 1 - a01 - a12 };
        }

        public double[] GetBarycentricCoordinates_Old( RenderPoint point )
        {
            var v0 = this.Vertexes[0].Position;
            var v1 = this.Vertexes[1].Position;
            var v2 = this.Vertexes[2].Position;

            var result = new double[3];

            if ( Math.Abs( this.Face.Normal.Z ) > Constants.Epsilon )
            {
                var area = Square( v0.X, v0.Y, v1.X, v1.Y, v2.X, v2.Y );
                result[0] = Square( point.Position.X, point.Position.Y, v0.X, v0.Y, v1.X, v1.Y ) / area;
                result[1] = Square( point.Position.X, point.Position.Y, v1.X, v1.Y, v2.X, v2.Y ) / area;
                result[2] = 1 - result[0] - result[1];
                return result;
            }
            else if ( Math.Abs( this.Face.Normal.X ) > Constants.Epsilon )
            {
                var area = Square( v0.Z, v0.Y, v1.Z, v1.Y, v2.Z, v2.Y );
                result[0] = Square( point.Position.Z, point.Position.Y, v0.Z, v0.Y, v1.Z, v1.Y ) / area;
                result[1] = Square( point.Position.Z, point.Position.Y, v1.Z, v1.Y, v2.Z, v2.Y ) / area;
                result[2] = 1 - result[0] - result[1];
                return result;
            }
            else
            {
                var area = Square( v0.X, v0.Z, v1.X, v1.Z, v2.X, v2.Z );
                result[0] = Square( point.Position.X, point.Position.Z, v0.X, v0.Z, v1.X, v1.Z ) / area;
                result[1] = Square( point.Position.X, point.Position.Z, v1.X, v1.Z, v2.X, v2.Z ) / area;
                result[2] = 1 - result[0] - result[1];
                return result;
            }
        }

        public bool IsPointInTriangleFace2( RenderPoint point )
        {
            var v0 = this.Vertexes[0].Position;
            var v1 = this.Vertexes[1].Position;
            var v2 = this.Vertexes[2].Position;

            if ( Math.Abs( this.Face.Normal.Z ) > Constants.Epsilon )
            {
                var s = Square( point.Position.X, point.Position.Y, v0.X, v0.Y, v1.X, v1.Y ) +
                        Square( point.Position.X, point.Position.Y, v1.X, v1.Y, v2.X, v2.Y ) +
                        Square( point.Position.X, point.Position.Y, v2.X, v2.Y, v0.X, v0.Y );
                return Math.Abs( Square( v0.X, v0.Y, v1.X, v1.Y, v2.X, v2.Y ) - s ) <= Constants.EpsilonDouble;
            }
            else if ( Math.Abs( this.Face.Normal.X ) > Constants.Epsilon )
            {
                var s = Square( point.Position.Z, point.Position.Y, v0.Z, v0.Y, v1.Z, v1.Y ) +
                        Square( point.Position.Z, point.Position.Y, v1.Z, v1.Y, v2.Z, v2.Y ) +
                        Square( point.Position.Z, point.Position.Y, v2.Z, v2.Y, v0.Z, v0.Y );
                return Math.Abs( Square( v0.Z, v0.Y, v1.Z, v1.Y, v2.Z, v2.Y ) - s ) <= Constants.EpsilonDouble;
            }
            else
            {
                var s = Square( point.Position.X, point.Position.Z, v0.X, v0.Z, v1.X, v1.Z ) +
                        Square( point.Position.X, point.Position.Z, v1.X, v1.Z, v2.X, v2.Z ) +
                        Square( point.Position.X, point.Position.Z, v2.X, v2.Z, v0.X, v0.Z );
                return Math.Abs( Square( v0.X, v0.Z, v1.X, v1.Z, v2.X, v2.Z ) - s ) <= Constants.EpsilonDouble;
            }
        }

        public static RenderPoint GetNearestRenderPoint( IEnumerable<RenderPoint> renderPoints, Point3D point )
        {
            RenderPoint result = null;
            var length2 = float.MaxValue;
            foreach ( var renderPoint in renderPoints )
            {
                var tmp = ( renderPoint.Position - point ).Length2;
                if ( tmp < length2 )
                {
                    length2 = tmp;
                    result = renderPoint;
                }
            }

            return result;
        }

        static double Square( double ax1, double ay1, double ax2, double ay2, double ax3, double ay3 )
        {
            return Math.Abs( ax2 * ay3 - ax3 * ay2 - ax1 * ay3 + ax3 * ay1 + ax1 * ay2 - ax2 * ay1 );
        }

        static double Q( double aAx, double aAy, double aBx, double aBy, double atx, double aty )
        {
            return atx * ( aBy - aAy ) + aty * ( aAx - aBx ) + aAy * aBx - aAx * aBy;
        }
    }
}
