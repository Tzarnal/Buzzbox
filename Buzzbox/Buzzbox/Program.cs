using System;
using System.IO;
using Buzzbox_Common;
using CommandLine;
using CommandLine.Text;

namespace Buzzbox
{
    class Program
    {

        //Command line options through CommandLine: http://commandline.codeplex.com/
        class Options
        {
            [Option('i', "input", 
                Required = true, 
                HelpText = "Path to input file to be Encoded, must be in hearthstonejson format.")]
            public string InputFile { get; set; }

            [Option('o', "output",
                HelpText = "Output file path.",
                DefaultValue = "output.txt")]
            public string OutputFile { get; set; }

            [Option('e', "encoding",
                HelpText = "Which encoding format to use.",
                DefaultValue = EncodingFormats.MtgFormat)]
            public EncodingFormats EncodingFormat { get; set; }

            [Option("shuffle-fields", DefaultValue = false,
                HelpText = "Shuffles the fields of the output in supported Encoding Formats")]
            public bool ShuffleFields { get; set; }

            [Option("shuffle-cards", DefaultValue = false,
                HelpText = "Shuffles the the cards, randomizing the order of output.")]
            public bool ShuffleCards { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return "Encodes a hearthstonejson file to a mtg-rnn usable corpus txt file. \n\n" + 
                    HelpText.AutoBuild(this,
                  (current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        private static void Main(string[] args)
        {
            //Parse Commandline options
            var options = new Options();
            var commandLineResults = Parser.Default.ParseArguments(args, options);
            var encode = new Encode
            {
                ShuffleFields = options.ShuffleFields
            };

            
            //Only continue if commandline options fullfilled. CommandLine will handle helptext if something was off.
            if (commandLineResults)
            {
                string outPath;
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

                //Load card collection to encode
                CardCollection cardCollection;
                try
                {
                    cardCollection = CardCollection.Load(inPath);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not parse '{0}'.",inPath);
                    return;
                }

                //Shuffle the cards, if that option is set
                if (options.ShuffleCards)
                {
                    cardCollection.Cards.Shuffle();
                }

                //Actually encode.
                string output;
                try
                {
                    output = encode.EncodeCardCollection(cardCollection, options.EncodingFormat);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error while trying to encode {0} using {1}.", inPath, options.EncodingFormat);
                    return;
                }
                

                //Write out again
                try
                {
                    File.WriteAllText(outPath, output);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not write output to {0}", outPath);
                }

            }
        }
    }
}
