using System;
using System.Collections.Generic;
using System.Text;

namespace Illuminarium.Core.Lights
{
    public class PointLight : LightBase, ILight
    {
        public Spectrum Intensity { get; set; }

        public PointLight()
        {
        }
        public PointLight( Point3D pos )
        {
            this.Position = pos;
        }

        public LightRay RandomRay()
        {
            throw new NotImplementedException();
        }

        public Spectrum GetIlluminance( IRayTracer rayTracer, IMaterial material, Point3D point, Vector normal, Vector view )
        {
             var toLight = this.Position - point;
             var toLightVector = new Vector( toLight, true );
             //var toLightRay = new Ray( point + normal * Constants.Epsilon, this.Position );

             if ( !rayTracer.Occluded( point + normal * Constants.Epsilon, toLightVector, 0, toLight.Length ) )
             {
                 return this.Intensity * material.Reflectance.BRDF( toLightVector, normal, view ) / toLight.Length2;
             }
             else
                 return new Spectrum();
        }

        public IList<Spectrum> GetIlluminance( IRayTracer rayTracer, IMaterial material, Point3D point, Vector normal, IList<Vector> view)
        {
            throw new NotImplementedException();
        }

        public Spectrum GetIlluminanceSourceSurface( Point3D point, Vector direction )
        {
            throw new NotImplementedException();
        }
    }
}