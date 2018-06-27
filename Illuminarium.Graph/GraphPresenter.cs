using Illuminarium.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace Illuminarium.Graph
{
    public class GraphPresenter
    {
        public void IlliminanceLine2D( IList<XYGraphInfo> graphs )
        {
            var frm = new FormGraph2DLine();

            // Get a reference to the GraphPane instance in the ZedGraphControl
            GraphPane myPane = frm.Graph1.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Illuminance";
            myPane.XAxis.Title.Text = "X";
            myPane.YAxis.Title.Text = "Illuminance";

            foreach( var graph in graphs)
            {
                PointPairList data = new PointPairList();
                for ( int i = 0; i < graph.X.Count; i++ )
                    data.Add( new PointPair( graph.X[i], graph.Y[i] ) );

                LineItem curve = myPane.AddCurve( graph.Title, data, graph.Color, SymbolType.XCross );
            }

            // Make up some data points based on the Sine function
            PointPairList list = new PointPairList();
            PointPairList list2 = new PointPairList();
            for ( int i = 0; i < 36; i++ )
            {
                double x = (double)i * 5.0;
                double y = Math.Sin( (double)i * Math.PI / 15.0 ) * 16.0;
                double y2 = y * 13.5;
                list.Add( x, y );
                list2.Add( x, y2 );
            }

            
            //// Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;
            myPane.YAxis.MajorGrid.IsVisible = true;
            
            //// Make the Y axis scale red
            //myPane.YAxis.Scale.FontSpec.FontColor = Color.Red;
            //myPane.YAxis.Title.FontSpec.FontColor = Color.Red;
            //// turn off the opposite tics so the Y tics don't show up on the Y2 axis
            //myPane.YAxis.MajorTic.IsOpposite = false;
            //myPane.YAxis.MinorTic.IsOpposite = false;
            //// Don't display the Y zero line
            //myPane.YAxis.MajorGrid.IsZeroLine = false;
            //// Align the Y axis labels so they are flush to the axis
            //myPane.YAxis.Scale.Align = AlignP.Inside;
            //// Manually set the axis range
            //myPane.YAxis.Scale.Min = -30;
            //myPane.YAxis.Scale.Max = 30;

            //// Enable the Y2 axis display
            //myPane.Y2Axis.IsVisible = true;
            //// Make the Y2 axis scale blue
            //myPane.Y2Axis.Scale.FontSpec.FontColor = Color.Blue;
            //myPane.Y2Axis.Title.FontSpec.FontColor = Color.Blue;
            //// turn off the opposite tics so the Y2 tics don't show up on the Y axis
            //myPane.Y2Axis.MajorTic.IsOpposite = false;
            //myPane.Y2Axis.MinorTic.IsOpposite = false;
            //// Display the Y2 axis grid lines
            //myPane.Y2Axis.MajorGrid.IsVisible = true;
            //// Align the Y2 axis labels so they are flush to the axis
            //myPane.Y2Axis.Scale.Align = AlignP.Inside;

            //// Fill the axis background with a gradient
            //myPane.Chart.Fill = new Fill( Color.White, Color.LightGray, 45.0f );

            //// Add a text box with instructions
            //TextObj text = new TextObj(
            //    "Zoom: left mouse & drag\nPan: middle mouse & drag\nContext Menu: right mouse",
            //    0.05f, 0.95f, CoordType.ChartFraction, AlignH.Left, AlignV.Bottom );
            //text.FontSpec.StringAlignment = StringAlignment.Near;
            //myPane.GraphObjList.Add( text );

            // Enable scrollbars if needed
            frm.Graph1.IsShowHScrollBar = true;
            frm.Graph1.IsShowVScrollBar = true;
            frm.Graph1.IsAutoScrollRange = true;
            frm.Graph1.IsScrollY2 = true;

            // OPTIONAL: Show tooltips when the mouse hovers over a point
            frm.Graph1.IsShowPointValues = true;
            /*
            frm.Graph1.PointValueEvent += new ZedGraphControl.PointValueHandler( MyPointValueHandler );

            // OPTIONAL: Add a custom context menu item
            frm.Graph1.ContextMenuBuilder += new ZedGraphControl.ContextMenuBuilderEventHandler(
                            MyContextMenuBuilder );

            // OPTIONAL: Handle the Zoom Event
            frm.Graph1.ZoomEvent += new ZedGraphControl.ZoomEventHandler( MyZoomEvent );

            // Size the control to fit the window
            SetSize();
            */
            // Tell ZedGraph to calculate the axis ranges
            // Note that you MUST call this after enabling IsAutoScrollRange, since AxisChange() sets
            // up the proper scrolling parameters
            frm.Graph1.AxisChange();
            // Make sure the Graph gets redrawn
            frm.Graph1.Invalidate();

            frm.Show();
        }
    }

    
}
