using ServerApp.Abstractions;
using ServerApp.Server;
using ServerApp.UI;

IServer serverLogic = new ServerLogic();
serverLogic.StartServer();
IUserInterface userInterface = new ConsoleUI(serverLogic);
userInterface.StartUI();