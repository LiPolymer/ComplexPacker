using DawnUtils;
using CPSI;

namespace ComplexPaccker
{
    internal class Debug
    {
        public static void debugCrossroad(string[] args)
        {
            #if DEBUG
            if (args.Length == 1)
            {
                printHelp();
            }
            else
            {
                switch (args[1])
                {
                    case "":
                        printHelp();
                        break;
                    case "terminal":
                        Terminal.DeColorfulBug();
                        break;
                    case "s":
                        if (args.Length == 3)
                        {
                            CPScript.LoadRun(args[2]);
                        }
                        else
                        {
                            Terminal.WriteLine("[!] Arggggggg!");
                        }
                        break;
                    default:
                        actionNotExist();
                        break;
                } 
            }
            #endif
        }

        public static void printHelp() 
        {
        
        }

        public static void actionNotExist() 
        {
        
        }

        public static void debugTerminal()
        {
            Terminal.DeColorfulBug();
        }


    }
}
