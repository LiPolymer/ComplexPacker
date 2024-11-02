using DawnUtils;
using System.Diagnostics;
using System.IO.Compression;

namespace CPScriptInterpreter {
    public static class CpScript {
        private static void Run(string[] script) {
            CpsrInstance instance = new CpsrInstance(script);
            instance.Start();
        }

        public static void StrRun(string scriptStr) {
            string[] script = scriptStr.Split("\r\n");
            Run(script);
        }

        public static void LoadRun(string? path) {
            if (path != null) {
                try {
                    Run(File.ReadAllLines(path));
                }
                catch (Exception e) {
                    Terminal.WriteLine("&c%8%&[&c%c%&!&c%8%&]&c%c%&Error & Stack trace:&c%6%&\r\n&c%6%&" + e);
                }
            }
        }

        private class CpsrInstance(
            string[] s,
            string cache = "./cache",
            string outp = "./build",
            string rootp = "./src",
            string aname = "artifact") {
            // ReSharper disable MemberCanBePrivate.Local
            // ReSharper disable FieldCanBeMadeReadOnly.Local
            public string[] Script = s;
            public Dictionary<string, string> Vars = new();

            public Dictionary<string, string> EnvVars = new()
            {
                ["project.cache"] = cache,
                ["project.outdest"] = outp,
                ["project.rootdest"] = rootp,
                ["project.artifactName"] = aname
            };

            public List<string> MkCopyIgnores = new();

            public bool ShowState = true;
            // ReSharper enable MemberCanBePrivate.Local
            // ReSharper enable FieldCanBeMadeReadOnly.Local

            public void Start() {
                Runner(Script);
            }

            public void Runner(string[] s) {
                //Loader
                int line = 0;
                foreach (string statement in s) {
                    line += 1;
                    if (!StateConductor(statement, line)) {
                        break;
                    }
                }
            }

            public bool StateConductor(string s, int line = 0) {
                try {
                    s = VarFormater(s);
                    if (s.StartsWith('#')) {
                        return true;
                    }
                    if (ShowState) {
                        Terminal.WriteLine(
                            $"&c%8%&[&c%b%&{line}&c%8%&]&c%e%&{s}"
                        );
                    }
                    switch (s.Split(' ')[0]) {
                        case "makecopy":
                            MakeCopyCrossroad(s);
                            break;
                        case "copy":
                            CopyCp(Resolver.ResolveArgs(s));
                            break;
                        case "hidescript":
                            ShowState = false;
                            break;
                        case "showscript":
                            ShowState = true;
                            break;
                        case "sleep":
                            Thread.Sleep(Convert.ToInt32(s.Substring(6)));
                            break;
                        case "task":
                            RunTask(s.Substring(5), false);
                            break;
                        case "load":
                            RunTask(s.Substring(5), true);
                            break;
                        case "del":
                            DeleteDo(s);
                            break;
                        case "var":
                            VarSet(s);
                            break;
                        case "env":
                            EvarSet(s);
                            break;
                        case "shdo":
                            ShDo(s);
                            break;
                        case "echo":
                            Terminal.WriteLine("&c%8%&[&c%7%&>&c%8%&]&c%r%&" + s.Substring(5));
                            break;
                        case "pkg":
                            PkgCrossroad(s);
                            break;
                        default:
                            Terminal.WriteLine(
                                $"&c%8%&[&c%4%&!&c%8%&]&c%c%&UnknownStatement&c%8%&[&c%e%&{
                                    s.Split(' ')[0]
                                    + "&c%8%&"
                                    + s.Substring(s.Split(' ').Length + 2)
                                }&c%8%&]"
                            );
                            break;
                    }
                }
                catch (Exception e) {
                    Terminal.WriteLine("&c%6%&[Line" + Convert.ToString(line) +
                                       "] Error & Stack trace:&c%6%&\r\n&c%6%&" + e);
                    return false;
                }
                return true;
            }

