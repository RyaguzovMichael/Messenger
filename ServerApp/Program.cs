using ServerApp.Helpers;
using ServerApp.Interfaces;
using ServerApp.Server;
using ServerApp.UI;
using System;

namespace ServerApp
{
    internal class Program
    {
        static void Main()
        {
            IServerLogic serverLogic = new ServerLogic();
            IUserInterface userInterface = new ConsoleUI(serverLogic);
            userInterface.StartUI();
        }
    }
}