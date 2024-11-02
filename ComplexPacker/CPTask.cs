using DawnUtils;

namespace ComplexPacker {
    public static class CpTask {
        public static void CpTaskCrossEntry(string[] args) {
            if (args.Length >= 2) {
                if (args[1] == "-r") {
                    TaskScan();
                    CpTaskCrossroad(args, 1);
                }
                else {
                    CpTaskCrossroad(args, 0);
                }
            }
            else {
                Terminal.WriteLine("&c%8%&[&c%c%&!&c%8%&]&c%c%&Couldn't resolve args");
            }
        }

        static List<string> _tasks = new List<string>();

        private static void CpTaskCrossroad(string[] args, int argDelta) {
            if (_tasks.Count == 0) {
                TaskScan();
            }
            string ta = args[1 + argDelta];
            switch (ta) {
                case "-list":
                    Terminal.WriteLine("&c%8%&[&c%2%&i&c%8%&]&c%6%&Listing...");
                    foreach (string t in _tasks) {
                        Terminal.WriteLine($"&c%8%& |&c%6%&{t}");
                    }
                    Terminal.WriteLine("&c%8%&[-]&c%6%&Done!");
                    break;
                default:
                    if (_tasks.Contains(ta)) {
                        CPScriptInterpreter.CpScript.LoadRun($"./tasks/{ta}.cps");
                    }
                    else {
                        Terminal.WriteLine($"&c%8%&[&c%c%&!&c%8%&]&c%c%&Unknown task!");
                    }
                    break;
            }
        }

        private static void TaskScan() {
            _tasks = new List<string>();
            Terminal.WriteLine("&c%8%&[&c%2%&i&c%8%&]&c%6%&Scanning...");
            if (!Directory.Exists("./tasks")) {
                Directory.CreateDirectory("./tasks");
            }
            foreach (string file in Directory.GetFiles("./tasks")) {
                if (Path.GetFileName(file).EndsWith(".cps")) {
                    Terminal.WriteLine(
                        $"&c%8%& |&c%6%&{Path.GetFileName(file).Remove(Path.GetFileName(file).Length - 4)}&c%8%&[{file.Replace("\\", "/")}]");
                    _tasks.Add(Path.GetFileName(file).Remove(Path.GetFileName(file).Length - 4));
                }
            }
            Terminal.WriteLine($"&c%8%&[-]&c%6%&Done! {_tasks.Count()} Task(s) Loaded");
        }
    }
}