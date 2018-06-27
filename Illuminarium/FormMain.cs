using Illuminarium.Core;
using Illuminarium.Core.Renderers;
using Illuminarium.Core.RayTracer.Embree;
using System;
using System.Drawing;
using System.Windows.Forms;
using Illuminarium.Core.Debug;
using Illuminarium.Core.GlobalIllumination;
using Illuminarium.Core.MeshViewIndependent;
using Illuminarium.Core.MeshViewIndependent.Renderer;

namespace Illuminarium
{
    public partial class FormMain : Form
    {
        public Scene Scene
        {
            get
            {
                return this.sceneViewer.Scene;
            }
            set
            {
                this.sceneViewer.Scene = value;
                this.ReloadScene( value );
            }
        }

        public FormMain()
        {
            InitializeComponent();

            sceneViewer.DebugRays = RayDebugStaticCollection.Rays;
        }
        
        public void ReloadScene( Scene sc )
        {
            sceneViewer.Scene = sc;
            sceneViewer.Camera = sc.Camera;
            sceneViewer.DrawScene();
            sceneViewer.MouseCursorMode = MouseCursorMode.Camera;
            sceneViewer.CameraMoveMode = CameraMoveMode.Orbit;
        }
        
        private void btnRenderDLE_Click( object sender, EventArgs e )
        {
            // Parameters
            Scene = new TestScenes.SceneCreatorCornellBox().CreateScene();   // Сцена
            int nPackets = 3;       // Число пакетов
            int nRays = 100;        // Число лучей внутри пакета для двойной локальной
            float wMin = 0.1f;      // Минимульный вес луча марковской цепи для локальной


            FormRenederResultView formImageView = new FormRenederResultView();
            FormRenderLog formLog = new FormRenderLog();
            formLog.Show();
            Application.DoEvents();

            formLog.Message( "--- Start render ---" );
            using ( IRayTracer rayTracer = new RayTracerEmbree( this.Scene ) )
            {
                var ticks = Environment.TickCount;
                IRenderer renderer = new SimpleRenderer( rayTracer );
                renderer.GlobalIllumination = new DoubleLocalEstimation( rayTracer, nPackets, nRays, wMin, formLog );

                renderer.Log = formLog;

                formLog.Message( "Strart render" );

                var renderResult = renderer.GenerateRenderPoints( this.sceneViewer.Camera, this.sceneViewer.Width, this.sceneViewer.Height );
                renderer.RenderDirectIllumination( renderResult );
                renderer.RenderGlobalIllumination( renderResult );
                renderer.FinalGathering( renderResult );

                formLog.Message( "Strart display image" );
                formImageView.RenderPointsStructure = renderResult;
            }
            formImageView.Show();
            formLog.Message( "--- Render complete ---" );
        }

        private void btnViewIndependentDLE_Click( object sender, EventArgs e )
        {

            // Parameters
            Scene = new TestScenes.SceneCreatorSobolevEx().CreateScene();   // Сцена

            int maxDivideDeep = 10; // Максимальная глубина разбиения грани, больше 14 лучше не задавать даже для Соболева
            int nTheta = 8;         // Число углов разбиения для расчета яркости по зенитному углу
            int nPhi = 16;          // Число углов разбиения для расчета яркости по азимутальному углу
            int nSH = 8;            // Число гармоник при аппроксимации через СГ (MVertexIlluminanceApproximationMode.SphericalHarmonics)
            int nRays = 100;        // Число лучей для двойной локальной
            float wMin = 0.1f;      // Минимульный вес луча марковской цепи для локальной

            // Create MScene
            var mscene = new MScene( Scene, new MeshSettings( maxDivideDeep, nTheta, nPhi, nSH ) );
            sceneViewer.MScene = mscene;

            MScene.BuildStaticMinimalMesh( mscene );

            FormRenderLog formLog = new FormRenderLog();
            formLog.Show();
            Application.DoEvents();

            using ( IRayTracer rayTracer = new RayTracerEmbree( Scene ) )
            {
                formLog.Message( "Render direct" );
                var renderDirect = new MRenderDirectLight( mscene, rayTracer, formLog );
                renderDirect.Calculate();

                formLog.Message( "Render indirect" );
                var renderIndirect = new MRenderDoubleLocalEst( mscene, rayTracer, nRays, wMin, formLog );
                renderIndirect.Calculate();

                formLog.Message( "Start update statistics", 0 );
                mscene.UpdateStatistics();
                formLog.Message( "End update statistics", 0 );
            }

            sceneViewer.IlluminanceApproximationMode = MVertexIlluminanceApproximationMode.Spline;
            sceneViewer.IlluminanceMode = MVertexIlluminanceMode.Full;
            sceneViewer.DrawMode = DrawMode.CalculatedMScene;
        }

        private void tsCamera_ItemClicked( object sender, ToolStripItemClickedEventArgs e )
        {
            // Режим мыши - управление камерой
            sceneViewer.MouseCursorMode = MouseCursorMode.Camera;

            // Выключаю все кнопки кроме выбранной
            foreach ( ToolStripItem tsi in tsCamera.Items )
                if ( tsi != e.ClickedItem )
                    ( (ToolStripButton)tsi ).Checked = false;

            if ( e.ClickedItem == btnCameraOrbit )
                sceneViewer.CameraMoveMode = CameraMoveMode.Orbit;
            else if ( e.ClickedItem == btnCameraZoom )
                sceneViewer.CameraMoveMode = CameraMoveMode.Zoom;
            else if ( e.ClickedItem == btnCameraDolly )
                sceneViewer.CameraMoveMode = CameraMoveMode.Dolly;
            else if ( e.ClickedItem == btnCameraTargeting )
                sceneViewer.CameraMoveMode = CameraMoveMode.Targeting;
        }

        private void btnTestTraceCameraRays_Click( object sender, EventArgs e )
        {
            // новый список отладочных лучей для контрола визуализации сцены
            RayDebugStaticCollection.Init();

            // создаем объект трассировщика, т.к. он IDisposable, то используем конструкцию using чтобы гарантированно высвободить ресурсы
            // см. public void Dispose() в RayTracerEmbree
            using ( IRayTracer rayTracer = new RayTracerEmbree( this.Scene ) )
            {

                // циклы по экранным координатам по x и y
                for ( int x = 0; x < this.sceneViewer.Width; x = x + 50 )
                    for ( int y = 0; y < this.sceneViewer.Height; y = y + 50 )
                    {
                        // получаем луч из камеры для соответствующих экранных координат
                        Ray ray = this.sceneViewer.Camera.GetTracingRay( x, y, this.sceneViewer.Width, this.sceneViewer.Height );

                        RayDebugStaticCollection.Add( new Ray( ray.From, ray.Direction ), Color.Red );

                        // ищем пересечение со сценой
                        var intersection = rayTracer.Trace( ray.From, ray.Direction );

                        // если пересчение со сценой есть (объект intersection не пустой), то добавляем луч в список отладочных лучей
                        if ( intersection != null )
                        {
                            RayDebugStaticCollection.Add( new Ray( ray.From, intersection.Point ), Color.Green );

                            var fall = ray.Direction;
                            var normal = intersection.Face.Normal;


                            var reflected = fall.Reflect( normal );

                            RayDebugStaticCollection.Add( new Ray( intersection.Point, normal ), Color.Blue );
                            RayDebugStaticCollection.Add( new Ray( intersection.Point, reflected ), Color.Red );
                        }
                    }
            }
        }

    }
}
