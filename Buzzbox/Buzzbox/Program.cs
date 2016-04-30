using System;
using System.ComponentModel;
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
                HelpText = "Output file path. Defaults to output.txt",
                DefaultValue = "output.txt")]
            public string OutputFile { get; set; }

            [Option('e', "encoding",
                HelpText = "Which encoding format to use.",
                DefaultValue = EncodingFormats.scfdivineFormat)]
            public EncodingFormats EncodingFormat { get; set; }

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
            var encode = new Encode();

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
                    Console.WriteLine("Could parse '{0}'.",inPath);
                    return;
                }

                var output = encode.EncodeCardCollection(cardCollection, options.EncodingFormat);

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
