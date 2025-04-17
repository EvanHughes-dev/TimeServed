using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Controls
{
    /// <summary>
    /// An extension of PixelBox that can draw a border on top of the drawn image.
    /// </summary>
    public class BorderBox : PixelBox
    {
        private float _borderWidth;
        /// <summary>
        /// How wide the border should be, in pixels.
        /// </summary>
        public float BorderWidth
        {
            get => _borderWidth;
            set
            {
                _borderWidth = value;
                Invalidate(); // We have to redraw!
            }
        }

        private Color _borderColor;
        /// <summary>
        /// The color the border should be.
        /// </summary>
        public Color BorderColor
        {
            get => _borderColor;
            set
            {
                _borderColor = value;
                Invalidate(); // We have to redraw!
            }
        }

        /// <summary>
        /// Draws the border on top of the drawn image.
        /// </summary>
        /// <param name="pe">The provided event args.</param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            Graphics graphics = pe.Graphics;

            // If the BorderWidth is zero or less, don't draw a damn border!
            if (BorderWidth > 0)
            {
                Pen pen = new Pen(BorderColor, BorderWidth);

                // DrawRectangle draws the *outline* of a rectangle
                graphics.DrawRectangle(pen, new Rectangle(Point.Empty, Size));
            }
        }
    }
}
