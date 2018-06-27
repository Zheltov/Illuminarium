using Illuminarium.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Illuminarium
{
    public partial class FormRenderLog : Form, ILog
    {
        int lastTick;
        public FormRenderLog()
        {
            InitializeComponent();
        }

        public void Message( string message )
        {
            this.Message( message, 0 );
        }

        public void Message( string message, int level )
        {
            string prefix = string.Empty;

            for ( int i = 0; i < level; i++ )
                prefix += "   ";

            if ( this.Visible )
                if ( this.txtLog.Text.Length == 0 )
                    this.txtLog.Text = message;
                else
                    this.txtLog.AppendText( "\r\n" + prefix + "[" + ( (float)( Environment.TickCount - lastTick ) / 1000 ).ToString("0.000") + " ms] - " + message );

            lastTick = Environment.TickCount;
            Application.DoEvents();
        }

        public void Error( Exception ex )
        {
            if ( this.Visible )
                if ( this.txtLog.Text.Length == 0 )
                    this.txtLog.Text = ex.Message;
                else
                    this.txtLog.AppendText( "\r\n" + ex.Message );

            Application.DoEvents();
        }

        private void btnClose_Click( object sender, EventArgs e )
        {
            this.Close();
        }
    }
}
