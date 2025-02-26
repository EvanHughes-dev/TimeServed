using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeEveryDayRecount.GameObjects
{
    internal interface IInteractable
    {
        public void Interact(Player player);
    }
}
