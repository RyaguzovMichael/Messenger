using ServerApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApp.UI
{
    internal class ConsoleUI : IUserInterface
    {
        private IServerLogic serverLogic;

        public ConsoleUI(IServerLogic serverLogic)
        {
            this.serverLogic = serverLogic;
        }

        public void StartUI()
        {
            
        }
    }
}