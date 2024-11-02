using DawnUtils;

namespace ComplexPacker
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            foreach (string ag in args) 
            {
                #if DEBUG
                Terminal.WriteLine(ag);
                #endif
            }
            if (args.Length == 0) 
            {
                InteractiveCli();
            }
            else
            {
                RootCrossroad(args);
            }
        }

        static void RootCrossroad(string[] args)
        {
            switch (args[0])
            {
                case "":
                    PrintAbout();
                    PrintHelp();
                    break;
                case "task":
                    CpTask.CpTaskCrossEntry(args);
                    break;
                case "build":
                    RootCrossroad(["task", "build"]);
                    break;
                case "run":
                    RootCrossroad(["task", "run"]);
                    break;
                case "publish":
                    RootCrossroad(["task", "publish"]);
                    break;
                case "debug":
                    RootCrossroad(["task", "build"]);
                    break;
#if DEBUG
                case "-dbg":
                    Debug.DebugCrossroad(args);
                    break;
                #endif
                default:
                    ActionNotExist();
                    break;
            }
        }

        static void PrintAbout()
        {
            Terminal.WriteLine("Complex Packer - Dev.Inf");
        }

        static void PrintHelp()
        {
            Terminal.WriteLine("");
        }

        static void ActionNotExist() 
        {
            Terminal.WriteLine("&c%8%&[&c%c%&!&c%8%&]&c%c%&Action Not Exist!");
        }

        static void InteractiveCli()
        {
            Terminal.WriteLine("&c%3%&Complex Packer &c%r%&- &c%6%&Interactive Mode");
            while (true)
            {
                Terminal.Write("> ");
                ConsoleColor cc = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                string cmd = Console.ReadLine()!;
                Console.ForegroundColor = cc;
                CommandProcessor(cmd);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        static void CommandProcessor(string cmd)
        {
            string[] args = Resolver.ResolveArgs(cmd);
            if (args.Length != 0)
            {
                switch (args[0])
                {
                    case "":
                        break;
                    case "help":
                        Terminal.WriteLine("&c%b%&[*]AvailableCommands");
                        break;
                    case "exit":
                        Environment.Exit(0);
                        break;
                    default:
                        RootCrossroad(args);
                        break;
                }
            }
        }
    }
}
