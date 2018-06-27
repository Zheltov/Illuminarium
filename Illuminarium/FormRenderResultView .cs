using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Illuminarium.Core;

namespace Illuminarium
{
    public partial class FormRenederResultView : Form
    {
        private RenderPointsStructure renderPointsStructure;

        public RenderMode RenderMode { get; set; }

        public RenderPointsStructure RenderPointsStructure
        {
            get { return renderPointsStructure; }
            set
            {
                this.renderPointsStructure = value;

                SetRenderPointsStructure();
            }
        }

        public FormRenederResultView()
        {
            InitializeComponent();
            Initialize();
        }

        void Initialize()
        {
            tscmbRenderMode.ComboBox.DataSource = Enum.GetValues( typeof( RenderMode ) );
            tscmbRenderMode.ComboBox.SelectedItem = RenderMode.Full;
        }

        void SetRenderPointsStructure()
        {
            this.ClientSize = new Size( renderPointsStructure.Width, renderPointsStructure.Height + 25 );

            var maxIlluminance = float.MinValue;
            var maxIlluminanceLight = float.MinValue;

            var objs = this.RenderPointsStructure.RenderPoints.Where( x => !( x.Obj is ILight ) ).ToList();

            switch ( this.RenderMode )
            {
                case RenderMode.Direct:
                    maxIlluminance = objs.Max( x => x.IlluminanceDirect.ValueMax );
                    break;
                case RenderMode.Indirect:
                    maxIlluminance = objs.Max( x => x.IlluminanceIndirect.ValueMax );
                    break;
                case RenderMode.Full:
                    maxIlluminance = objs.Max( x => x.Illuminance.ValueMax );
                    break;
            }

            objs = this.RenderPointsStructure.RenderPoints.Where( x => x.Obj is ILight ).ToList();
            if ( objs.Count > 0 )
                maxIlluminanceLight = objs.Max( x => x.IlluminanceDirect.ValueMax );

            int ambient = 0;
            Color ambientColor = Color.FromArgb( ambient, ambient, ambient );

            float cx = maxIlluminanceLight;

            Bitmap bmp = new Bitmap( renderPointsStructure.Width, renderPointsStructure.Height );
            for ( int x = 0; x < renderPointsStructure.Width; x++ )
                for ( int y = 0; y < renderPointsStructure.Height; y++ )
                {
                    var s = new Spectrum();
                    var renderPoint = renderPointsStructure.GetRenderPoint( x, y );

                    Color color;
                    if ( renderPoint != null )
                    {
                        Spectrum normalizedSpectrum = new Spectrum();
                        Spectrum ill = new Spectrum();
                        if ( renderPoint.Obj is ILight )
                        {
                            ill = renderPoint.IlluminanceDirect;
                            normalizedSpectrum = renderPoint.IlluminanceDirect / maxIlluminanceLight;
                        }
                        else
                        {
                            switch ( this.RenderMode )
                            {
                                case RenderMode.Direct:
                                    ill = renderPoint.IlluminanceDirect;
                                    normalizedSpectrum = ill / maxIlluminance;
                                    break;
                                case RenderMode.Indirect:
                                    ill = renderPoint.IlluminanceIndirect;
                                    normalizedSpectrum = ill / maxIlluminance;
                                    break;
                                case RenderMode.Full:
                                    ill = renderPoint.Illuminance;
                                    normalizedSpectrum = ill / maxIlluminance;
                                    break;
                            }
                        }

                        if ( ill.ValueMax > 0 )
                        {
                            //s = ( normalizedSpectrum / cx );
                            var r = (int)( normalizedSpectrum.R * ( 255 - ambient ) );
                            var g = (int)( normalizedSpectrum.G * ( 255 - ambient ) );
                            var b = (int)( normalizedSpectrum.B * ( 255 - ambient ) );

                            if ( r < 0 || g < 0 || b < 0 )
                                color = Color.Red;
                            else
                                color = Color.FromArgb( r, g, b );
                        }
                        else
                            color = Color.Black;
                        bmp.SetPixel( x, y, color );
                    }
                    else
                        bmp.SetPixel( x, y, Color.Black );
                }

            this.pnlImage.BackgroundImage = bmp;
        }

        private void FormRenederResultView_Shown( object sender, EventArgs e )
        {
        }


        private void tscmbRenderMode_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( !this.Visible )
                return;

            this.RenderMode = (RenderMode)tscmbRenderMode.ComboBox.SelectedItem;
            this.SetRenderPointsStructure();
        }
    }

    public enum RenderMode
    {
        Full,
        Direct,
        Indirect
    }
}
