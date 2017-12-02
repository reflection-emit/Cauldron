using Cauldron;
using Cauldron.Consoles;
using Cauldron.Core.Diagnostics;
using System;
using System.Linq;

namespace Win32_Console_ParameterHandling
{
    public class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var parser = new ParameterParser(new MainExecutionGroup(), new EveMarketExecutionGroup());
                parser.Parse(args);

                if (!parser.Execute())
                    parser.ShowHelp();
            }
            catch (RequiredParametersMissingException requiredE)
            {
                Console.WriteLine("Required parameters were not set: " + requiredE.MissingRequiredParameters.Join(", "));
                Environment.Exit(3);
            }
            catch (UnknownParameterException unknownE)
            {
                Console.WriteLine("Unknown parameter: " + unknownE.Parameter);
                Environment.Exit(2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.GetStackTrace());
                Environment.Exit(-10);
            }
        }
    }
}