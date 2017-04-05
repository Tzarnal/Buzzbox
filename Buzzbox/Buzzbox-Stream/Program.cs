using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
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
                MutuallyExclusiveSet = "input",
                HelpText = "Path to input file to be streamed, must be in hearthstonejson format or a simple text file. Exclusive with input-directory.")
            ]
            public string InputFile { get; set; }

            [Option('d', "input-directory",                
                MutuallyExclusiveSet = "input",
                HelpText = "Path to director of files ot be streamed, must be in hearthstonejson format or a simple text file. Exclusive with file input.")
            ]
            public string InputDirectory { get; set; }

            [ValueList(typeof (List<string>))]
            public IList<string> FdsList { get; set; }

            [Option("loop-data", DefaultValue = false,
                HelpText = "Keep streaming data untill the stream is closed by torch-rnn.")]
            public bool LoopForever { get; set; }

            [Option("shuffle", DefaultValue = false,
                MutuallyExclusiveSet = "shuffle",
                HelpText = "Shuffles the fields of the output if the input is json data. Exclusive with the --alternate-shuffling option.")]
            public bool ShuffleFields { get; set; }

            [Option("alternate-shuffling", DefaultValue = false,
                MutuallyExclusiveSet = "shuffle",
                HelpText = "Streams two copies of all input, if they are json data only one of the copies will be shuffled. Exclusive with the --shuffle option.")]
            public bool AlternateShuffleFields { get; set; }

            [Option("flavor-text", DefaultValue = false,
                HelpText = "Include flavortext field.")]
            public bool FlavorText { get; set; }

            [Option("name-replacement",
                HelpText = "Replace some cardnames with different ones from a file.")]
            public string NameReplacement { get; set; }

            [Option("name-replacement-chance", DefaultValue = 50,
                HelpText = "Chance name replacement will occur, out of a hundred.")]
            public int NameReplacementChance { get; set; }

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
                return "Streams encoded card lines to torch-rss.\n\n" +
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
                var consoleLog = ConsoleLog.Instance;
                consoleLog.Verbose = options.Verbose;
                consoleLog.Silent = options.Silent;

                if (!string.IsNullOrWhiteSpace(options.InputFile))
                {
                    SingleFileInput(options);
                }
                else if (!string.IsNullOrWhiteSpace(options.InputDirectory))
                {
                    DirectoryInput(options);
                }
                else
                {
                    Console.WriteLine("One of --input or --input-directory is required.");
                    Console.Write(options.GetUsage());
                }                                
            }
        }

        private static void DirectoryInput(Options options)
        {           
            string inPath;

            //Use Path to get proper filesystem path for input
            try
            {
                inPath = Path.GetFullPath(options.InputDirectory);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Path to input directory: {0}", options.InputFile);
                return;
            }

            if (!Directory.Exists(inPath))
            {
                Console.WriteLine("Directory '{0}' does not exists.", inPath);
                return;
            }

            var jsonFiles = Directory.GetFiles(inPath, "*.json");
            var txtFiles = Directory.GetFiles(inPath, "*.txt");
            var allFiles = jsonFiles.Concat(txtFiles).ToArray();
                         
            if (!allFiles.Any())
            {
                Console.WriteLine("Found no .json or .txt files in '{0}'", inPath);
                return;
            }

            DispatchStreams(options, allFiles);            
        }

        private static void SingleFileInput(Options options)
        {
            string inPath;
            
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
                Console.WriteLine("Buzzbox-Stream only supports hearthstoneapi json and simple txt files.");
                Console.WriteLine(Path.GetExtension(inPath));
                return;
            }

            DispatchStreams(options, new[] {inPath});
        }

        private static void DispatchStreams(Options options, string[] files)
        {
            var countdownEvent = new CountdownEvent(options.FdsList.Count);
            var consoleLog = ConsoleLog.Instance;
            var nameReplacement = false;
            NameCollection nameCollection;

            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            int fileIndex = 0;

            //Check Name Replacement details
            if (!string.IsNullOrWhiteSpace(options.NameReplacement))
            {
                nameCollection = ReadNameCollection(options.NameReplacement);

                if (nameCollection == null)
                {
                    Console.WriteLine("Running without Name Replacement.");                    
                }
                else
                {
                    nameReplacement = true;
                    nameCollection.ReplacementChance = options.NameReplacementChance;
                }
            }
            else
            {
                nameCollection = null;
            }

            //double the array and then sort it, this will make --alternate-sorting easier.
            if (options.AlternateShuffleFields)
            {
                files = files.Concat(files).ToArray();
                Array.Sort(files);
            }
            
            if (options.FdsList.Count == 0)
            {
                Console.WriteLine("No filestream id's to write too. Closing.");
                return;
            }

            foreach (var fd in options.FdsList)
            {
                string streamPath;

                //on linux/unix platforms
                if (pid == PlatformID.Unix)
                {
                    streamPath = "/proc/self/fd/" + fd;
                } //not linux
                else
                {
                    streamPath = "file" + fd + ".txt";
                }

                var stream = new StreamWriter(streamPath);

                var filePath = files[fileIndex];
                var fileExtension = Path.GetExtension(filePath);

                if (fileExtension == ".json")
                {
                    var cardCollection = ReadCardCollection(filePath);

                    if (cardCollection == null)
                    {
                        return;
                    }

                    bool shuffleFields;
                    if (options.AlternateShuffleFields)
                    {
                        //when alternate shuffling is on there are two copies of each file
                        //in the array, shuffle one, do not shuffle the other.
                        shuffleFields = (fileIndex%2 == 1);
                    }
                    else //not alternating shuffeling, just copy the option value then
                    {
                        shuffleFields = options.ShuffleFields;
                    }

                    var streamEncoder = new StreamEncode(cardCollection, stream)
                    {
                        LoopForever = options.LoopForever,
                        ShuffleFields = shuffleFields,
                        IncludeFlavorText = options.FlavorText,
                        NameReplacement = nameReplacement,
                        NameCollection =  nameCollection,
                    };

                    new Thread(delegate ()
                    {
                        consoleLog.VerboseWriteLine($"Streaming '{filePath}' to '{streamPath}'. Shuffeled = {shuffleFields}.");
                        streamEncoder.ThreadEntry();
                        consoleLog.VerboseWriteLine($"Stream to '{streamPath}' ended.");
                        countdownEvent.Signal();
                    }).Start();
                }
                else //Spawn a StreamText instead. 
                {
                    var text = ReadTextFile(filePath);

                    var streamText = new StreamText(text, stream)
                    {
                        LoopForever = options.LoopForever
                        
                    };

                    new Thread(delegate ()
                    {
                        consoleLog.VerboseWriteLine($"Streaming '{filePath}' to '{streamPath}'.");                    
                        streamText.ThreadEntry();
                        consoleLog.VerboseWriteLine($"Stream to '{streamPath}' ended.");
                        countdownEvent.Signal();
                    }).Start();
                }

                fileIndex++;
                if (fileIndex >= files.Length)
                {
                    fileIndex = 0;
                }
            }

            countdownEvent.Wait();
            Console.WriteLine("Final thread closed. Buzzbox-Stream exiting.");
        }

        private static NameCollection ReadNameCollection(string filePath)
        {
            //Use Path to get proper filesystem path for input
            try
            {
                filePath = Path.GetFullPath(filePath);
            }
            catch (Exception)
            {
                Console.WriteLine("Invalid Path to input file: {0}", filePath);
                return null;
            }

            //Check if input file is real.
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File does not exist: {0}", filePath);
                return null;
            }

            var fileExtension = Path.GetExtension(filePath);

            //Check its an input file we understand
            if (fileExtension != ".json")
            {
                Console.WriteLine("Name replacement file not json.");                                
            }

            var nameCollection = new NameCollection();
            try
            {
                nameCollection = NameCollection.Load(filePath);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not parse '{0}'.", filePath);
                return null;
            }

            return nameCollection;
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
    }
}
