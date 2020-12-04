using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _TielJ
{
    public static class Tests
    {
        public static void PlayerTest()
        {
            Player.Player.Initialize();
            Player.Player.Play("https://www.youtube.com/watch?v=HBpXtVtfaWE");
        }
    }
}