            public void ShDo(string s) {
                Terminal.WriteLine($"&c%8%&[&c%c%&c&c%8%&]&c%7%&Executing &c%8%&[&c%7%&{s.Substring(5)}&c%8%&]");
                string[] ss = s.Split(' ');
                ProcessStartInfo i = new ProcessStartInfo(ss[1], s.Substring(5 + ss[1].Length))
                    { RedirectStandardOutput = true };
                var p = Process.Start(i);
                if (p == null) {
                    Terminal.WriteLine("&c%8%&[&c%c%&!&c%8%&]&c%7%&Failed!");
                }
                else {
                    var sr = p.StandardOutput;
                    while (!p.HasExited) {
                        while (!sr.EndOfStream) {
                            Terminal.WriteLine($"&c%8%&[&c%c%&c&c%8%&][&c%7%&>&c%8%&]{sr.ReadLine()}");
                        }
                    }
                    Terminal.WriteLine(
                        $"&c%8%&[&c%c%&c&c%8%&]&c%7%&Executing finished in &c%8%&[&c%7%&{(p.ExitTime - p.StartTime).TotalMilliseconds}&c%8%&] &c%7%&ms");
                }
            }

            // task & clsm
            public void RunTask(string path, bool isCarry) {
                string[] s = File.ReadAllLines(path);
                if (isCarry) {
                    Runner(s);
                }
                else {
                    CpsrInstance i = new CpsrInstance(s);
                    i.Start();
                }
            }

            //Package
            public void PkgCrossroad(string s) {
                string[] ss = Resolver.ResolveArgs(s);
                switch (ss[1]) {
                    case "make":
                        switch (ss[2]) {
                            case "zip":
                                if (ss.Length > 3) {
                                    PkgMakeZip(ss[3], ss[4], ss[5]);
                                }
                                else {
                                    PkgMakeZip();
                                }
                                break;
                        }
                        break;
                    case "apart":
                        switch (ss[2]) {
                            case "zip":
                                PkgApartZip(ss[3], ss[4]);
                                break;
                        }
                        break;
                }
            }

            public void PkgMakeZip(string? tdir = null, string? odir = null, string? ofnm = null) {
                tdir ??= GetEnvVar("project.cache");
                odir ??= GetEnvVar("project.outdest");
                ofnm ??= VarFormater(GetEnvVar("project.artifactName"));
                Terminal.WriteLine($"&c%8%&[&c%6%&z&c%8%&]&c%7%&Creating ZipPack...");
                if (!Directory.Exists(odir)) {
                    Directory.CreateDirectory(odir);
                }
                PkgZipGenerator(tdir,
                    Path.Combine(odir, ofnm)
                        .Replace("\\", "/")
                );
            }

            public void PkgApartZip(string tfile, string tdest) {
                if (!Directory.Exists(tdest)) {
                    Directory.CreateDirectory(tdest);
                }
                ZipFile.ExtractToDirectory(tfile, tdest);
            }

            public void PkgZipGenerator(string target, string outpath) {
                Terminal.WriteLine($"&c%8%&[&c%6%&z&c%8%&]&c%7%&{outpath} &c%8%&from {target}");
                if (File.Exists(outpath)) {
                    File.Delete(outpath);
                }
                ZipFile.CreateFromDirectory(target, outpath);
            }

            // Vars
            public void VarSet(string s) {
                string[] sp = s.Split(' ');
                SetVar(sp[1], s.Substring(sp[1].Length + 5));
            }

            public string VarFormater(string s) {
                if (s.Contains("_%")) {
                    if (s.Contains("%_")) {
                        string buffer = "";
                        string[] sp = s.Split("_%");
                        foreach (string se in sp) {
                            if (se.Contains("%_")) {
                                string[] sep = se.Split("%_");
                                string v = GetVar(sep[0]);
                                buffer += v + se.Substring(3 + sep[0].Length);
                            }
                            else {
                                if (se != "") {
                                    buffer += se;
                                }
                            }
                        }
                        return buffer;
                    }
                    else {
                        return s;
                    }
                }
                else {
                    return s;
                }
            }

            public void SetVar(string key, string var) {
                Vars[key] = var;
            }

            public string GetVar(string key) {
                return Vars[key];
            }

            // EnvVars
            public void EvarSet(string s) {
                string[] sp = s.Split(' ');
                Terminal.WriteLine(
                    $"&c%8%&[&c%9%&e&c%8%&]&c%7%&set ev&c%8%&[&c%f%&{sp[1]}&c%8%&]&c%7%&to&c%8%&[&c%f%&{s.Substring(sp[1].Length + 5)}&c%8%&]");
                SetEnvVar(sp[1], s.Substring(sp[1].Length + 5));
            }

