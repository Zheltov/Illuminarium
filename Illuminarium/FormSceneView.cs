using Illuminarium.Core;
using Illuminarium.Core.MeshTransform;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Illuminarium
{
    public partial class FormSceneView : Form
    {
        public Scene Scene
        {
            get { return sceneViewer.Scene; }
            set { sceneViewer.Scene = value; sceneViewer.Camera = value.Camera; }
        }

        public IList<AngleFunc> AngleFuncs
        {
            get { return sceneViewer.AngleFuncs; }
            set { sceneViewer.AngleFuncs = value; }
        }

        public FormSceneView()
        {
            InitializeComponent();
            sceneViewer.CameraMoveMode = CameraMoveMode.Orbit;
            sceneViewer.Camera.Position = new Point3D( 1, 1, 0 );
            sceneViewer.Camera.Target = new Point3D();
        }
    }
}
