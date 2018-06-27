namespace Illuminarium.Core
{
    public class RenderResultDeleteMe
    {
        public RenderPointsStructure RenderPointsStructure { get; set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int RayTraced { get; set; }

        public RenderResultDeleteMe( RenderPointsStructure renderPointsStructure, int width, int height )
        {
            this.Width = width;
            this.Height = height;
            this.RenderPointsStructure = renderPointsStructure;
        }

        public RenderPoint GetRenderPoint( int x, int y )
        {
            var index = y * this.Width + x;
            if ( this.RenderPointsStructure.RenderPointsByCoordinate[index] != null )
                return this.RenderPointsStructure.RenderPointsByCoordinate[index];
            else
                return null;
        }
    }
}