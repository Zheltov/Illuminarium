using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Illuminarium.Core;
using Illuminarium.Core.Lights;
using Illuminarium.Core.Materials;

namespace Illuminarium.TestScenes
{
    public class SceneCreatorDubrovnikSponza : ISceneCreator
    {
        public Scene CreateScene()
        {
            var defaultMaterial = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.5f ) ) );

            Illuminarium.IO.ISceneLoader loader = new Illuminarium.IO.SceneLoader3ds( (float)1, defaultMaterial );
            var scene = loader.LoadFromFile( "TestScenes\\3ds\\sponza.3ds" );

            scene.Lights.Add( new RectangleLight( new Point3D( 0, scene.Lights[0].Position.Y, 5f ), 14f, 0.5f, new Spectrum( 1 ), defaultMaterial, 4 ) );
            //scene.Lights.Add( new RectangleLight( new Point3D( 0, scene.Lights[0].Position.Y, 9f ), 14f, 0.5f, new Specturm( 1 ) ) );
            //scene.Lights.Add( new RectangleLight( new Point3D( 0, scene.Lights[0].Position.Y, 12f ), 14f, 0.5f, new Specturm( 1 ) ) );
            scene.Lights.RemoveAt( 0 );
            //scene.Lights[0].Position = new Point3D( scene.Lights[0].Position.X, scene.Lights[0].Position.Y, 6f );
            //scene.Lights[0].Position = new Point3D( scene.Lights[0].Position.X, scene.Lights[0].Position.Y, 6f );
            //scene.Lights.Add( new PointLight( "", new Point3D( 4, 0, 6 ) ) );
            //scene.Lights.Add( new PointLight( "", new Point3D( -4, 0, 6 ) ) );


            scene.RecalcSceneParams();
            scene.CameraSetDefault();
            scene.Camera.Position = new Point3D( 14f, 2f, 2f );
            scene.Camera.Target = new Point3D( 0f, 0f, 3f );

            // materials
            //var floor = new DiffuseReflectanceMirrorMaterial( new Spectrum( new float[Spectrum.Size] { 0.5f, 0.5f, 0.5f } ), new Spectrum( new float[Spectrum.Size] { 0.1f, 0.1f, 0.1f } ) );
            var floor = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0.85f, 0.8f, 0.8f ) ) );
            //var floor = new PhongReflectanceMaterial( new Specturm( new float[Specturm.Size] { 0.9f, 0.1f, 0.1f } ), 5f, 0.9f );
            foreach ( var obj in scene.Objects.Where( x => x.Name.Contains( "wall" ) ).ToList() )
            {
                foreach ( var face in obj.Faces )
                    face.Material = floor;
            }

            var arc = new Material( new DiffuseReflectanceMaterial( new Spectrum( 1f, 1f, 1f ) ) );
            foreach ( var obj in scene.Objects.Where( x => x.Name.Contains( "arc" ) ).ToList() )
            {
                foreach ( var face in obj.Faces )
                    face.Material = arc;
            }

            var hole = new Material( new DiffuseReflectanceMaterial( new Spectrum( 0f, 1f, 0f ) ) );
            //var hole = new DiffuseReflectanceMirrorMaterial( new Spectrum( new float[Spectrum.Size] { 0.1f, 0.1f, 0.1f } ), new Spectrum( new float[Spectrum.Size] { 0.8f, 0.8f, 0.8f } ) );
            foreach ( var obj in scene.Objects.Where( x => x.Name.Contains( "pil" ) ).ToList() )
            {
                foreach ( var face in obj.Faces )
                    face.Material = hole;
            }



            return scene;
        }
    }
}
