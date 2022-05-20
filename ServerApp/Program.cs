using ServerApp.Interfaces;
using ServerApp.Server;
using ServerApp.UI;


IServerLogic serverLogic = new ServerLogic();
serverLogic.StartServer();
IUserInterface userInterface = new ConsoleUI(serverLogic);
userInterface.StartUI();