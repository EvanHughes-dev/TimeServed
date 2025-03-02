using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeEveryDayRecount
{
    public static class InputManager
    {
        private static KeyboardState PriorState { get; set; }
        private static KeyboardState CurrentState { get; set; }

        public static void Update()
        {
            throw new NotImplementedException("Update has not been implemented in InputManager");
        }

        public static bool GetKeyPress(Keys key)
        {
            throw new NotImplementedException("GetKeyPress has not been implemented in InputManager");
        }
        public static bool GetKeyRelease(Keys key)
        {
            throw new NotImplementedException("GetKeyRelease has not been implemented in InputManager");
        }
        public static bool GetKeyStatus(Keys key)
        {
            throw new NotImplementedException("GetKeyStatus has not been implemented in InputManager");
        }
    }
}
