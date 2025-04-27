using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Classes.Triggers;

namespace LevelEditor.Controls
{
    /// <summary>
    /// An extension of BorderBox specialized to store a Prop.
    /// </summary>
    internal class TriggerBox : BorderBox
    {
        private Trigger _trigger; 

        /// <summary>
        /// Gets or sets the Prop this PropBox should display.
        /// </summary>
        public Trigger Trigger
        {
            get => _trigger;
            set
            {
                _trigger = value!;
                Invalidate();
            }
        }
        /// <summary>
        /// Override of onpaint which creates the icon representation of what 
        /// each trigger looks like in the trigger pallette
        /// </summary>
        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics graphics = pe.Graphics;

            Rectangle boundRect = new Rectangle(0, 0, Bounds.Size.Width, Bounds.Size.Height);

            //let me have my headcannon
            Pen moana = new Pen(_trigger.Color, 4);
            Brush maoi = new SolidBrush(Color.FromArgb(63, _trigger.Color));

            graphics.DrawRectangle(moana, boundRect);
            graphics.FillRectangle(maoi, boundRect);
            base.OnPaint(pe);
        }
    }
}
