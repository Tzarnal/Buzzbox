using System;
using System.IO;
using Buzzbox_Common;
using CommandLine;
using CommandLine.Text;



namespace Inflater
{
    class Program
    {
        //Command line options through CommandLine: http://commandline.codeplex.com/
        class Options
        {
            [Option('i',"input",Required = true, HelpText = "Path to input file to be inflated, must be in hearthstonejson format.")]
            public string InputFile { get; set; }

            [Option('o',"output",HelpText = "Output file path. Defaults to inflated.(inputFileName)")]
            public string OutputFile { get; set; }

            [Option('r',"rate", 
                HelpText = "Amount of inflation. Defaults to 3 times original.",
                DefaultValue = 3)]
            public int InflationRate { get; set; }

            [HelpOption]
            public string GetUsage()
            {
                return "Adds additional copies of cards into a hearthstonejson file and randomizes the order. \n\n" + 
                    HelpText.AutoBuild(this,
                  (current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static void Main(string[] args)
        {
            //Parse Commandline options
            var options = new Options();
            var commandLineResults = Parser.Default.ParseArguments(args, options);

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

                //Check output Path or build a default one if none is supplied
                if (string.IsNullOrWhiteSpace(options.OutputFile))
                {
                    outPath = Path.GetDirectoryName(inPath) + "/inflated." + Path.GetFileName(inPath);
                }
                else
                {                    
                    try
                    {
                        outPath = Path.GetFullPath(options.OutputFile);                        
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Invalid Path to output file: {0}", options.OutputFile);
                        return;
                    }
                }

                //Actually inflate and randomize the card collection
                var inflatedCollection = Inflate(inPath, options.InflationRate);

                //Write out again
                try
                {
                    inflatedCollection.Save(outPath);
                }
                catch (Exception)
                {
                    Console.WriteLine("Could not write output");
                }
            }
        }

        static CardCollection Inflate(string orignalCollectionPath, int rate)
        {
            var inflatedCollection = new CardCollection();
            var originalCollection = CardCollection.Load(orignalCollectionPath);

            for (var i = 0; i < rate; i++)
            {                
                originalCollection.Cards.Shuffle();
                inflatedCollection.Cards.AddRange(originalCollection.Cards);
            }

            return inflatedCollection;
        }
    }    
}
