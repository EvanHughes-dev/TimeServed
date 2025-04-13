using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Classes.Props
{
    /// <summary>
    /// Represents a single Prop (interactable GameObject) in a room.
    /// </summary>
    public abstract class Prop
    {
        /// <summary>
        /// This prop's sprite.
        /// </summary>
        public Image Sprite { get; private set; }
        /// <summary>
        /// This point's integer position in the room, in tile space.
        /// (0, 0) is the top-left corner of the room.
        /// </summary>
        public Point? Position { get; set; }

        /// <summary>
        /// Index of this image in the game's array
        /// </summary>
        public int ImageIndex {get; private set; }

        /// <summary>
        /// The type of object this prop is
        /// </summary>
        public ObjectType PropType{ get; private set;}
        
        /// <summary>
        /// Creates a new Prop with the given sprite and position.
        /// </summary>
        /// <param name="sprite">The sprite this Prop displays with.</param>
        /// <param name="position">The position of this Prop.</param>
        /// <param name="imageIndex"> Index of this image inside the coresponding array in the game</prarm>
        /// <param name="objectType">Type of object this is</param>
        public Prop(Image sprite, int imageIndex, ObjectType objectType, Point? position=null)
        {
            // Save params without filtering
            Sprite = sprite;
            ImageIndex = imageIndex;
            Position = position;
            PropType = objectType;
        }

        /// <summary>
        /// Creates a copy of this Prop at the given position.
        /// </summary>
        /// <param name="position">The position to copy this Prop to.</param>
        /// <returns>A copy of this Prop at the given position.</returns>
        public abstract Prop Instantiate(Point position);
    }
}
