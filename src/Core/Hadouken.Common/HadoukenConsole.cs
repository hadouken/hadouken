using System;

namespace Hadouken.Common {
    public class HadoukenConsole : IConsole {
        public void Write(string format, params object[] args) {
            Console.Write(format, args);
        }

        public void WriteLine(string format, params object[] args) {
            Console.WriteLine(format, args);
        }
    }
}