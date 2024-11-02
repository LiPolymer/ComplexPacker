using System.Text.RegularExpressions;

namespace DawnUtils {
    public static class Terminal {
        public static void Write(string msg) {
            if (msg.Contains("&c%") & msg.Contains("%&")) {
                string[] msgp = msg.Split("&c%");
                ConsoleColor bc = Console.ForegroundColor;
                foreach (string s in msgp) {
                    if (s.Length != 0) {
                        if (s.Substring(0, 3) == "r%&") {
                            Console.ForegroundColor = bc;
                            string slice = s.Remove(0, 3);
                            Console.Write(slice);
                        }
                        else if (s.Substring(1, 2) == "%&") {
                            string clr = s.Substring(0, 1);
                            string slice = s.Remove(0, 3);
                            Console.ForegroundColor = GetColor(clr);
                            Console.Write(slice);
                        }
                        else {
                            Console.Write("&c%" + s);
                        }
                    }
                }
                Console.ForegroundColor = bc;
            }
            else {
                Console.Write(msg);
            }
        }

        public static string RemoveTags(string msg) {
            if (msg.Contains("&c%") & msg.Contains("%&")) {
                string[] msgp = msg.Split("&c%");
                string result = string.Empty;
                foreach (string s in msgp) {
                    if (s.Length != 0) {
                        if (s.Substring(1, 2) == "%&") {
                            string slice = s.Remove(0, 3);
                            result = result + slice;
                        }
                        else {
                            result = result + "&c%" + s;
                        }
                    }
                }
                return result;
            }
            else {
                return msg;
            }
        }

        public static void WriteLine(string msg) {
            Write(msg);
            Console.Write("\r\n");
        }

        private static ConsoleColor GetColor(string color) {
            switch (color) {
                case "0":
                    return ConsoleColor.Black;
                case "1":
                    return ConsoleColor.DarkBlue;
                case "2":
                    return ConsoleColor.DarkGreen;
                case "3":
                    return ConsoleColor.DarkCyan;
                case "4":
                    return ConsoleColor.DarkRed;
                case "5":
                    return ConsoleColor.DarkMagenta;
                case "6":
                    return ConsoleColor.DarkYellow;
                case "7":
                    return ConsoleColor.Gray;
                case "8":
                    return ConsoleColor.DarkGray;
                case "9":
                    return ConsoleColor.Blue;
                case "a":
                    return ConsoleColor.Green;
                case "b":
                    return ConsoleColor.Cyan;
                case "c":
                    return ConsoleColor.Red;
                case "d":
                    return ConsoleColor.Magenta;
                case "e":
                    return ConsoleColor.Yellow;
                case "f":
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.White;
            }
        }

        public static void DeColorfulBug() {
            WriteLine("&c%3%&Dawn&c%7%&.&c%2%&Util&c%7%&.&c%e%&Terminal&c%f%&(); &c%8%&v1");
            WriteLine("&c%6%&Use &c%e%&&c%<ColorTag>%& &c%6%&to make terminal more &c%5%&colorful.");
            WriteLine("&c%8%&=[&c%7%&ColorTags&c%8%&]=================================");
            WriteLine(
                "&c%1%&[1]&c%2%&[2]&c%3%&[3]&c%4%&[4]&c%5%&[5]&c%6%&[6]&c%7%&[7]&c%8%&[8]&c%9%&[9]&c%a%&[a]&c%b%&[b]&c%c%&[c]&c%d%&[d]&c%e%&[e]&c%f%&[f]");
            WriteLine("&c%8%&=============================================");
            Console.WriteLine(RemoveTags("&c%3%&Dawn&c%7%&.&c%2%&Util&c%7%&.&c%e%&Terminal&c%f%&(); &c%8%&v1"));
        }
    }

    public class Resolver {
        public static string[] ResolveArgs(string st) {
            return Regex.Matches(st, @"""([^""]*)""|(\S+)")
                .Select(i => i.Value)
                .ToArray();
        }
    }
}