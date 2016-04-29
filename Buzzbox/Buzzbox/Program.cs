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

            //Only continue if commandline options fullfilled. CommandLine will handle helptext if something was off.
            if (commandLineResults)
            {
                
            }
        }
    }
}