            public void SetEnvVar(string key, string var) {
                EnvVars[key] = var;
            }

            public string GetEnvVar(string key) {
                return EnvVars[key];
            }

            // MakeCopy
            public void MakeCopyCrossroad(string s) {
                switch (s.Split(' ')[1]) {
                    case "ignore":
                        MakeCopyIgnoreCrossroad(s);
                        break;
                    case "make":
                        MakeCopyMake(GetEnvVar("project.rootdest"), GetEnvVar("project.cache"));
                        break;
                    case "cleanup":
                        MakeCopyCleanUp(GetEnvVar("project.cache"));
                        break;
                }
            }

            public void MakeCopyIgnoreCrossroad(string s) {
                switch (s.Split(' ')[2]) {
                    case "add":
                        MakeCopyIgnoreAdd(s.Substring(20));
                        break;
                    case "rmv":
                        MakeCopyIgnoreRemove(s.Substring(20));
                        break;
                    case "remove":
                        MakeCopyIgnoreRemove(s.Substring(23));
                        break;
                    case "show":
                        MakeCopyIgnoreShow();
                        break;
                }
            }

            public void MakeCopyIgnoreAdd(string rule) {
                MkCopyIgnores.Add(rule);
            }

            public void MakeCopyIgnoreRemove(string rule) {
                MkCopyIgnores.Remove(rule);
            }

            public void MakeCopyIgnoreShow() {
                Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%7%&Ignored:");
                foreach (string s in MkCopyIgnores) {
                    Terminal.WriteLine($"&c%8%& |[&c%7%&{s}&c%8%&]");
                }
                Terminal.WriteLine("&c%8%&[-]");
            }

            public void MakeCopyMake(string sourceFolder, string destFolder) {
                // init
                Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%7%&Resolving ignorations");
                List<string> ic = new List<string>();
                List<string> im = new List<string>();
                List<string> ip = new List<string>();
                foreach (string s in MkCopyIgnores) {
                    Terminal.WriteLine($"&c%8%& |&c%7%&Parsing&c%8%&[&c%f%&{s}&c%8%&]");
                    if (s.StartsWith("./")) {
                        ip.Add(s);
                    }
                    else if (s.Contains("@")) {
                        if (s.StartsWith("c@")) {
                            ic.Add(s.Substring(2));
                        }
                        else if (s.StartsWith("m@")) {
                            im.Add(s.Substring(2));
                        }
                        else if (s.StartsWith("p@")) {
                            ip.Add(s.Substring(2));
                        }
                    }
                    else if (s.StartsWith("*") | s.EndsWith("*")) {
                        ic.Add(s.Replace("*", ""));
                    }
                    else {
                        im.Add(s);
                    }
                }
                Terminal.WriteLine($"&c%8%&[-]&c%7%&Done!");

                #if DEBUG
                if (true) {
                    Terminal.WriteLine($"&c%d%&[D]Done!");
                    foreach (string s in ic) {
                        Terminal.WriteLine($"&c%d%&[D][ic]{s}");
                    }
                    foreach (string s in im) {
                        Terminal.WriteLine($"&c%d%&[D][im]{s}");
                    }
                    foreach (string s in ip) {
                        Terminal.WriteLine($"&c%d%&[D][ip]{s}");
                    }
                }
                #endif
                Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Start Copy!");
                MakeCopyCopy(sourceFolder, destFolder, ic, im, ip);
                Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Finished!");
            }

            public void MakeCopyCleanUp(string destFolder) {
                if (Directory.Exists(destFolder)) {
                    Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Cleaning...");
                    Directory.Delete(destFolder, true);
                }
                else {
                    Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Already Cleaned!");
                }
            }

