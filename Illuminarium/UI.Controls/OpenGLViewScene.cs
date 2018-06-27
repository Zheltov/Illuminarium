using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using Graph3D.OpenGL;
using Illuminarium.Core;
using Illuminarium.Core.Debug;
using Illuminarium.Core.MeshTransform;
using Illuminarium.Core.MeshViewIndependent;
using System.Linq;

namespace Illuminarium
{
    /// <summary>
    /// Режим перемещения камеры
    /// </summary>
    public enum CameraMoveMode
    {
        None,
        Orbit,
        Zoom,
        Dolly,
        Targeting
    }

    /// <summary>
    /// Режим рисования сцены
    /// </summary>
    public enum DrawMode
    {
        Default,
        DefaultMScene,
        CalculatedMScene
    }

    /// <summary>
    /// Режим мыши
    /// </summary>
    public enum MouseCursorMode
    {
        None,
        Camera
    }

    /// <summary>
    /// Компонет отображающий сцену Scene с использование opengl
    /// </summary>
    public partial class OpenGLViewScene : OpenGLView
    {
        static readonly float ANGEL_SENSITIVITY = 0.02f;

        static readonly float ZOOM_SENSITIVITY = 0.1f;

        static readonly float DOLLY_SENSITIVITY = 0.3f;

        static readonly float TARGETING_SENSITIVITY = 0.03f;

        #region Fields

        int oldX;
        int oldY;

        IList<AngleFunc> angleFuncs;
        Scene scene;
        MScene mscene;
        Camera camera = new Camera();
        DrawMode priorDrawMode;
        #endregion

        #region Propertys

