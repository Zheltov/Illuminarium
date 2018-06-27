using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Illuminarium.Core;
using Illuminarium.Core.Lights;
using Illuminarium.Core.Materials;

namespace Illuminarium.TestScenes
{
    public class SceneCreatorSobolevEx : ISceneCreator
    {
        public Scene CreateScene()
        {
            var defaultMaterial = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.9f ) ) );

            Illuminarium.IO.ISceneLoader loader = new IO.SceneLoader3ds( (float)0.0254, defaultMaterial );
            var scene = loader.LoadFromFile( "TestScenes\\3ds\\sobolevex.3ds" );
            scene.Lights.Clear();
            scene.Lights.Add( new RectangleLight( new Point3D( 0.0f, 0.0f, 0f ), 0.1f, 0.1f, new Spectrum( 1 ), defaultMaterial, 32 ) );
            //scene.Lights.Add( new RectangleLight( new Point3D( -1f, 1f, 0f ), 1f, 1f, new Specturm( 1 ) ) );
            //scene.Lights.Add( new RectangleLight( new Point3D( 1f, -1f, 0f ), 1f, 1f, new Specturm( 1 ) ) );
            //scene.Lights.Add( new RectangleLight( new Point3D( -1f, -1f, 0f ), 1f, 1f, new Specturm( 1 ) ) );
            //scene.Lights.Add( new PointLight( "xx", new Point3D( -1, 1, 0 ) ) );
            //scene.Lights.Add( new PointLight( "xx", new Point3D( 1, -1, 0 ) ) );
            //scene.Lights.Add( new PointLight( "xx", new Point3D( -1, -1, 0 ) ) );

            float cx = 3.5f;
            foreach ( var obj in scene.Objects )
            {
                for ( int i = 0; i < obj.Vertices.Count; i++ )
                {
                    obj.Vertices[i] = new Point3D( obj.Vertices[i].X / cx, obj.Vertices[i].Y / cx, obj.Vertices[i].Z );
                }
            }

            //var Material = new DiffuseReflectanceMaterial( new Specturm( new float[Specturm.Size] { 0.5f, 1.0f, 0.9f } ) );
            var face11Material = new Material( new PhongReflectanceMaterial( new Spectrum( 0.8f, 0.8f, 0.8f ), 16, 1f ) );
            var face12Material = new Material( new PhongReflectanceMaterial( new Spectrum( 0.8f, 0.8f, 0.8f ), 16, 1f ) );
            scene.Objects[0].Faces[0].Material = face11Material;
            scene.Objects[0].Faces[1].Material = face12Material;
            //foreach ( var face in scene.Objects[0].Faces )
            //     face.Material = face1Material;

            //Material = new DiffuseReflectanceMaterial( new Specturm( new float[Specturm.Size] { 0.1f, 0.1f, 0.9f } ) );
            // верх
            //Material = new PhongReflectanceMaterial( new Spectrum( new float[Spectrum.Size] { 1f, 0f, 0f } ), 25, 0.9f );
            var face2Material = new Material( new PhongReflectanceMaterial( new Spectrum( 0.8f ), 16, 1f ) );
            foreach ( var face in scene.Objects[1].Faces )
                face.Material = face2Material;

            scene.RecalcSceneParams();
            scene.CameraSetDefault();
            scene.Camera.Position = new Point3D( 2f, 2f, 0f );
            scene.Camera.Target = new Point3D( 0f, 0f, 0f );
            return scene;
        }
    }
}
