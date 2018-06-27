using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public interface ILight
    {
        Point3D Position { get; set; }

        LightRay RandomRay();

        Spectrum GetIlluminance( IRayTracer rayTracer, IMaterial material, Point3D point, Vector normal, Vector view );
        IList<Spectrum> GetIlluminance( IRayTracer rayTracer, IMaterial material, Point3D point, Vector normal, IList<Vector> view );

        Spectrum GetIlluminanceSourceSurface( Point3D point, Vector direction );
    }

    public class LightRay : Ray
    {
        public Spectrum Illuminance { get; set; }

        public LightRay( Point3D from, Point3D to )
            : base( from, to )
        {
            this.Illuminance = new Spectrum();
        }
        public LightRay( Point3D from, Point3D to, Spectrum illuminance )
            : base( from, to )
        {
            this.Illuminance = illuminance;
        }
        public LightRay( Point3D from, Vector direction )
            : base( from, direction )
        {
            this.Illuminance = new Spectrum();
        }
        public LightRay( Point3D from, Vector direction, Spectrum illuminance )
            : base( from, direction )
        {
            this.Illuminance = illuminance;
        }
    }
}
