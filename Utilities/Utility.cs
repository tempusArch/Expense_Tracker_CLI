using System.Text.RegularExpressions;

namespace tracker.Utilities {
    public static class Utility {
        public static void PrintInfoMessage(string m) {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n" + m);
            Console.ResetColor();
        }

        public static void PrintHelpMessage(string m) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n" + m);
            Console.ResetColor();
        }

        public static void PrintErrorMessage(string m) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n" + m);
            Console.ResetColor();
        }

        public static void PrintCommandMessage(string m) {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("\n" + m + "\n");
            Console.ResetColor();
        }


        public static void PrintNumberMessage(string m) {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(m);
            Console.ResetColor();
        }

        public static List<string> ParseInput(string input) {
            var commandArgs = new List<string>();

            // Regex to match arguments, including those inside quotes
            var regex = new Regex(@"[\""].+?[\""]|[^ ]+");
            var matches = regex.Matches(input);

            foreach (Match i in matches) {
                // Remove surrounding quotes if any
                string value = i.Value.Trim('"');
                commandArgs.Add(value);
            }

            return commandArgs;
        }

        public static void ClearConsole() {
            Console.Clear(); 
        }
    }
}