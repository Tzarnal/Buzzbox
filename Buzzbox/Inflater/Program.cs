using CommandLine;
using CommandLine.Text;

namespace Inflater
{
    class Program
    {
        class Options
        {
            [Option('i',"input",Required = true, HelpText = "Input file to be inflated, must be in api.hearthstonejson format.")]
            public string InputFile { get; set; }

            [Option('o',"output",HelpText = "Output file. Defaults to inflated.(inputFileName)")]
            public string OutputFile { get; set; }

            [Option('r',"rate", HelpText = "Amount of inflation. Defaults to 3 times original.")]
            public int InflationRate { get; set; } = 3;

            [HelpOption]
            public string GetUsage()
            {
                return HelpText.AutoBuild(this,
                  (current) => HelpText.DefaultParsingErrorsHandler(this, current));
            }
        }

        static void Main(string[] args)
        {
            var options = new Options();
            var commandLineResults = Parser.Default.ParseArguments(args, options);

            if (commandLineResults)
            {
                
            }
        }
    }
}
