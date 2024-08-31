using DawnUtils;
using CPScriptInterpreter;

namespace ComplexPacker
{
    internal static class Debug
    {
        public static void DebugCrossroad(string[] args)
        {
            #if DEBUG
            if (args.Length == 1)
            {
                PrintHelp();
            }
            else
            {
                switch (args[1])
                {
                    case "":
                        PrintHelp();
                        break;
                    case "terminal":
                        DebugTerminal();
                        break;
                    case "s":
                        if (args.Length == 3)
                        {
                            CpScript.LoadRun(args[2]);
                        }
                        else
                        {
                            Terminal.WriteLine("[!] Can't Resolve Args!");
                        }
                        break;
                    default:
                        ActionNotExist();
                        break;
                } 
            }
            #endif
        }

        private static void PrintHelp() 
        {
        
        }

        private static void ActionNotExist() 
        {
        
        }

        private static void DebugTerminal()
        {
            Terminal.DeColorfulBug();
        }


    }
}
