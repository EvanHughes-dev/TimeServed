using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor
{
    public readonly struct Tile
    {
        public Image Sprite { get; }
        public bool IsWalkable { get; }

        public Tile(Image sprite, bool isWalkable)
        {
            Sprite = sprite;
            IsWalkable = isWalkable;
        }
    }
}
