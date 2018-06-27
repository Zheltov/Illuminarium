namespace Illuminarium
{
    partial class FormMain
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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.sceneViewer = new Illuminarium.OpenGLViewScene();
            this.tsTests = new System.Windows.Forms.ToolStrip();
            this.btnTestTraceCameraRays = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRenderDLE = new System.Windows.Forms.ToolStripButton();
            this.btnViewIndependentDLE = new System.Windows.Forms.ToolStripButton();
            this.tsCamera = new System.Windows.Forms.ToolStrip();
            this.btnCameraOrbit = new System.Windows.Forms.ToolStripButton();
            this.btnCameraZoom = new System.Windows.Forms.ToolStripButton();
            this.btnCameraDolly = new System.Windows.Forms.ToolStripButton();
            this.btnCameraTargeting = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tsTests.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.tsCamera.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.sceneViewer);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1033, 507);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(1033, 557);
            this.toolStripContainer1.TabIndex = 1;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsTests);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tsCamera);
            // 
            // sceneViewer
            // 
            this.sceneViewer.AngleFuncs = null;
            this.sceneViewer.CameraMoveMode = Illuminarium.CameraMoveMode.None;
            this.sceneViewer.DebugRays = null;
            this.sceneViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sceneViewer.DrawMode = Illuminarium.DrawMode.Default;
            this.sceneViewer.IlluminanceApproximationMode = Illuminarium.Core.MeshViewIndependent.MVertexIlluminanceApproximationMode.Spline;
            this.sceneViewer.IlluminanceMode = Illuminarium.Core.MeshViewIndependent.MVertexIlluminanceMode.Full;
            this.sceneViewer.Location = new System.Drawing.Point(0, 0);
            this.sceneViewer.MouseCursorMode = Illuminarium.MouseCursorMode.None;
            this.sceneViewer.MScene = null;
            this.sceneViewer.Name = "sceneViewer";
            this.sceneViewer.Scene = null;
            this.sceneViewer.Size = new System.Drawing.Size(1033, 507);
            this.sceneViewer.TabIndex = 1;
            // 
            // tsTests
            // 
            this.tsTests.Dock = System.Windows.Forms.DockStyle.None;
            this.tsTests.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnTestTraceCameraRays});
            this.tsTests.Location = new System.Drawing.Point(210, 0);
            this.tsTests.Name = "tsTests";
            this.tsTests.Size = new System.Drawing.Size(149, 25);
            this.tsTests.TabIndex = 2;
            this.tsTests.Visible = false;
            // 
            // btnTestTraceCameraRays
            // 
            this.btnTestTraceCameraRays.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnTestTraceCameraRays.Image = ((System.Drawing.Image)(resources.GetObject("btnTestTraceCameraRays.Image")));
            this.btnTestTraceCameraRays.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTestTraceCameraRays.Name = "btnTestTraceCameraRays";
            this.btnTestTraceCameraRays.Size = new System.Drawing.Size(106, 22);
            this.btnTestTraceCameraRays.Text = "Trace camera rays";
            this.btnTestTraceCameraRays.Click += new System.EventHandler(this.btnTestTraceCameraRays_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRenderDLE,
            this.btnViewIndependentDLE});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(459, 25);
            this.toolStrip1.TabIndex = 0;
            // 
            // btnRenderDLE
            // 
            this.btnRenderDLE.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRenderDLE.Image = ((System.Drawing.Image)(resources.GetObject("btnRenderDLE.Image")));
            this.btnRenderDLE.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRenderDLE.Name = "btnRenderDLE";
            this.btnRenderDLE.Size = new System.Drawing.Size(179, 22);
            this.btnRenderDLE.Text = "Render Double Local Estimation";
            this.btnRenderDLE.Click += new System.EventHandler(this.btnRenderDLE_Click);
            // 
            // btnViewIndependentDLE
            // 
            this.btnViewIndependentDLE.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnViewIndependentDLE.Image = ((System.Drawing.Image)(resources.GetObject("btnViewIndependentDLE.Image")));
            this.btnViewIndependentDLE.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnViewIndependentDLE.Name = "btnViewIndependentDLE";
            this.btnViewIndependentDLE.Size = new System.Drawing.Size(237, 22);
            this.btnViewIndependentDLE.Text = "View Independent Double Local Estimation";
            this.btnViewIndependentDLE.Click += new System.EventHandler(this.btnViewIndependentDLE_Click);
            // 
            // tsCamera
            // 
            this.tsCamera.Dock = System.Windows.Forms.DockStyle.None;
            this.tsCamera.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCameraOrbit,
            this.btnCameraZoom,
            this.btnCameraDolly,
            this.btnCameraTargeting});
            this.tsCamera.Location = new System.Drawing.Point(3, 25);
            this.tsCamera.Name = "tsCamera";
            this.tsCamera.Size = new System.Drawing.Size(193, 25);
            this.tsCamera.TabIndex = 1;
            this.tsCamera.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsCamera_ItemClicked);
            // 
            // btnCameraOrbit
            // 
            this.btnCameraOrbit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCameraOrbit.Image = ((System.Drawing.Image)(resources.GetObject("btnCameraOrbit.Image")));
            this.btnCameraOrbit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCameraOrbit.Name = "btnCameraOrbit";
            this.btnCameraOrbit.Size = new System.Drawing.Size(38, 22);
            this.btnCameraOrbit.Text = "Orbit";
            // 
            // btnCameraZoom
            // 
            this.btnCameraZoom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCameraZoom.Image = ((System.Drawing.Image)(resources.GetObject("btnCameraZoom.Image")));
            this.btnCameraZoom.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCameraZoom.Name = "btnCameraZoom";
            this.btnCameraZoom.Size = new System.Drawing.Size(43, 22);
            this.btnCameraZoom.Text = "Zoom";
            // 
            // btnCameraDolly
            // 
            this.btnCameraDolly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCameraDolly.Image = ((System.Drawing.Image)(resources.GetObject("btnCameraDolly.Image")));
            this.btnCameraDolly.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCameraDolly.Name = "btnCameraDolly";
            this.btnCameraDolly.Size = new System.Drawing.Size(38, 22);
            this.btnCameraDolly.Text = "Dolly";
            // 
            // btnCameraTargeting
            // 
            this.btnCameraTargeting.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCameraTargeting.Image = ((System.Drawing.Image)(resources.GetObject("btnCameraTargeting.Image")));
            this.btnCameraTargeting.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCameraTargeting.Name = "btnCameraTargeting";
            this.btnCameraTargeting.Size = new System.Drawing.Size(62, 22);
            this.btnCameraTargeting.Text = "Targeting";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 535);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1033, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 16);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1033, 557);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Illuminarium";
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tsTests.ResumeLayout(false);
            this.tsTests.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.tsCamera.ResumeLayout(false);
            this.tsCamera.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private OpenGLViewScene sceneViewer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStrip tsCamera;
        private System.Windows.Forms.ToolStripButton btnCameraOrbit;
        private System.Windows.Forms.ToolStripButton btnCameraZoom;
        private System.Windows.Forms.ToolStripButton btnCameraDolly;
        private System.Windows.Forms.ToolStripButton btnCameraTargeting;
        private System.Windows.Forms.ToolStrip tsTests;
        private System.Windows.Forms.ToolStripButton btnTestTraceCameraRays;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.ToolStripButton btnRenderDLE;
        private System.Windows.Forms.ToolStripButton btnViewIndependentDLE;
    }
}

