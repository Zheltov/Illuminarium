using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Illuminarium.Core;
using Illuminarium.Core.Lights;
using Illuminarium.Core.Materials;

namespace Illuminarium.TestScenes
{
    public class SceneCreatorRoom : ISceneCreator
    {
        public Scene CreateScene()
        {
            var defaultMaterial = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.5f ) ) );

            Illuminarium.IO.ISceneLoader loader = new Illuminarium.IO.SceneLoader3ds( (float)0.0254, defaultMaterial );
            var scene = loader.LoadFromFile( "TestScenes\\3ds\\2PlaneWithTable.3ds" );
            
            scene.RecalcSceneParams();
            scene.CameraSetDefault();
            scene.Camera.Position = new Point3D( 3.25f, -3f, 2f );
            scene.Camera.Target = new Point3D( -2.3f, 2.8f, 0.68f );

            // lights
            scene.Lights.Clear();
            //scene.Lights.Add( new PointLight( string.Empty, new Point3D( -3, 3, 2 ) ) );
            scene.Lights.Add( new RectangleLight( new Point3D( -1.2f, 1.5f, 2.5f ), 1f, 1f, new Spectrum( 1 ), defaultMaterial, 4 ) );
            //scene.Lights.Add( new RectangleLight( new Point3D( 1f, -2f, 2.5f ), 1f, 1f, new Spectrum( 1 ) ) );

            //scene.Lights.Add( new PointLight( string.Empty, new Point3D( 0, 0, 1 ) ) );
            //scene.Lights.Add( new PointLight( string.Empty, new Point3D( -2, 2, 2 ) ) );
            //scene.Lights.Add( new PointLight( string.Empty, new Point3D( 2, -2, 2 ) ) );
            //scene.Lights.Add( new PointLight( string.Empty, new Point3D( -2, -2, 2 ) ) );

            // materials
            var pol = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.1f, 0.5f, 0.4f ) ) );
            var potolok = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.1f, 0.1f, 0.1f ) ));
                //new PhongReflectanceMaterial( new Spectrum( new float[3] { 0.9f, 0.9f, 0.9f } ), 20f, 0f ); //new DiffuseReflectanceMaterial( new Specturm( new float[3] { 0.9f, 0.9f, 0.9f } ) );
            var st = new Material( new PhongReflectanceMaterial( new Spectrum( 0.5f, 0.3f, 0.1f ), 64f, 0.0f ) ); //new DiffuseReflectanceMaterial( new Specturm( new float[3] { 0.5f, 0.3f, 0.1f } ) );
            var n =  new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.4f, 0.1f, 0.9f ) ) );

            foreach ( var face in scene.Objects[0].Faces )
                face.Material = pol;

            foreach ( var face in scene.Objects[1].Faces )
                face.Material = potolok;

            foreach ( var face in scene.Objects[6].Faces )
                face.Material = st;

            foreach ( var face in scene.Objects[2].Faces )
                face.Material = n;

            foreach ( var face in scene.Objects[3].Faces )
                face.Material = n;

            foreach ( var face in scene.Objects[4].Faces )
                face.Material = n;

            foreach ( var face in scene.Objects[5].Faces )
                face.Material = n;

            return scene;
        }
    }
}
