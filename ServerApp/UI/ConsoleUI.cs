using ServerApp.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ServerApp.UI;

public delegate string CommandReaction(string[] args);

internal class ConsoleUI : IUserInterface
{
    private readonly IServer _serverLogic;
    private readonly Dictionary<string, CommandReaction> _commandReactions;
    private readonly Dictionary<string, string> _commandsHelp;
    private const string HELP_INFO_PATH = @".\Data\HelpData.json";

    public ConsoleUI(IServer serverLogic)
    {
        _serverLogic = serverLogic;
        _commandReactions = new()
        {
            { "help",  GetHelpInfo},
            { "clear", ClearConsole},
            { "clr", ClearConsole},
            { "restart", RestartServer},
            { "stop", StopServer},
            { "start", StartServer},
            { "exit", ExitServer},
            { "getip", GetIP},
            { "setip", SetIP},
            { "gip", GetIP},
            { "sip", SetIP},
            { "getport", GetPort},
            { "setport", SetPort},
            { "gp", GetPort},
            { "sp", SetPort},
            { "getendpoint", GetEndPoint},
            { "setendpoint", SetEndPoint},
            { "gep", GetEndPoint},
            { "sep", SetEndPoint},
        };
        _commandsHelp = null;
        if (File.Exists(HELP_INFO_PATH))
            _commandsHelp = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(HELP_INFO_PATH), new JsonSerializerOptions() { WriteIndented = true });
    }

    public void StartUI()
    {
        Console.WriteLine("------- Simple server console V1.0.0 --------\n");
        while (true)
        {
            Console.Write("> ");
            string command = Console.ReadLine().ToLower().Trim();
            string[] commands = command.Split(' ');
            command = commands[0];
            string[] args = new string[commands.Length - 1];
            if (args.Length > 0)
            {
                Array.Copy(commands, 1, args, 0, commands.Length - 1);
            }
            if (_commandReactions.TryGetValue(command, out CommandReaction replyGenerator)) Console.WriteLine(replyGenerator.Invoke(args));
            else
            {
                Console.WriteLine("Unknown command");
                Console.WriteLine(GetHelpInfo(Array.Empty<string>()));
            }
        }
    }

    private string GetHelpInfo(string[] args)
    {
        if (_commandsHelp == null) return "No help data!";
        if (args.Length == 0) return GetHelpInfo(new string[] { "default" });
        if (args.Length > 1) return GetHelpInfo(new string[] { "help" });
        if (_commandsHelp.TryGetValue(args[0], out string helpInfo)) return helpInfo;
        else
        {
            return $"No information about this command: \"{args[0]}\"\n" + GetHelpInfo(new string[] { "default" });
        }
    }

    private string ClearConsole(string[] args)
    {
        Console.Clear();
        return "------- Simple server console V1.0.0 --------\n";
    }

    private string RestartServer(string[] args)
    {
        try
        {
            _serverLogic.RestartServer();
            return "Server restarted";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private string StopServer(string[] args)
    {
        try
        {
            _serverLogic.StopServer();
            return "The server is stopped";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private string StartServer(string[] args)
    {
        try
        {
            _serverLogic.StartServer();
            return "Server is up and running";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private string ExitServer(string[] args)
    {
        try
        {
            _serverLogic.StopServer();
            Environment.Exit(0);
            return "";
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private string SetEndPoint(string[] args) => "Not included command on server";

    private string GetEndPoint(string[] args) => "Not included command on server";

    private string SetPort(string[] args)
    {
        if (args.Length != 1) return GetHelpInfo(new string[] { "setport" });
        try
        {
            _serverLogic.Port = int.Parse(args[0]);
            return GetPort(args);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    private string GetPort(string[] args) => $"Server Port: {_serverLogic.Port}";

    private string SetIP(string[] args) => "Not included command on server";

    private string GetIP(string[] args) => "Not included command on server";
}