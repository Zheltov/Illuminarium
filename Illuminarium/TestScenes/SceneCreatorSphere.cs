using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Illuminarium.Core;
using Illuminarium.Core.Lights;
using Illuminarium.Core.Materials;

namespace Illuminarium.TestScenes
{
    public class SceneCreatorSphere : ISceneCreator
    {
        public Scene CreateScene()
        {
            var defaultMaterial = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.5f ) ) );

            Illuminarium.IO.ISceneLoader loader = new Illuminarium.IO.SceneLoader3ds( (float)1, defaultMaterial );
            var scene = loader.LoadFromFile( "TestScenes\\3ds\\sphere.3ds" );
            
            scene.RecalcSceneParams();
            scene.CameraSetDefault();
            //scene.Camera.Position = new Point3D( 0f, -6f, 2.5f );
            //scene.Camera.Target = new Point3D( 0f, 0f, 2.5f );

            // remove one wall for camera
            scene.Objects[0].Faces.RemoveAt( 4 );
            scene.Objects[0].Faces.RemoveAt( 4 );

            // lights
            scene.Lights.Clear();
            scene.Lights.Add( new RectangleLight( new Point3D( 0f, 0f, 0f ), 0.02f, 0.02f, new Spectrum( 1 ), defaultMaterial, 4 ) );

            
            return scene;
        }
    }
}
