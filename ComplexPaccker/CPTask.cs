using DawnUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexPaccker
{
    public class CPTask
    {
        public static void cpTaskCrossroad(string[] args)
        {
            if (args.Length >= 2)
            {
                if (args[1] == "-r")
                {
                    taskScan();
                    cpTaskCrossroadR(args, 1);
                }
                else
                {
                    cpTaskCrossroadR(args, 0);
                }
            }
            else
            {
                Terminal.WriteLine("&c%8%&[&c%c%&!&c%8%&]&c%c%&Couldn't resolve args");
            }
        }

        static List<string> Tasks = new List<string>();

        public static void cpTaskCrossroadR(string[] args, int argDelta)
        {
            if (Tasks.Count == 0)
            {
                taskScan();
            }
            string ta = args[1 + argDelta];
            switch (ta)
            {
                case "-list":
                    Terminal.WriteLine("&c%8%&[&c%2%&i&c%8%&]&c%6%&Listing...");
                    foreach (string t in Tasks)
                    {
                        Terminal.WriteLine($"&c%8%& |&c%6%&{t}");
                    }
                    Terminal.WriteLine("&c%8%&[-]&c%6%&Done!");
                    break;
                default:
                    if (Tasks.Contains(ta))
                    {
                        CPSI.CPScript.LoadRun($"./tasks/{ta}.cps");
                    }
                    else
                    {
                        Terminal.WriteLine($"&c%8%&[&c%c%&!&c%8%&]&c%c%&Unknown task!");
                    }
                    break;
            }
        }

        public static void taskScan()
        {
            Tasks = new List<string>();
            Terminal.WriteLine("&c%8%&[&c%2%&i&c%8%&]&c%6%&Scanning...");
            if (!Directory.Exists("./tasks"))
            {
                Directory.CreateDirectory("./tasks");
            }
            foreach (string file in Directory.GetFiles("./tasks"))
            {
                Terminal.WriteLine($"&c%8%& |&c%6%&{Path.GetFileName(file).Remove(Path.GetFileName(file).Length - 4)}&c%8%&[{file.Replace("\\","/")}]");
                Tasks!.Add(Path.GetFileName(file).Remove(Path.GetFileName(file).Length - 4));
            }
            Terminal.WriteLine($"&c%8%&[-]&c%6%&Done! {Tasks.Count()} Script(s) Loaded");
        }
    }
}
