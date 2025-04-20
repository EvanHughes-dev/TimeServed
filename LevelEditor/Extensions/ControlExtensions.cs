using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Extensions
{
    /// <summary>
    /// Extensions on the Control class, children of it, and/or related classes.
    /// </summary>
    public static class ControlExtensions
    {
        /// <summary>
        /// Gets the form-relative location of this control, irrespective of any parents this control may have.
        /// </summary>
        /// <param name="control">This control.</param>
        /// <returns>The form-relative location of this control.</returns>
        public static Point GetAbsoluteLocation(this Control control)
        {
            // Recursive method!
            //   Goes up to the highest parent, and then adds all of the found locations together
            //   to find the form-relative location of this control (rather than just the parent-relative location)
            if (control.Parent != null)
                return control.Parent.GetAbsoluteLocation().Add(control.Location);
            else
                return control.Location;
        }
    }
}
