using Graph3D.OpenGL;
using System;

namespace Illuminarium
{
    partial class OpenGLView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            try
            {
                OpenGL.wglMakeCurrent(IntPtr.Zero, IntPtr.Zero);

                if (hRC != IntPtr.Zero)
                    OpenGL.wglDeleteContext(hRC);

                if (hDC != IntPtr.Zero)
                    OpenGL.ReleaseDC(Handle, hDC);

                hDC = hRC = IntPtr.Zero;
            }
            finally
            {
                base.Dispose(disposing);
            }

        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // OpenGLView
            // 
            this.Name = "OpenGLView";
            this.Size = new System.Drawing.Size(247, 143);
            this.ResumeLayout(false);

        }
        #endregion
    }
}
