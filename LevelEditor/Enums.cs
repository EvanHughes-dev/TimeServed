using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    /// <summary>
    /// The type of key that an Item can be.
    /// </summary>
    public enum KeyType
    {
        None,
        KeyCard,
        Screwdriver
    }

    /// <summary>
    /// The four possible cardinal orientations, starting with North and 
    /// going clockwise by 90-degree increments.
    /// </summary>
    public enum Orientation
    {
        North,
        East,
        South,
        West
    }
}
