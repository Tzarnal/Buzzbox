using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Buzzbox_Common;
using CommandLine;
using CommandLine.Text;

namespace Buzzbox_Stream
{
    internal class Program
    {
        private class Options
        {
            [Option('i', "input",
                Required = true,
                HelpText = "Path to input file to be Encoded, must be in hearthstonejson format or a simple text file.")
            ]
            public string InputFile { get; set; }

            [ValueList(typeof (List<string>))]
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

        private static CardCollection ReadCardCollection(string filePath)
        {
            //Load card collection to encode
            CardCollection cardCollection;
            try
            {
                cardCollection = CardCollection.Load(filePath);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not parse '{0}'.", filePath);
                return null;
            }

            return cardCollection;
        }

        private static List<string> ReadTextFile(string filePath)
        {
            List<string> fileLines;

            try
            {
                fileLines = File.ReadAllLines(filePath).ToList();
            }
            catch (Exception)
            {
                Console.WriteLine("Could not read '{0}'.", filePath);

                return null;
            }

            return fileLines;
        }

        private static void Main(string[] args)
        {
            var options = new Options();
            var commandLineResults = Parser.Default.ParseArguments(args, options);


            if (commandLineResults)
            {
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

                var fileExtension = Path.GetExtension(inPath);

                //Check its an input file we understand
                if (fileExtension != ".json" && fileExtension != ".txt")
                {
                    Console.WriteLine("Buzzbox-Stream currently only supports hearthstoneapi json and simple txt files.");
                    Console.WriteLine(Path.GetExtension(inPath));
                    return;
                }

                if (options.FdsList.Count == 0)
                {
                    Console.WriteLine("No filestream id's to write too. Closing.");
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
                    } //not linux
                    else
                    {
                        fileLoc = "file" + fd + ".txt";
                    }

                    var stream = new StreamWriter(fileLoc);
                    //Spawn a StreamText or SteamEncode
                    if (fileExtension == ".json")
                    {
                        var cardCollection = ReadCardCollection(inPath);

                        if (cardCollection == null)
                        {
                            return;
                        }

                        var streamEncoder = new StreamEncode(cardCollection, stream)
                        {
                            LoopForever = options.LoopForever,
                            ShuffleFields = options.ShuffleFields
                        };

                        new Thread(delegate()
                        {
                            streamEncoder.ThreadEntry();
                            countdownEvent.Signal();
                        }).Start();
                    }
                    else //Spawn a StreamText instead. 
                    {
                        var text = ReadTextFile(inPath);

                        var streamText = new StreamText(text, stream)
                        {
                            LoopForever = options.LoopForever
                        };

                        new Thread(delegate ()
                        {
                            streamText.ThreadEntry();
                            countdownEvent.Signal();
                        }).Start();
                    }

                }

                countdownEvent.Wait();
                Console.WriteLine("Final thread finished writing. Buzzbox-Stream exiting.");
            }

        }

    }
}
