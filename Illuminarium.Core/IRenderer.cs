using System;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Illuminarium.Core
{
    public interface IRenderer
    {
        ILog Log { get; set; }
        IRayTracer RayTracer { get; }
        Scene Scene { get; }
        IGlobalIllumination GlobalIllumination { get; set; }
        void RenderDirectIllumination( RenderPointsStructure renderPointsStructure );
        RenderPointsStructure RenderDirectIllumination( ICamera camera, int width, int height );
        void RenderGlobalIllumination( RenderPointsStructure renderPointsStructure );
        RenderPointsStructure RenderGlobalIllumination( ICamera camera, int width, int height );
        RenderPointsStructure GenerateRenderPoints( ICamera camera, int width, int height );

        void FinalGathering( RenderPointsStructure renderPointsStructure );
    }
}