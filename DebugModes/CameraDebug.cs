using MakeEveryDayRecount.Managers;
using MakeEveryDayRecount.Map;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using MakeEveryDayRecount.GameObjects.Props;

namespace MakeEveryDayRecount.DebugModes
{
    /// <summary>
    /// Debug all cameras in the current room
    /// </summary>
    internal class CameraDebug : BaseDebug
    {
        private Room _currentRoom;

        /// <summary>
        /// Create a new camera debug
        /// </summary>
        public CameraDebug()
        {
            MapManager.OnRoomUpdate += newRoom => _currentRoom = newRoom;
        }

        /// <summary>
        /// Draw all text and camera's rays to the screen
        /// </summary>
        /// <param name="sb">Sprite batch to draw with</param>
        public override void Draw(SpriteBatch sb)
        {

            foreach (Camera cam in _currentRoom.Cameras)
            {
                cam.DebugDraw(sb);
            }
            base.Draw(sb, "Camera");
        }
    }
}