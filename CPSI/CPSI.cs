using DawnUtils;
using System.Diagnostics;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CPSI
{
    public class CPScript
    {
        public static void Run(string[] script)
        {
            CPSRInstance instance = new CPSRInstance(script);
            instance.Start();
        }

        public static void StrRun(string scriptstr)
        {
            string[] script = scriptstr.Split("\r\n");
            Run(script);
        }

        public static void LoadRun(string path)
        {
            if (path != null)
            {
                try
                {
                    string[] script = File.ReadAllLines(path);
                    if (script != null)
                    {
                        Run(script);
                    }
                    else
                    {
                        Terminal.WriteLine("&c%f%&[&c%c%&!&c%f%&]&c%c%&null exception occured!");
                    }
                }
                catch (Exception e)
                {
                    Terminal.WriteLine("&c%8%&[&c%c%&!&c%8%&]&c%c%&Error & Stack trace:&c%6%&\r\n&c%6%&" + e.ToString());
                }
            }
        }

        public class CPSRInstance
        {
            private string[] Script;
            public Dictionary<string, string> Vars;
            public Dictionary<string, string> EnvVars;
            public List<string> MKCopyIgnores;
            public bool ShowState = true;


            public CPSRInstance(string[] s, string cache = "./cache", string outp = "./build", string rootp = "./src", string aname = "artifact")
            {
                Script = s;
                Vars = new Dictionary<string, string>();
                MKCopyIgnores = new List<string>();
                EnvVars = new Dictionary<string, string>();
                EnvVars["project.cache"] = cache;
                EnvVars["project.outdest"] = outp;
                EnvVars["project.rootdest"] = rootp;
                EnvVars["project.artifactName"] = aname;
            }

            public void Start()
            {
                Runner(Script);
            }

            public void Runner(string[] s)
            {
                //Loader
                int line = 0;
                foreach (string statement in s)
                {
                    line += 1;
                    if (!stateConductor(statement, line))
                    {
                        break;
                    }
                }
            }

            public bool stateConductor(string s, int line = 0)
            {
                try
                {
                    s = varFormater(s);
                    if (ShowState)
                    {
                        Terminal.WriteLine(string.Format("&c%8%&[&c%b%&{0}&c%8%&]&c%e%&{1}", line, s));
                    }
                    switch (s.Split(' ')[0])
                    {
                        case "makecopy":
                            makecopyCrossroad(s);
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
                        case "clsl":
                            runTask(s.Substring(5), false);
                            break;
                        case "load":
                            runTask(s.Substring(5), true);
                            break;
                        case "del":
                            deleteDo(s);
                            break;
                        case "var":
                            varSet(s);
                            break;
                        case "env":
                            evarSet(s);
                            break;
                        case "echo":
                            Terminal.WriteLine("&c%8%&[&c%7%&>&c%8%&]&c%r%&" + s.Substring(5));
                            break;
                        case "pkg":
                            pkgCrossroad(s);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception e)
                {
                    Terminal.WriteLine("&c%6%&[Line" + Convert.ToString(line) + "] Error & Stack trace:&c%6%&\r\n&c%6%&" + e.ToString());
                    return false;
                }
                return true;
            }

            // task & clsm
            public void runTask(string path, bool isCarry)
            {
                string[] s = File.ReadAllLines(path);
                if (isCarry)
                {
                    Runner(s);
                }
                else
                {
                    CPSRInstance i = new CPSRInstance(s);
                    i.Start();
                }
            }

            //Package
            public void pkgCrossroad(string s)
            {
                string[] ss = s.Split(' ');
                switch (ss[1])
                {
                    case "make":
                        switch (ss[2])
                        {
                            case "zip":
                                pkgMakeZip();
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }

            public void pkgMakeZip()
            {
                Terminal.WriteLine($"&c%8%&[&c%6%&z&c%8%&]&c%7%&Creating ZipPack...");
                if (!Directory.Exists(geteVar("project.outdest")))
                {
                    Directory.CreateDirectory(geteVar("project.outdest"));
                }
                pkgZipGenerator(geteVar("project.cache"),Path.Combine(geteVar("project.outdest"),varFormater(geteVar("project.artifactName")).Replace("\\","/")));
            }

            public void pkgZipGenerator(string target,string outpath)
            {
                Terminal.WriteLine($"&c%8%&[&c%6%&z&c%8%&]&c%7%&{outpath} &c%8%&from {target}");
                if (File.Exists(outpath))
                {
                    File.Delete(outpath);
                }
                ZipFile.CreateFromDirectory(target, outpath);
            }

            // Vars
            public void varSet(string s)
            {
                string[] sp = s.Split(' ');
                setVar(sp[1], s.Substring(sp[1].Length + 5));
            }

            public string varFormater(string s)
            {
                if (s.Contains("_%"))
                {
                    if (s.Contains("%_"))
                    {
                        string buffer = "";
                        string[] sp = s.Split("_%");
                        foreach (string se in sp)
                        {
                            if (se.Contains("%_"))
                            {
                                string[] sep = se.Split("%_");
                                string v = getVar(sep[0]);
                                buffer += v + se.Substring(3 + sep[0].Length);
                            }
                            else
                            {
                                if (se != "")
                                {
                                    buffer += se;
                                }
                            }
                        }
                        return buffer;
                    }
                    else
                    {
                        return s;
                    }
                }
                else
                {
                    return s;
                }
            }

            public void setVar(string key,string var)
            {
                Vars[key] = var;
            }

            public string getVar(string key)
            {
                return Vars[key]; 
            }

            // EnvVars
            public void evarSet(string s)
            {
                string[] sp = s.Split(' ');
                Terminal.WriteLine($"&c%8%&[&c%9%&e&c%8%&]&c%7%&set ev&c%8%&[&c%f%&{sp[1]}&c%8%&]&c%7%&to&c%8%&[&c%f%&{s.Substring(sp[1].Length + 5)}&c%8%&]");
                seteVar(sp[1], s.Substring(sp[1].Length + 5));
            }

            public void seteVar(string key, string var)
            {
                EnvVars[key] = var;
            }

            public string geteVar(string key)
            {
                return EnvVars[key];
            }

            // MakeCopy
            public void makecopyCrossroad(string s)
            {
                switch (s.Split(' ')[1])
                {
                    case "ignore":
                        makecopyIgnoreCrossroad(s);
                        break;
                    case "make":
                        makecopyMake(geteVar("project.rootdest"),geteVar("project.cache"));
                        break;
                    case "cleanup":
                        makecopyCleanUp(geteVar("project.cache"));
                        break;
                    default:
                        break;
                }
            }

            public void makecopyIgnoreCrossroad(string s)
            {
                switch (s.Split(' ')[2])
                {
                    case "add":
                        makecopyIgnoreAdd(s.Substring(20));
                        break;
                    case "rmv":
                        makecopyIgnoreRemove(s.Substring(20));
                        break;
                    case "remove":
                        makecopyIgnoreRemove(s.Substring(23));
                        break;
                    case "show":
                        makecopyIgnoreShow();
                        break;
                    default:
                        break;
                }
            }

            public void makecopyIgnoreAdd(string rule)
            {
                MKCopyIgnores.Add(rule);
            }

            public void makecopyIgnoreRemove(string rule)
            {
                MKCopyIgnores.Remove(rule);
            }

            public void makecopyIgnoreShow()
            {
                Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%7%&Ignored:");
                foreach (string s in MKCopyIgnores)
                {
                    Terminal.WriteLine($"&c%8%& |[&c%7%&{s}&c%8%&]");
                }
                Terminal.WriteLine("&c%8%&[-]");
            }

            public void makecopyMake(string sourceFolder, string destFolder)
            {
                // init
                Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%7%&Resolving ignorations");
                List<string> ic = new List<string>();
                List<string> im = new List<string>();
                List<string> ip = new List<string>();
                foreach (string s in MKCopyIgnores)
                {
                    Terminal.WriteLine($"&c%8%& |&c%7%&Parsing&c%8%&[&c%f%&{s}&c%8%&]");
                    if (s.StartsWith("./"))
                    {
                        ip.Add(s);
                    }
                    else if (s.Contains("@"))
                    {
                        if (s.StartsWith("c@"))
                        {
                            ic.Add(s.Substring(2));
                        }
                        else if (s.StartsWith("m@"))
                        {
                            im.Add(s.Substring(2));
                        }
                        else if (s.StartsWith("p@"))
                        {
                            ip.Add(s.Substring(2));
                        }
                    }
                    else if (s.StartsWith("*") | s.EndsWith("*"))
                    {
                        ic.Add(s.Replace("*",""));
                    }
                    else 
                    { 
                        im.Add(s);
                    }
                }
                Terminal.WriteLine($"&c%8%&[-]&c%7%&Done!");

                #if DEBUG
                if (true)
                {
                    Terminal.WriteLine($"&c%d%&[D]Done!");
                    foreach (string s in ic)
                    {
                        Terminal.WriteLine($"&c%d%&[D][ic]{s}");
                    }
                    foreach (string s in im)
                    {
                        Terminal.WriteLine($"&c%d%&[D][im]{s}");
                    }
                    foreach (string s in ip)
                    {
                        Terminal.WriteLine($"&c%d%&[D][ip]{s}");
                    }
                }
                #endif
                Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Start Copy!");
                makecopyCopy(sourceFolder, destFolder, ic, im, ip);
                Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Finished!");
            }

            public void makecopyCleanUp(string destFolder)
            {
                if (Directory.Exists(destFolder))
                {
                    Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Cleaning...");
                    Directory.Delete(destFolder, true);
                }
                else
                {
                    Terminal.WriteLine("&c%8%&[&c%b%&c&c%8%&]&c%f%&Already Cleaned!");
                }
            }

            public void makecopyCopy(string sourceFolder, string destFolder, List<string>? icontain = null, List<string>? imatch = null, List<string>? ipath = null)
            {
                //Pre-Process
                if (icontain == null)
                {
                    icontain = new List<string>();
                }
                if (imatch == null)
                {
                    imatch = new List<string>();
                }
                if (ipath == null)
                {
                    ipath = new List<string>();
                }
                bool isContain = false;
                if (icontain.Count > 0)
                {
                    isContain = true;
                }
                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }
                //Main
                string[] files = Directory.GetFiles(sourceFolder);
                foreach (string file in files)
                {
                    string name = Path.GetFileName(file);
                    string dest = Path.Combine(destFolder, name).Replace("\\", "/");
                    string src = Path.Combine(sourceFolder, name).Replace("\\", "/");
                    if ((!imatch.Contains(name)) & (!ipath.Contains(src)))
                    {
                        if (isContain)
                        {
                            bool allow = true;
                            foreach (string c in icontain)
                            {
                                if (name.Contains(c))
                                {
                                    allow = false;
                                    break;
                                }
                            }
                            if (allow)
                            {
                                Terminal.WriteLine($"&c%8%&[&c%b%&c&c%8%&][&c%7%&F&c%8%&]&c%7%&{name}&c%8%&|{src} to {dest}");
                                File.Copy(file, dest);
                            }
                        }
                        else
                        {
                            Terminal.WriteLine($"&c%8%&[&c%b%&c&c%8%&][&c%7%&F&c%8%&]&c%7%&{name}&c%8%&|{src} to {dest}");
                            File.Copy(file, dest);
                        }
                    }
                }
                string[] folders =  Directory.GetDirectories(sourceFolder);
                foreach (string folder in folders)
                {
                    string name = Path.GetFileName(folder);
                    string dest = Path.Combine(destFolder, name).Replace("\\","/");
                    string src = Path.Combine(sourceFolder, name).Replace("\\", "/");
                    if ((!imatch.Contains(name)) & (!ipath.Contains(src)))
                    {
                        if (isContain)
                        {
                            bool allow = true;
                            foreach (string c in icontain)
                            {
                                if (name.Contains(c))
                                {
                                    allow = false;
                                    break;
                                }
                            }
                            if (allow)
                            {
                                Terminal.WriteLine($"&c%8%&[&c%b%&c&c%8%&][&c%7%&D&c%8%&]&c%7%&{name}&c%8%&|{src} to {dest}");
                                makecopyCopy(folder, dest, icontain, imatch, ipath);
                            }
                        }
                        else
                        {
                            Terminal.WriteLine($"&c%8%&[&c%b%&c&c%8%&][&c%7%&D&c%8%&]&c%7%&{name}&c%8%&|{src} to {dest}");
                            makecopyCopy(folder, dest, icontain, imatch, ipath);
                        }
                    }
                }
            }

            //misc
            public void deleteDo(string s)
            {
                deletor(s.Substring(4));
            }
            public void deletor(string path)
            {
                if (File.GetAttributes(path) == FileAttributes.Directory)
                {
                    Directory.Delete(path, true);
                }
                else
                {
                    File.Delete(path);
                }
            }
        }
    }
}