        public IList<AngleFunc> AngleFuncs
        {
            get { return angleFuncs; }
            set { angleFuncs = value; DrawScene(); }
        }
        public IList<RayDebug> DebugRays { get; set; }
        public Scene Scene
        {
            get { return scene; }
            set
            {
                this.ReloadView();
                scene = value;
            }
        }
        public MScene MScene
        {
            get { return mscene; }
            set
            {
                this.ReloadView();
                mscene = value;
            }
        }
        internal Camera Camera
        {
            get { return camera; }
            set
            {
                camera = value;
                this.ReloadView();
            }
        }
        public CameraMoveMode CameraMoveMode { get; set; }
        public DrawMode DrawMode { get; set; }
        public MVertexIlluminanceMode IlluminanceMode { get; set; }
        public MVertexIlluminanceApproximationMode IlluminanceApproximationMode { get; set; }
        public MouseCursorMode MouseCursorMode { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Конструктор
        /// </summary>
        public OpenGLViewScene()
        {
            InitializeComponent();

            this.DebugRays = new List<RayDebug>();


        }

        #endregion

        #region Methods

        /// <summary>
        /// Перезагрузка данных о текущем ракурсе в opengl.
        /// Необходимо вызывать при переключениях камер, либо при изменении 
        /// положения камеры
        /// </summary>
        public void ReloadView()
        {
            if ( DesignMode )
                return;

            if ( ClientSize.Height != 0 )
            {
                OpenGL.glViewport( 0, 0, ClientSize.Width, ClientSize.Height );
                OpenGL.glMatrixMode( OpenGL.GL_PROJECTION );
                OpenGL.glLoadIdentity();

                //if ( ClientSize.Width > ClientSize.Height )
                var depth = scene != null ? scene.PMax.Length * 10 : 1000;
                OpenGL.gluPerspective( (double)this.Camera.Fov * 180 / Math.PI, ClientSize.Width / ClientSize.Height, Constants.Epsilon, depth );

                OpenGL.gluLookAt( camera.Position.X,
                    camera.Position.Y,
                    camera.Position.Z,
                    camera.Target.X,
                    camera.Target.Y,
                    camera.Target.Z,
                    camera.UpVector.X,
                    camera.UpVector.Y,
                    camera.UpVector.Z );
                OpenGL.glMatrixMode( OpenGL.GL_MODELVIEW );
            }
        }

        /// <summary>
        /// Отрисовка сцены, на основе состояния DrawMode
        /// </summary>
        public void DrawScene()
        {
            OpenGL.glClear( OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT );

            if ( DesignMode )
                return;

            switch ( DrawMode )
            {
                case DrawMode.Default:
                    OpenGL.glPolygonMode( OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE );
                    DrawSceneDefault();
                    DrawLights();
                    DrawAngleFuncs();
                    DrawDebugRays();
                    break;

                case DrawMode.DefaultMScene:
                    OpenGL.glPolygonMode( OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE );
                    DrawMSceneDefault();
                    DrawLights();
                    DrawAngleFuncs();
                    DrawDebugRays();
                    break;
                case DrawMode.CalculatedMScene:
                    OpenGL.glPolygonMode( OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_FILL );
                    //OpenGL.glPolygonMode( OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE );
                    DrawMSceneCalculated();
                    DrawLights();
                    DrawDebugRays();
                    //DrawMSceneIlluminanceBodies();
                    break;
            }

            OpenGL.SwapBuffers( hDC );
        }


        /// <summary>
        /// Отрисовка изначальной сцены без учета расчета
        /// </summary>
        protected void DrawSceneDefault()
        {
            //OpenGL.glClear( OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT );
            //OpenGL.glPolygonMode( OpenGL.GL_BACK, OpenGL.GL_POINT | OpenGL.GL_FILL );
            //OpenGL.glPolygonMode( OpenGL.GL_BACK, OpenGL.GL_LINE );

            //OpenGL.glBlendFunc( OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA );
            //OpenGL.glEnable( OpenGL.GL_BLEND );
            //OpenGL.glClearColor( 0f, 0f, 0f, 0f );

            //OpenGL.glMaterialfv( OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_DIFFUSE, new float[3] { 1, 1, 1 } );

            //OpenGL.glEnable( OpenGL.GL_LIGHT0 );
            //OpenGL.glLightfv( OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, new float[3] { 0, 0, 12} );
            //OpenGL.glLightfv( OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, new float[3] { 0, 0, -1 } );

            if ( scene != null )
            {
                foreach ( Obj o in scene.Objects )
                    foreach ( Face f in o.Faces )
                    {
                        OpenGL.glBegin( OpenGL.GL_TRIANGLES );
                        for ( int i = 0; i < 3; i++ )
                        {
                            //OpenGL.glLineWidth(3f);
                            OpenGL.glColor4f( 0f, 0f, 0f, 0.2f );
                            OpenGL.glVertex3f(
                                f.Obj.Vertices[f.VertexIndexes[i]].X,
                                f.Obj.Vertices[f.VertexIndexes[i]].Y,
                                f.Obj.Vertices[f.VertexIndexes[i]].Z );
                        }

                        OpenGL.glEnd();
                    }



            }
        }
        protected void DrawMSceneDefault()
        {
            if ( mscene != null )
            {
                foreach ( var o in mscene.Objects )
                    foreach ( var f in o.Faces )
                    {
                        OpenGL.glBegin( OpenGL.GL_TRIANGLES );
                        for ( int i = 0; i < 3; i++ )
                        {
                            //OpenGL.glLineWidth(3f);
                            OpenGL.glColor4f( 0f, 0f, 0f, 0.2f );
                            OpenGL.glVertex3f(
                                f.Vertices[i].Point.X,
                                f.Vertices[i].Point.Y,
                                f.Vertices[i].Point.Z );
                        }
                        OpenGL.glEnd();

                        
                        float cxyz = 0.2f;
                        for ( int i = 0; i < 3; i++ )
                        {
                            var vertex = f.Vertices[i];

                            foreach ( var direction in vertex.IlluminanceAngles.Directions )
                            {
                                OpenGL.glColor3f( 0f, 0f, 1f );
                                OpenGL.glBegin( OpenGL.GL_LINES );

                                OpenGL.glVertex3f( vertex.Point.X, vertex.Point.Y, vertex.Point.Z );
                                OpenGL.glVertex3f( vertex.Point.X + direction.X * cxyz, vertex.Point.Y + direction.Y * cxyz, vertex.Point.Z + direction.Z * cxyz );

                                OpenGL.glEnd();
                            }

                        }
                        
                    }
            }
        }

        class MFaceIlluminance
        {
            public MFace Face;
            public Spectrum[] Illuminance;

            public MFaceIlluminance( MFace face )
            {
                Face = face;
                Illuminance = new Spectrum[3];
            }
        }

        protected void DrawMSceneCalculated()
        {
            if ( mscene == null )
                return;

            float norm = 0; // mscene.MaxIlluminanceDirect;
            var mfaces = new List<MFaceIlluminance>();

            foreach ( var o in mscene.Objects )
                foreach ( var f in o.Faces )
                {
                    var mface = new MFaceIlluminance( f );
                    for ( int i = 0; i < 3; i++ )
                    {
                        var vertex = f.Vertices[i];
                        var ill = vertex.GetIlluminance( camera.Position, IlluminanceMode, IlluminanceApproximationMode );

                        /*
                        var a1 = vertex.GetIlluminance( camera.Position, IlluminanceMode, MVertexIlluminanceApproximationMode.Spline );
                        var a2 = vertex.GetIlluminance( camera.Position, IlluminanceMode, MVertexIlluminanceApproximationMode.SphericalHarmonics );
                        if ( a1.R > 0.000001 )
                            a1 = a1 + a1;
                        */

                        mface.Illuminance[i] = ill;
                        if ( ill.ValueMax > norm )
                            norm = ill.ValueMax;
                    }
                    mfaces.Add( mface );
                }

            if ( norm == 0 ) norm = 1;

            //norm = MScene.MaxIlluminanceDirect + MScene.MaxIlluminanceIndirect;

            foreach ( var f in mfaces )
            {
                OpenGL.glBegin( OpenGL.GL_TRIANGLES );
                for ( int i = 0; i < 3; i++ )
                {
                    var vertex = f.Face.Vertices[i];
                    var ill = f.Illuminance[i] / norm;
                    OpenGL.glColor3f( ill.R, ill.G, ill.B );
                    OpenGL.glVertex3f(
                        vertex.Point.X,
                        vertex.Point.Y,
                        vertex.Point.Z );
                }
                OpenGL.glEnd();
            }
        }
        protected void DrawMSceneIlluminanceBodies()
        {
            float cxyz = 0.2f;
            foreach ( var obj in MScene.Objects )
                foreach ( var vertex in obj.Vertices )
                { 
                        if ( vertex.Point.Z > 0 )
                            cxyz = 0.21f;

                        var m = vertex.IlluminanceIndirect.Select( x => x.ValueMax ).Max();
                        if ( m == 0 )
                            m = 1;
                        for ( int iDirection = 0; iDirection < vertex.IlluminanceAngles.Directions.Count; iDirection++ )
                        {
                            var direction = vertex.IlluminanceAngles.Directions[iDirection];

                        float length = 0;
                        switch( IlluminanceMode )
                        {
                            case MVertexIlluminanceMode.Direct:
                                length = vertex.IlluminanceDirect[iDirection].ValueMax;
                                break;
                            case MVertexIlluminanceMode.Indirect:
                                length = vertex.IlluminanceIndirect[iDirection].ValueMax;
                                break;
                            case MVertexIlluminanceMode.Full:
                                length = vertex.IlluminanceDirect[iDirection].ValueMax + vertex.IlluminanceIndirect[iDirection].ValueMax;
                                break;
                        }

                         length = length * cxyz / m;
                            if ( length > 0 )
                            {
                                OpenGL.glColor3f( 0f, 1f, 0f );
                                OpenGL.glBegin( OpenGL.GL_LINES );

                                OpenGL.glVertex3f( vertex.Point.X, vertex.Point.Y, vertex.Point.Z );
                                OpenGL.glVertex3f( vertex.Point.X + direction.X * length, vertex.Point.Y + direction.Y * length, vertex.Point.Z + direction.Z * length );

                                OpenGL.glEnd();
                            }
                        }
                    }
        }
        protected void DrawLights()
        {
            if ( scene == null )
                return;

            foreach ( var light in scene.Lights )
                if ( light is IObj )
                    foreach ( Face f in ( (IObj)light ).Faces )
                    {
                        OpenGL.glBegin( OpenGL.GL_TRIANGLES );
                        for ( int i = 0; i < 3; i++ )
                        {
                            OpenGL.glColor3f( 0.8f, .8f, .8f );
                            OpenGL.glVertex3f(
                                f.Obj.Vertices[f.VertexIndexes[i]].X,
                                f.Obj.Vertices[f.VertexIndexes[i]].Y,
                                f.Obj.Vertices[f.VertexIndexes[i]].Z );
                        }
                        OpenGL.glEnd();
                    }
        }
        protected void DrawDebugRays()
        {
            if ( this.DebugRays != null )
            {
                foreach ( var ray in this.DebugRays )
                {
                    OpenGL.glColor3f( (float)ray.Color.R / 255, (float)ray.Color.G / 255, (float)ray.Color.B / 255 );
                    OpenGL.glBegin( OpenGL.GL_LINES );
                    OpenGL.glVertex3f( ray.Ray.From.X, ray.Ray.From.Y, ray.Ray.From.Z );
                    OpenGL.glVertex3f( ray.Ray.To.X, ray.Ray.To.Y, ray.Ray.To.Z );
                    OpenGL.glEnd();
                }
            }
        }
        protected void DrawAngleFuncs()
        {
            if ( angleFuncs != null )
            {

                var pi2 = Math.PI / 2;
                foreach ( var angleFunc in angleFuncs )
                {

                    for ( int imu = 0; imu < angleFunc.Mu.Count - 1; imu++ )
                    {
                        for ( int iphi = 0; iphi < angleFunc.Phi.Count - 1; iphi++ )
                        {
                            var dir1 = angleFunc.Point + new Vector( (float)( pi2 - Math.Acos( (double)angleFunc.Mu[imu + 0] ) ), angleFunc.Phi[iphi + 0] ) * angleFunc.R[imu + 0][iphi + 0];
                            var dir2 = angleFunc.Point + new Vector( (float)( pi2 - Math.Acos( (double)angleFunc.Mu[imu + 1] ) ), angleFunc.Phi[iphi + 0] ) * angleFunc.R[imu + 1][iphi + 0];
                            var dir3 = angleFunc.Point + new Vector( (float)( pi2 - Math.Acos( (double)angleFunc.Mu[imu + 1] ) ), angleFunc.Phi[iphi + 1] ) * angleFunc.R[imu + 1][iphi + 1];
                            var dir4 = angleFunc.Point + new Vector( (float)( pi2 - Math.Acos( (double)angleFunc.Mu[imu + 0] ) ), angleFunc.Phi[iphi + 1] ) * angleFunc.R[imu + 0][iphi + 1];

                            OpenGL.glBegin( OpenGL.GL_QUADS );

                            OpenGL.glColor3f( (float)angleFunc.Color.R / 255, (float)angleFunc.Color.G / 255, (float)angleFunc.Color.B / 255 ); OpenGL.glVertex3f( dir1.X, dir1.Y, dir1.Z );
                            OpenGL.glColor3f( (float)angleFunc.Color.R / 255, (float)angleFunc.Color.G / 255, (float)angleFunc.Color.B / 255 ); OpenGL.glVertex3f( dir2.X, dir2.Y, dir2.Z );
                            OpenGL.glColor3f( (float)angleFunc.Color.R / 255, (float)angleFunc.Color.G / 255, (float)angleFunc.Color.B / 255 ); OpenGL.glVertex3f( dir3.X, dir3.Y, dir3.Z );
                            OpenGL.glColor3f( (float)angleFunc.Color.R / 255, (float)angleFunc.Color.G / 255, (float)angleFunc.Color.B / 255 ); OpenGL.glVertex3f( dir4.X, dir4.Y, dir4.Z );

                            OpenGL.glEnd();
                        }
                    }
                }
            }
        }


        #endregion

        #region Events

        /// <summary>
        /// Событие производящие непосредственно отображение сцены
        /// </summary>
        protected override void OnPaint( PaintEventArgs e )
        {
            DrawScene();
        }
        /// <summary>
        /// Реакция на изменение размеров компонента.
        /// </summary>
        protected override void OnResize( EventArgs e )
        {
            base.OnResize( e );

            ReloadView();
        }
        /// <summary>
        /// Перемещение мыши
        /// </summary>
        protected override void OnMouseMove( MouseEventArgs e )
        {
            if ( e.Button == MouseButtons.Left )
            {
                switch ( CameraMoveMode )
                {
                    case CameraMoveMode.Orbit:
                        if ( oldY > e.Y )
                            camera.Phi += ANGEL_SENSITIVITY;
                        if ( oldY < e.Y )
                            camera.Phi -= ANGEL_SENSITIVITY;

                        if ( oldX > e.X )
                            camera.Theta += ANGEL_SENSITIVITY;
                        if ( oldX < e.X )
                            camera.Theta -= ANGEL_SENSITIVITY;
                        break;

                    case CameraMoveMode.Zoom:
                        if ( oldY > e.Y )
                            camera.R += ZOOM_SENSITIVITY;
                        if ( oldY < e.Y )
                            camera.R -= ZOOM_SENSITIVITY;
                        break;

                    case CameraMoveMode.Dolly:
                        if ( oldY > e.Y )
                            camera.Dolly( DOLLY_SENSITIVITY );
                        if ( oldY < e.Y )
                            camera.Dolly( -DOLLY_SENSITIVITY );
                        break;

                    case CameraMoveMode.Targeting:
                        float dx;
                        if ( oldX > e.X )
                            dx = 1;
                        else
                            dx = -1;
                        camera.Targeting( dx * TARGETING_SENSITIVITY, 0 );
                        break;
                }
                oldX = e.X;
                oldY = e.Y;
                ReloadView();
                DrawScene();
            }
        }
        /// <summary>
        /// Нажатие мышки
        /// </summary>
        protected override void OnMouseDown( MouseEventArgs e )
        {
            base.OnMouseDown( e );

            if ( e.Button == MouseButtons.Left )
            {
                // Переключаем в default draw mode
                priorDrawMode = DrawMode;
                //drawMode = DrawMode.Default;
            }

            ReloadView();
            DrawScene();
        }
        /// <summary>
        /// Отжатие кнопки мыши
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp( MouseEventArgs e )
        {
            base.OnMouseUp( e );
            if ( e.Button == MouseButtons.Left )
            {
                DrawMode = priorDrawMode;
            }
            //drawMode = DrawMode.Mesh;

            DrawScene();
        }
        protected override void OnMouseWheel( MouseEventArgs e )
        {
            base.OnMouseWheel( e );

            if ( e.Delta > 0 )
                camera.R += ZOOM_SENSITIVITY;
            else
                camera.R -= ZOOM_SENSITIVITY;

            ReloadView();
            DrawScene();
        }

        #endregion
    }
}