using System.Collections.Generic;

namespace Illuminarium.Core
{
    public class RenderPointsStructure
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public IDictionary<IObj, IList<RenderPoint>> ObjRenderPoints { get; set; }
        public IDictionary<IFace, IList<RenderPoint>> FaceRenderPoints { get; set; }
        public IList<RenderPoint> RenderPoints { get; set; }
        public RenderPoint[] RenderPointsByCoordinate { get; set; }
        public Scene Scene { get; set; }

        public RenderPointsStructure( Scene scene, int width, int height )
        {
            this.Scene = scene;
            this.Width = width;
            this.Height = height;
            this.FaceRenderPoints = new Dictionary<IFace, IList<RenderPoint>>();
            this.ObjRenderPoints = new Dictionary<IObj, IList<RenderPoint>>();
            this.RenderPoints = new List<RenderPoint>();
            this.RenderPointsByCoordinate = new RenderPoint[width * height];
        }
        
        public void Add( RenderPoint renderPoint )
        {
            this.RenderPoints.Add( renderPoint );

            if ( this.ObjRenderPoints.ContainsKey( renderPoint.Obj ) )
                this.ObjRenderPoints[renderPoint.Obj].Add( renderPoint );
            else
                this.ObjRenderPoints.Add( renderPoint.Obj, new List<RenderPoint>() { renderPoint } );

            if ( this.FaceRenderPoints.ContainsKey( renderPoint.Face ) )
                this.FaceRenderPoints[renderPoint.Face].Add( renderPoint );
            else
                this.FaceRenderPoints.Add( renderPoint.Face, new List<RenderPoint>() { renderPoint } );

            if ( renderPoint.IsPrimaryPoint )
                this.RenderPointsByCoordinate[renderPoint.ScreenY * this.Width + renderPoint.ScreenX] = renderPoint;
        }

        public RenderPoint GetRenderPoint( int x, int y )
        {
            var index = y * this.Width + x;
            if ( this.RenderPointsByCoordinate[index] != null )
                return this.RenderPointsByCoordinate[index];
            else
                return null;
        }

    }
}