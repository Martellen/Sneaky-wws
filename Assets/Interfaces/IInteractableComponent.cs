using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    interface IInteractableComponent
    {
        void HandleHover(bool isHovered);
        void HandleClick();
    }
}
