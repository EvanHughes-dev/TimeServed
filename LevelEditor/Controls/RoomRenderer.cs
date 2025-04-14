using LevelEditor.Classes;
using LevelEditor.Extensions;
using System.Drawing.Drawing2D;

namespace LevelEditor.Controls
{
    public class RoomRenderer : Control
    {
        public bool ShowTriggers { get; set; }
        
        private Room _room;
        public Room Room
        {
            get => _room;
            set
            {
                _room = value;
                UpdateDisplayProperties();
            }
        }

        public int RoomWidth
        {
            get => Room.Tiles.GetLength(1);
        }

        public int RoomHeight
        {
            get => Room.Tiles.GetLength(0);
        }

        private RectangleF _roomBounds;
        private float _tileSize;

        public RoomRenderer(Control? parent = null, string? text = null, int left = 0, int top = 0, int width = 0, int height = 0)
            : base(parent, text, left, top, width, height)
        {
            _room = null!;
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            base.SetBoundsCore(x, y, width, height, specified);

            UpdateDisplayProperties();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;

            // Point filtering!
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            // Offsets all sampling by half a pixel. Read https://stackoverflow.com/a/54726707 to understand why this is important,
            //   there are some very clear images that make it easy to understand!
            graphics.PixelOffsetMode = PixelOffsetMode.Half;

            DrawTiles(graphics);
            DrawProps(graphics);
            if (ShowTriggers) DrawTriggers(graphics);

            base.OnPaint(e);
        }

        private void UpdateDisplayProperties()
        {
            // Dimensions are x,y
            Point dimensions = new Point(
                Room.Tiles.GetLength(1),
                Room.Tiles.GetLength(0)
                );

            float tileSize = MathF.Min(
                Bounds.Width / (float)dimensions.X,
                Bounds.Height / (float)dimensions.Y
                );

            SizeF displaySize = new SizeF(
                dimensions.X * tileSize,
                dimensions.Y * tileSize
                );

            SizeF unused = Bounds.Size - displaySize;

            _roomBounds = new RectangleF(
                (unused / 2).ToPointF(), // The map display should be centered within the unused space
                displaySize
                );

            _tileSize = tileSize;
        }

        private void DrawTiles(Graphics graphics)
        {
            for (int tileY = 0; tileY < RoomHeight; tileY++)
            {
                for (int tileX = 0; tileX < RoomWidth; tileX++)
                {
                    Tile tile = Room.Tiles[tileY, tileX];

                    RectangleF drawRect = new RectangleF(
                        new PointF(
                            _tileSize * tileX,
                            _tileSize * tileY
                            ).Add(_roomBounds.Location),
                        new SizeF(_tileSize, _tileSize)
                        );

                    graphics.DrawImage(tile.Sprite, drawRect);
                }
            }
        }

        private void DrawProps(Graphics graphics)
        {

        }

        private void DrawTriggers(Graphics graphics)
        {
            throw new NotImplementedException();
        }
    }
}
