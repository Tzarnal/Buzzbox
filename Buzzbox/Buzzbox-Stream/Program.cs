using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Buzzbox_Common;
using CommandLine;
using CommandLine.Text;

namespace Buzzbox_Stream
{
    class Program
    {
        private class Options
        {
            [Option('i', "input",
                Required = true,
                HelpText = "Path to input file to be Encoded, must be in hearthstonejson format.")]
            public string InputFile { get; set; }

            [ValueList(typeof(List<string>))]
            public IList<string> FdsList { get; set; }

            [Option("loop-forever", DefaultValue = false,
                HelpText = "When the end of a card set is reached, shuffle it and start again.")]
            public bool LoopForever { get; set; }

            [Option("shuffle-fields", DefaultValue = false,
                HelpText = "Shuffles the fields of the output.")]
            public bool ShuffleFields { get; set; }

            [Option("verbose", DefaultValue = false,
               MutuallyExclusiveSet = "Verbosity",
               HelpText = "Output additional information. Exclusive with the --silent option.")]
            public bool Verbose { get; set; }

            [Option("silent", DefaultValue = false,
               MutuallyExclusiveSet = "Verbosity",
               HelpText = "Never output anything but error messages. Exclusive with the --verbose option.")]
            public bool Silent { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return "Streams encoded card lines to torch-rss" +
                    HelpText.AutoBuild(this,
                  (current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }


        private static void Main(string[] args)
        {
            var options = new Options();
            var commandLineResults = Parser.Default.ParseArguments(args, options);
            

            if (commandLineResults)
            {
                List<StreamWriter> streams = new List<StreamWriter>();
                var countdownEvent = new CountdownEvent(options.FdsList.Count);
                string inPath;

                var _ConsoleLog = ConsoleLog.Instance;
                _ConsoleLog.Verbose = options.Verbose;
                _ConsoleLog.Silent = options.Silent;

                //Use Path to get proper filesystem path for input
                try
                {
                    inPath = Path.GetFullPath(options.InputFile);
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid Path to input file: {0}", options.InputFile);
                    return;
                }
                
                //Check if input file is real.
                if (!File.Exists(inPath))
                {
                    Console.WriteLine("File does not exist: {0}", options.InputFile);
                    return;
                }

                foreach (var fd in options.FdsList)
                {
                    OperatingSystem os = Environment.OSVersion;
                    PlatformID pid = os.Platform;

                    string fileLoc = "";

                    //on linux/unix platforms
                    if (pid == PlatformID.Unix)
                    {
                        fileLoc = "/proc/self/fd/" + fd;
                    }//not linux
                    else
                    {
                        fileLoc = "file" + fd + ".txt";    
                    }

                    var stream = new StreamWriter(fileLoc);

                    //Load card collection to encode
                    CardCollection cardCollection;
                    try
                    {
                        cardCollection = CardCollection.Load(inPath);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Could not parse '{0}'.", inPath);
                        return;
                    }

                    var streamEncoder = new StreamEncode(cardCollection,stream)
                    {
                        LoopForever = options.LoopForever,
                        ShuffleFields = options.ShuffleFields
                    };

                    new Thread(delegate ()
                    {
                        streamEncoder.ThreadEntry();
                        countdownEvent.Signal();
                    }).Start();
                }

                countdownEvent.Wait();
                Console.WriteLine("Final thread finished writing. Buzzbox-Stream exiting.");
            }
        }
    }
}