            public void MakeCopyCopy(string sourceFolder, string destFolder, List<string>? icontain = null,
                List<string>? imatch = null, List<string>? ipath = null) {
                //Pre-Process
                icontain ??= new List<string>();
                imatch ??= new List<string>();
                ipath ??= new List<string>();
                bool isContain = icontain.Count > 0;
                if (!Directory.Exists(destFolder)) {
                    Directory.CreateDirectory(destFolder);
                }
                //Main
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files) {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name).Replace("\\", "/");
                    string src = Path.Combine(sourceFolder, name).Replace("\\", "/");
                    if ((!imatch.Contains(name)) & (!ipath.Contains(src))) {
                        if (isContain) {
                            bool allow = true;
                            foreach (string c in icontain) {
                                if (name.Contains(c)) {
                                    allow = false;
                                    break;
                                }
                            }
                            if (allow) {
                                Terminal.WriteLine(
                                    $"&c%8%&[&c%b%&c&c%8%&][&c%7%&F&c%8%&]&c%7%&{name}&c%8%&|{src} to {dest}");
                                File.Copy(file, dest);
                            }
                        }
                        else {
                            Terminal.WriteLine(
                                $"&c%8%&[&c%b%&c&c%8%&][&c%7%&F&c%8%&]&c%7%&{name}&c%8%&|{src} to {dest}");
                            File.Copy(file, dest);
                        }
                    }
                }
                string[] folders = Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders) {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name).Replace("\\", "/");
                    string src = Path.Combine(sourceFolder, name).Replace("\\", "/");
                    if ((!imatch.Contains(name)) & (!ipath.Contains(src))) {
                        if (isContain) {
                            bool allow = true;
                            foreach (string c in icontain) {
                                if (name.Contains(c)) {
                                    allow = false;
                                    break;
                                }
                            }
                            if (allow) {
                                Terminal.WriteLine(
                                    $"&c%8%&[&c%b%&c&c%8%&][&c%7%&D&c%8%&]&c%7%&{name}/&c%8%&|{src} to {dest}");
                                MakeCopyCopy(folder, dest, icontain, imatch, ipath);
                            }
                        }
                        else {
                            Terminal.WriteLine(
                                $"&c%8%&[&c%b%&c&c%8%&][&c%7%&D&c%8%&]&c%7%&{name}&c%8%&|{src} to {dest}");
                            MakeCopyCopy(folder, dest, icontain, imatch, ipath);
                        }
                    }
                }
            }

            //misc
            public void DeleteDo(string s) {
                Deleter(s.Substring(4));
            }

            public static void Deleter(string path) {
                if (File.GetAttributes(path) == FileAttributes.Directory) {
                    Directory.Delete(path, true);
                }
                else {
                    File.Delete(path);
                }
            }

            public void CopyCp(string[] ca) {
                if (ca.Length > 3) {
                    if (ca[3] == "carry") {
                        // init
                        Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%7%&Resolving ignorations");
                        List<string> ic = new List<string>();
                        List<string> im = new List<string>();
                        List<string> ip = new List<string>();
                        foreach (string s in MkCopyIgnores) {
                            Terminal.WriteLine($"&c%8%& |&c%7%&Parsing&c%8%&[&c%f%&{s}&c%8%&]");
                            if (s.StartsWith("./")) {
                                ip.Add(s);
                            }
                            else if (s.Contains("@")) {
                                if (s.StartsWith("c@")) {
                                    ic.Add(s.Substring(2));
                                }
                                else if (s.StartsWith("m@")) {
                                    im.Add(s.Substring(2));
                                }
                                else if (s.StartsWith("p@")) {
                                    ip.Add(s.Substring(2));
                                }
                            }
                            else if (s.StartsWith("*") | s.EndsWith("*")) {
                                ic.Add(s.Replace("*", ""));
                            }
                            else {
                                im.Add(s);
                            }
                        }
                        Terminal.WriteLine($"&c%8%&[-]&c%7%&Done!");

                        #if DEBUG
                        if (true) {
                            Terminal.WriteLine($"&c%d%&[D]Done!");
                            foreach (string s in ic) {
                                Terminal.WriteLine($"&c%d%&[D][ic]{s}");
                            }
                            foreach (string s in im) {
                                Terminal.WriteLine($"&c%d%&[D][im]{s}");
                            }
                            foreach (string s in ip) {
                                Terminal.WriteLine($"&c%d%&[D][ip]{s}");
                            }
                        }
                        #endif
                        Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Start Copy!");
                        MakeCopyCopy(ca[1], ca[2], ic, im, ip);
                        Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Done!");
                        return;
                    }
                }
                Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Start Copy!");
                MakeCopyCopy(ca[1], ca[2]);
                Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Done!");
            }
        }
    }
}