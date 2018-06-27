using Illuminarium.Core;
using Illuminarium.Core.Lights;
using Illuminarium.Core.Materials;

namespace Illuminarium.TestScenes
{
    public class SceneCreatorCornellBox : ISceneCreator
    {
        public Scene CreateScene()
        {
            var defaultMaterial = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.5f ) ) );

            Illuminarium.IO.ISceneLoader loader = new Illuminarium.IO.SceneLoader3ds( (float)1, defaultMaterial );
            var scene = loader.LoadFromFile( "TestScenes\\3ds\\cornellbox.3ds" );

            scene.RecalcSceneParams();
            scene.CameraSetDefault();
            scene.Camera.Position = new Point3D( 0f, -6f, 2.5f );
            scene.Camera.Target = new Point3D( 0f, 0f, 2.5f );

            // remove one wall for camera
            scene.Objects[0].Faces.RemoveAt( 4 );
            scene.Objects[0].Faces.RemoveAt( 4 );

            //scene.Objects[0].Faces.RemoveAt( 8 );
            //scene.Objects[0].Faces.RemoveAt( 8 );


            // lights
            scene.Lights.Clear();
            scene.Lights.Add( new RectangleLight( new Point3D( 0f, 0f, 4.98f ), 1f, 1f, new Spectrum( 1 ), defaultMaterial, 128 ) );

            var floor = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.54f, 0.54f, 0.54f ) ) );
            //var floor = new Material( new PhongReflectanceMaterial( new Spectrum( 0.54f, 0.54f, 0.54f ), 16f, 1f ) );
            //var floor = new Material( new MirrorMaterial( new Spectrum( 0.9f, 0.9f, 0.9f ) ) );

            scene.Objects[0].Faces[0].Material = floor;
            scene.Objects[0].Faces[1].Material = floor;

            var ceiling = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.84f, 0.84f, 0.84f ) ) );
            //var ceiling = new Material( new PhongReflectanceMaterial( new Spectrum( 0.84f, 0.84f, 0.84f ), 128f, 1f ) );
            scene.Objects[0].Faces[2].Material = ceiling;
            scene.Objects[0].Faces[3].Material = ceiling;

            var rightWall = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.0f, 0.0f, 1.0f ) ) );
            //var rightWall = new Material( new PhongReflectanceMaterial( new Spectrum( 0.0f, 0.0f, 1.0f ), 64f, 1f ) );
            scene.Objects[0].Faces[4].Material = rightWall;
            scene.Objects[0].Faces[5].Material = rightWall;

            var backWall = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.84f, 0.84f, 0.84f ) ) );
            scene.Objects[0].Faces[6].Material = backWall;
            scene.Objects[0].Faces[7].Material = backWall;

            //var leftWall = new Material( new PhongReflectanceMaterial( new Spectrum( 1.0f, 1.0f, 1.0f ), 8f, 1f ) );
            var leftWall = new Material( new DiffuseReflectanceMaterial( new Spectrum( 1.0f, 0.0f, 0.0f ) ) );
            scene.Objects[0].Faces[8].Material = leftWall;
            scene.Objects[0].Faces[9].Material = leftWall;

            //var box1 = new Material( new MirrorMaterial( new Spectrum( 0.95f, 0.95f, 0.95f ) ) );
            //var box1 = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.9f, 0.9f, 0.9f ) ) );
            var box1 = new Material( new PhongReflectanceMaterial( new Spectrum( 0.8f, 0.8f, 0.8f ), 16f, 0.5f ) );
            foreach ( var face in scene.Objects[1].Faces )
                face.Material = box1;

            //scene.Objects[1].Faces[10].Material = box1Mirror;
            //scene.Objects[1].Faces[11].Material = box1Mirror;


            //var box2 = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.8f, 0.8f, 0.8f ) ) );
            //var box2 = new Material( new MirrorMaterial( new Spectrum( 0.95f, 0.95f, 0.95f ) ) );
            var box2 = new Material( new PhongReflectanceMaterial( new Spectrum( 0.8f, 0.0f, 0.0f ), 64f, 0.5f ) );
            foreach ( var face in scene.Objects[2].Faces )
                face.Material = box2;

            /*
            scene.Objects.RemoveAt( 2 );
            scene.Objects.RemoveAt( 1 );
            scene.Objects[0].Faces.RemoveAt( 8 );
            scene.Objects[0].Faces.RemoveAt( 8 );
            scene.Objects[0].Faces.RemoveAt( 0 );
            scene.Objects[0].Faces.RemoveAt( 0 );
            scene.Objects[0].Faces.RemoveAt( 0 );
            scene.Objects[0].Faces.RemoveAt( 0 );
            scene.Objects[0].Faces.RemoveAt( 0 );
            scene.Objects[0].Faces.RemoveAt( 0 );

            */
            /*
            scene.Objects.RemoveAt( 2 );
            scene.Objects[1].Faces.RemoveAt( 4 );
            scene.Objects[1].Faces.RemoveAt( 4 );
            scene.Objects[1].Faces.RemoveAt( 4 );
            scene.Objects[1].Faces.RemoveAt( 4 );
            scene.Objects[1].Faces.RemoveAt( 4 );
            scene.Objects[1].Faces.RemoveAt( 4 );
            scene.Objects[1].Faces.RemoveAt( 4 );
            scene.Objects[1].Faces.RemoveAt( 4 );
            scene.Objects[1].Faces.RemoveAt( 0 );
            scene.Objects[1].Faces.RemoveAt( 0 );
            scene.Objects.RemoveAt( 0 );
            */
            /*
            scene.Objects[0].Faces.RemoveAt( 4 );
            scene.Objects[0].Faces.RemoveAt( 4 );
            scene.Objects[0].Faces.RemoveAt( 4 );
            scene.Objects[0].Faces.RemoveAt( 4 );
            scene.Objects[0].Faces.RemoveAt( 4 );
            scene.Objects[0].Faces.RemoveAt( 4 );
            scene.Objects[0].Faces.RemoveAt( 2 );
            scene.Objects[0].Faces.RemoveAt( 2 );
            */


            return scene;
        }
    }
}
