using DawnUtils;
using System.Net;
using System.Security.Cryptography;

namespace ComplexPaccker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            foreach (string ag in args)
            {
                Terminal.WriteLine(ag);
            }
            if (args.Length == 0)
            {
                interactiveCLI();
            }
            else
            {
                rootCrossroad(args);
            }
        }

        static void rootCrossroad(string[] args)
        {
            switch (args[0])
            {
                case "":
                    printAbout();
                    printHelp();
                    break;
                case "task":
                    CPTask.cpTaskCrossroad(args);
                    break;
                case "build":
                    rootCrossroad(["task", "build"]);
                    break;
                case "run":
                    rootCrossroad(["task", "run"]);
                    break;
                case "publish":
                    rootCrossroad(["task", "publish"]);
                    break;
                case "debug":
                    rootCrossroad(["task", "build"]);
                    break;
#if DEBUG
                case "-dbg":
                    Debug.debugCrossroad(args);
                    break;
                #endif
                default:
                    actionNotExist();
                    break;
            }
        }

        static void printAbout()
        {
            Terminal.WriteLine("Complex Paccker - Dev.Inf");
        }

        static void printHelp()
        {
            Terminal.WriteLine("");
        }

        static void actionNotExist() 
        {
            Terminal.WriteLine("&c%8%&[&c%c%&!&c%8%&]&c%c%&Action Not Exist!");
        }

        static void interactiveCLI()
        {
            Terminal.WriteLine("&c%3%&Complex Paccker &c%r%&- &c%6%&Interactive Mode");
            while (true)
            {
                Terminal.Write("> ");
                ConsoleColor cc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                string cmd = Console.ReadLine()!;
                Console.ForegroundColor = cc;
                CommandProcessor(cmd);
            }
        }

        static void CommandProcessor(string cmd)
        {
            string[] args = Resolver.ResolveArgs(cmd);
            if (args.Length == 0)
            {

            }
            else
            {
                switch (args[0])
                {
                    case "":
                        break;
                    case "help":
                        Terminal.WriteLine("&c%b%&[*]AvilableCommands");
                        break;
                    case "action":
                        if (cmd.Length > 7){
                            rootCrossroad(Resolver.ResolveArgs(cmd.Remove(0, 7)));
                        }
                        else
                        {
                            Terminal.WriteLine("&c%8%&[&c%c%&!&c%8%&]&c%c%&Can't resolve args");
                            Terminal.WriteLine("&c%b%&[*]Correct Using:");
                            Terminal.WriteLine("&c%b%& | action &c%e%&<action> &c%2%&<arg1> <arg...> ");
                            Terminal.WriteLine("&c%b%& -=[&c%e%&mandatory&c%b%&]=[&c%2%&opitional&c%b%&]=");
                        }
                        break;
                    case "task":
                        rootCrossroad(args);
                        break;
                    case "build":
                        rootCrossroad(["build"]);
                        break;
                    case "run":
                        rootCrossroad(["run"]);
                        break;
                    case "publish":
                        rootCrossroad(["publish"]);
                        break;
                    case "debug":
                        rootCrossroad(["debug"]);
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    default:
                        Terminal.WriteLine("&c%8%&[&c%c%&!&c%8%&]&c%c%&Command Not Exist!");
                        break;
                }
            }
        }
    }
}
