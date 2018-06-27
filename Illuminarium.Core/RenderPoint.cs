namespace Illuminarium.Core
{
    public class CalculationPoint
    {
        public IObj Obj { get; set; }
        public IFace Face { get; set; }
        public Point3D Position { get; set; }       
        public Vector Direction { get; set; }
        public virtual Spectrum Illuminance { get; set; }

        public CalculationPoint( IFace face, Point3D position, Vector direction )
        {
            this.Position = position;
            this.Obj = face.Obj;
            this.Face = face;
            this.Direction = direction;
            this.Illuminance = new Spectrum();
        }

        public CalculationPoint( Intersection intersection, Vector direction )
        {
            this.Position = intersection.Point;
            this.Obj = intersection.Obj;
            this.Face = intersection.Face;
            this.Direction = direction;
            this.Illuminance = new Spectrum();
        }
    }

    public class RenderPoint : CalculationPoint
    {
        private static int _indexValue = 0;

        private static int GetIndex()
        {
            return _indexValue++;
        }

        public int Index { get; set; }
        public int ScreenX { get; set; }
        public int ScreenY { get; set; }
        public RenderPoint MirrorRenderPoint { get; set; }
        public bool IsPrimaryPoint { get; set; }

        public Spectrum IlluminanceDirect { get; set; }
        public Spectrum IlluminanceIndirect { get; set; }

        public new Spectrum Illuminance
        {
            get { return this.IlluminanceDirect + this.IlluminanceIndirect; }
        }
        

        public RenderPoint( int screenX, int screenY, Intersection intersection, Vector direction, bool isPrimaryPoint )
            : base( intersection, direction )
        {
            this.ScreenX = screenX;
            this.ScreenY = screenY;
            this.IsPrimaryPoint = isPrimaryPoint;
            this.IlluminanceDirect = new Spectrum();
            this.IlluminanceIndirect = new Spectrum();
            this.Index = RenderPoint.GetIndex();
        }
    }
}