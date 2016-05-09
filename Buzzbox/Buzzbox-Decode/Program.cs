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
                Required = true,
                HelpText = "Path to input file to be Decoded.")]
            public string InputFile { get; set; }

            [Option('o', "output",
                HelpText = "Output file path.",
                DefaultValue = "output.json")]
            public string OutputFile { get; set; }

            [Option('e', "encoding",
                HelpText = "Which format to decode from.",
                DefaultValue = EncodingFormats.MtgFormat)]
            public EncodingFormats EncodingFormat { get; set; }

            [Option("set",
                HelpText = "The Set ( CORE, BRM, OG, etc) this card belongs too.",
                DefaultValue = "HS-RNN")]
            public string Set { get; set; }

            [Option("source",
                HelpText = "A costum atttribute, not in the hearhstoneapi json. Intended to store what generated the card.",
                DefaultValue = "hs-rnn")]
            public string Source { get; set; }


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
            var decode = new Decode(options.Set, options.Source);

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

                string inputData;

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

                var cardCollection = new CardCollection();

                //actually decode the text
                try
                {
                    cardCollection = decode.DecodeString(inputData, options.EncodingFormat);
                }
                catch (Exception)
                {
                    Console.WriteLine("Error while trying to decode '{0}'", inPath);
                    return;
                }

                //fail out if no cards are found
                if (cardCollection.Cards.Count == 0)
                {
                    Console.WriteLine("Did not find any card to save to file");
                    return;
                }

                //write output cards to file, report on the amount found.
                try
                {
                    Console.WriteLine("Found {0} cards in '{1}'.", cardCollection.Cards.Count, inPath);
                    cardCollection.Save(outPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not write to file '{0}': {1}", outPath, e.Message );
                }
            }
        }
    }
}
