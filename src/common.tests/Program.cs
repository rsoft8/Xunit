using System;

namespace Xunit
{
    public class Program
    {
        // TODO: This should be (optionally?) auto-injected
        [STAThread]
        public static int Main(string[] args)
            => ConsoleRunner.Run(args);
    }
}
