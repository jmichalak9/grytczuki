using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbelThueOnline
{
    internal interface IGame
    {
        List<int> GetAvailableActions();
        IGame MakeMove(int action);
    }
}
