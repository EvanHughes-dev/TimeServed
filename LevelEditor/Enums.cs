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
        None=0,
        KeyCard=1,
        Screwdriver=2
    }

    /// <summary>
    /// The four possible cardinal orientations, starting with North and 
    /// going clockwise by 90-degree increments.
    /// </summary>
    public enum Orientation
    {
        North=0,
        East=1,
        South=2,
        West=3
    }

    /// <summary>
    /// Define what type of object this is
    /// </summary>
    public enum ObjectType
    {
        Item = 0,
        Camera = 1,
        Box = 2,
        Door = 3
    }
    
    /// <summary>
    /// Get if a file's changes have been saved or not
    /// </summary>
    public enum SavedState{
        Unsaved,
        Saved
    }

}
