using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace LevelEditor.Controls
{
    public class RayRenderer : Control
    {
        /// <summary>
        /// The start point of the ray 
        /// </summary>
        public Point RayStart { get; set; }
        public Point RayEnd { get; set; }
        public float RayRadianSpread { get; set; }
        public Color RayColor { get; set; }

        public RayRenderer()
        {
            Debug.WriteLine("RayRenderer created!");
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            
            Pen pen = new Pen(RayColor)
            {
                StartCap = LineCap.RoundAnchor,
                EndCap = LineCap.ArrowAnchor,
            };

            graphics.DrawLine(pen, RayStart, RayEnd);
            
            // TODO: Implement drawing the sweeping arc of the camera ray
            //graphics.DrawArc()
            
            base.OnPaint(e);
        }
    }
}
