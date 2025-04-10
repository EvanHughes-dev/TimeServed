using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Controls
{
    /// <summary>
    /// An extension of PictureBox that is adjusted to be point-filtered.
    /// </summary>
    public class PixelBox : PictureBox
    {
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics graphics = pe.Graphics;

            // Point filtering!
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            // Offsets all sampling by half a pixel. Read https://stackoverflow.com/a/54726707 to understand why this is important,
            //   there are some very clear images that make it easy to understand!
            graphics.PixelOffsetMode = PixelOffsetMode.Half;

            base.OnPaint(pe);
        }
    }
}
