namespace Illuminarium
{
    partial class FormSceneView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSceneView));
            this.sceneViewer = new Illuminarium.OpenGLViewScene();
            this.SuspendLayout();
            // 
            // sceneViewer
            // 
            this.sceneViewer.CameraMoveMode = Illuminarium.CameraMoveMode.None;
            this.sceneViewer.DebugRays = ((System.Collections.Generic.IList<Illuminarium.Core.Debug.RayDebug>)(resources.GetObject("sceneViewer.DebugRays")));
            this.sceneViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sceneViewer.Location = new System.Drawing.Point(0, 0);
            this.sceneViewer.MouseCursorMode = Illuminarium.MouseCursorMode.None;
            this.sceneViewer.Name = "sceneViewer";
            this.sceneViewer.Scene = null;
            this.sceneViewer.Size = new System.Drawing.Size(792, 386);
            this.sceneViewer.TabIndex = 0;
            // 
            // FormSceneView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(792, 386);
            this.Controls.Add(this.sceneViewer);
            this.Name = "FormSceneView";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SceneViewer";
            this.ResumeLayout(false);

        }

        #endregion

        private OpenGLViewScene sceneViewer;
    }
}