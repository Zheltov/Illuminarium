using Illuminarium.Core;
using Illuminarium.Core.Lights;
using Illuminarium.Core.Materials;
namespace Illuminarium.TestScenes
{
    public class SceneCreatorCube : ISceneCreator
    {
        public Scene CreateScene()
        {
            var defaultMaterial = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.5f ) ) );

            IO.ISceneLoader loader = new IO.SceneLoader3ds( (float)1, defaultMaterial );

            var scene = loader.LoadFromFile( "TestScenes\\3ds\\cube.3ds" );

            scene.RecalcSceneParams();
            scene.CameraSetDefault();
            scene.Camera.Position = new Point3D( 0f, 2f, 2f );
            scene.Camera.Target = new Point3D( 0f, 0f, 0f );
            
            return scene;
        }
    }
}
