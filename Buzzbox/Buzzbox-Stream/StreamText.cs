using System;
using System.Collections.Generic;
using System.IO;
using Buzzbox_Common;

namespace Buzzbox_Stream
{
    internal class StreamText
    {
        public bool LoopForever;

        private StreamWriter _stream;
        private ConsoleLog _consoleLog;
        private List<string> _text;
     
        public StreamText(List<string> text, StreamWriter stream)
        {
            _consoleLog = ConsoleLog.Instance;
            _stream = stream;
            _text = text;
        }

        public void ThreadEntry()
        {
            try
            {
                do
                {
                    _text.Shuffle();

                    foreach (var line in _text)
                    {
                        //actually try to output
                        if(!string.IsNullOrWhiteSpace(line))
                            _stream.Write(line + "\n\n");

                    }

                } while (LoopForever);
            }
            catch (Exception e)
            {
                _consoleLog.WriteLine("Closing StreamText Thread, stream no longer available.");
                return;
            }

            _stream.Close();
        }
    }
}
