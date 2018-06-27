using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Illuminarium.Core;

namespace Illuminarium.Test.DoubleLocalEstimation
{
    [TestClass]
    public class FormFactor
    {
        [TestMethod]
        public void SolidAngle()
        {
            ISceneCreator sceneCreator = new Illuminarium.TestScenes.SceneCreatorSphere();
            var scene = sceneCreator.CreateScene();

            var point = new Point3D( 0, 0, 0f );
            var normal = new Vector( 0f, 0f, 0f, true );

            float angle = 0;
            foreach ( var face in scene.Objects[0].Faces )
            {
                //12.56
                angle += Math3D.SolidAngle(
                   face.Obj.Vertices[face.VertexIndexes[0]] - point,
                   face.Obj.Vertices[face.VertexIndexes[1]] - point,
                   face.Obj.Vertices[face.VertexIndexes[2]] - point );
            }

            Assert.AreEqual( angle, 12.56f, 0.1f );
        }
    }
}
