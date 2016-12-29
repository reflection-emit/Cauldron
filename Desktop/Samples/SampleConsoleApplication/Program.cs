using Cauldron.Consoles;
using Cauldron.Core.Extensions;
using System;

namespace SampleConsoleApplication
{
    public class Program
    {
        public MainExecutionGroup MainGroup { get; private set; }

        private static void Main(string[] args)
        {
            try
            {
                var parser = new ParameterParser();
                parser.Parse(new Program(), args);
                parser.Execute();
            }
            catch (RequiredParametersMissingException requiredE)
            {
                Console.WriteLine("Required parameters were not set: " + string.Join(", ", requiredE.MissingRequiredParameters));
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

            Console.ReadKey();
        }
    }
}