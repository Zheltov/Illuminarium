using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public interface ICamera
    {
        Point3D Position { get; set; }
        Point3D Target { get; set; }
        Ray GetTracingRay( int pixel_x_coord, int pixel_y_coord, int clientWidth, int clientHeight );
    }
}
