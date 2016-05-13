using System;
using System.IO;
using Buzzbox_Common;
using CommandLine;
using CommandLine.Text;

namespace Buzzbox_Decode
{
    class Program
    {                
        //Command line options through CommandLine: http://commandline.codeplex.com/
        class Options
        {
            [Option('i', "input",
                HelpText = "Path to input file to be Decoded.")]
            public string InputFile { get; set; }

            [Option('o', "output",
                HelpText = "Output file path.",
                DefaultValue = "output.json")]
            public string OutputFile { get; set; }

            [Option('e', "encoding",
                HelpText = "Which format to decode from. Supported formats are scfdivineFormat and MtgEncoderFormat.",
                DefaultValue = EncodingFormats.MtgEncoderFormat)]
            public EncodingFormats EncodingFormat { get; set; }

            [Option("set",
                HelpText = "The Set ( CORE, BRM, OG, etc) this card belongs too.",
                DefaultValue = "HS-RNN")]
            public string Set { get; set; }

            [Option("source",
                HelpText = "A costum atttribute, not in the hearhstoneapi json. Intended to store what generated the card.",
                DefaultValue = "hs-rnn")]
            public string Source { get; set; }

            [Option("image",
                HelpText = "What image gets put on the card.",
                DefaultValue = "Default")]
            public string Texture { get; set; }

            [Option("simple-output", DefaultValue = false,
               HelpText = "Instead of json data the output file will be simple text details.")]
            public bool SimpleOutput { get; set; }

            [Option("verbose", DefaultValue = false,
               MutuallyExclusiveSet = "Verbosity",
               HelpText = "Output additional information while decoding.")]
            public bool Verbose { get; set; }

            [Option("silent", DefaultValue = false,
               MutuallyExclusiveSet = "Verbosity",
               HelpText = "Never output anything but error messages.")]
            public bool Silent { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return "Decodes mtg-rnn output. Into a hearthstone api json file. \n\n" +
                    HelpText.AutoBuild(this,
                  (current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        private static void Main(string[] args)
        {            
            var options = new Options();
            var commandLineResults = Parser.Default.ParseArguments(args, options);
            var decode = new Decode(options.Set, options.Source, options.Texture);

            if (commandLineResults)
            {
                string outPath;
                string inPath;

                string inputData = "";

                var _ConsoleLog = ConsoleLog.Instance;
                _ConsoleLog.Verbose = options.Verbose;
                _ConsoleLog.Silent = options.Silent;

                //Use Path to get proper filesystem path for output
                try
                {                    
                    outPath = Path.GetFullPath(options.OutputFile);
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid Path to output file: {0}", options.OutputFile);
                    return;
                }

                if (Console.IsInputRedirected)
                {
                    //read from stdin
                    while (Console.In.Peek() != -1)
                    {
                        inputData += Console.ReadLine() + "\n\n";
                    }

                    //decode and abort early.
                    DecodeString(inputData, outPath, decode, options);
                    return;
                }

                if (string.IsNullOrWhiteSpace(options.InputFile))
                {
                    Console.WriteLine("No input file or input stream supplied. One or the other is required. ");                    
                    return;
                }

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

                //Read input file
                try
                {
                    inputData = File.ReadAllText(inPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error while trying to read from '{0}' : {1}", inPath, e.Message);
                    return;
                }

                if (!string.IsNullOrWhiteSpace(inputData))
                {
                    DecodeString(inputData, outPath, decode, options);
                }
                else
                {
                    Console.WriteLine("Input file was empty.");
                }
            }            
        }

        private static void DecodeString(string inputData, string outPath, Decode decode, Options options)
        {
            var _ConsoleLog = ConsoleLog.Instance;
            var cardCollection = new CardCollection();

            //actually decode the text
            try
            {
                cardCollection = decode.DecodeString(inputData, options.EncodingFormat);
            }
            catch (Exception)
            {
                Console.WriteLine("Error while trying to decode input.");
                return;
            }

            //fail out if no cards are found
            if (cardCollection.Cards.Count == 0)
            {
                Console.WriteLine("Did not find any card to save to file");
                return;
            }

            if (options.SimpleOutput)
            {
                StreamWriter file;
                
                //write output as simple .tostring data.
                try
                {
                    file = new StreamWriter(outPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not write to file '{0}': {1}", outPath, e.Message);
                    return;
                }

                foreach (var card in cardCollection.Cards)
                {
                    file.WriteLine(card.ToString());
                }
            }
            else
            {
                //write output cards to file as json data
                try
                {                    
                    cardCollection.Save(outPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not write to file '{0}': {1}", outPath, e.Message);
                }
            }

            _ConsoleLog.WriteLine($"Found {decode.ActualCards} cards out of a potential {decode.PotentialCards}.");
            _ConsoleLog.WriteLine(cardCollection.CollectionStats());
        }
    }
}
