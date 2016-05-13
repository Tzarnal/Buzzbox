using System;
using System.Collections.Generic;
using System.Text;

namespace Buzzbox_Common
{
    //Write things to the console based on what level of information the user wants to see.
    public sealed class ConsoleLog
    {
        public bool Verbose;
        public bool Silent;

        private static readonly ConsoleLog instance = new ConsoleLog();

        static ConsoleLog()
        {
        }

        private ConsoleLog()
        {
        }

        public void VerboseWriteLine(string text)
        {
            if(!Silent && Verbose)
                Console.WriteLine(text);
        }

        public void WriteLine(string text)
        {
            if(!Silent)
                Console.WriteLine(text);
        }

        public static ConsoleLog Instance
        {
            get
            {
                return instance;
            }
        }
    }
}
