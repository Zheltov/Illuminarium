using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Graph3D.OpenGL;

namespace Illuminarium
{
    /// <summary>
    /// Компонент для визуализации с использованием opengl
    /// </summary>
    public partial class OpenGLView : UserControl
    {
        /// <summary>
        /// Дескриптор окна вывода
        /// </summary>
        protected IntPtr hDC;
        /// <summary>
        /// opengl дескриптор окна вывода
        /// </summary>
        protected IntPtr hRC;

        /// <summary>
        /// Конструктор компонента
        /// </summary>
        public OpenGLView()
        {
            InitializeComponent();

            SetStyle(ControlStyles.ResizeRedraw, true);

            CreateOpenGLContext();
        }

        /// <summary>
        /// Создает контекст воспроизведения opengl
        /// </summary>
        protected virtual void CreateOpenGLContext()
        {
            OpenGL.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);

            hDC = OpenGL.GetDC(this.Handle);
            if (hDC == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            OpenGL.PIXELFORMATDESCRIPTOR pfd = new OpenGL.PIXELFORMATDESCRIPTOR();
            pfd.Initialize();

            pfd.dwFlags = OpenGL.PFD_DRAW_TO_WINDOW 
                | OpenGL.PFD_SUPPORT_OPENGL 
                | OpenGL.PFD_DOUBLEBUFFER;

            int npf = OpenGL.ChoosePixelFormat(hDC, ref pfd);
            OpenGL.SetPixelFormat(hDC, npf, ref pfd);

            hRC = OpenGL.wglCreateContext(hDC);
            if (hRC == IntPtr.Zero)
                throw new Win32Exception(Marshal.GetLastWin32Error());

            OpenGL.wglMakeCurrent(hDC, hRC);
            OpenGL.glClearColor(0.5f, 0.5f, 0.75f, 1.0f);
            //OpenGL.glClearColor( 1f, 1f, 1f, 1.0f );
            OpenGL.glPolygonMode(OpenGL.GL_FRONT_AND_BACK, OpenGL.GL_LINE);
            OpenGL.glEnable(OpenGL.GL_DEPTH_TEST);
        }

        protected override void OnGotFocus( EventArgs e )
        {
            base.OnGotFocus( e );

            OpenGL.wglMakeCurrent( hDC, hRC );
        }
    }
}
