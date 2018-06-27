namespace Illuminarium.Graph
{
    partial class FormGraph2DLine
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
            this.components = new System.ComponentModel.Container();
            this.Graph1 = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // Graph1
            // 
            this.Graph1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Graph1.Location = new System.Drawing.Point(0, 0);
            this.Graph1.Name = "Graph1";
            this.Graph1.ScrollGrace = 0D;
            this.Graph1.ScrollMaxX = 0D;
            this.Graph1.ScrollMaxY = 0D;
            this.Graph1.ScrollMaxY2 = 0D;
            this.Graph1.ScrollMinX = 0D;
            this.Graph1.ScrollMinY = 0D;
            this.Graph1.ScrollMinY2 = 0D;
            this.Graph1.Size = new System.Drawing.Size(784, 562);
            this.Graph1.TabIndex = 0;
            // 
            // FormGraph2DLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.Graph1);
            this.Name = "FormGraph2DLine";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormGraph2DLine";
            this.ResumeLayout(false);

        }

        #endregion

        internal ZedGraph.ZedGraphControl Graph1;


    }
}