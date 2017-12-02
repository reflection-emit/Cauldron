using System;
using Cauldron;
using Cauldron.Consoles;
using System.ComponentModel;
using System.ServiceProcess;
using Cauldron.WindowsService;

namespace @namespace
{
    [RunInstaller(true)]
    public class ServiceInstaller : WindowsServiceInstaller
    {
        public ServiceInstaller()
        {
            // this.UserName = "";
            // this.Password = null;
        }
    }

    public sealed class ServiceMain
    {
        private static void Main(string[] args)
        {
            var parser = new ParameterParser(new ServiceExecutionGroup<ServiceInstaller>());
            parser.DescriptionColor = ConsoleColor.Green;

            try
            {
                parser.Parse(args);
                if (!parser.Execute())
                    ServiceBase.Run(new Service());
            }
            catch (UnknownParameterException unknownex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(unknownex.Message + " Parameter: " + unknownex.Parameter);
                Console.ResetColor();

                Environment.Exit(4);
            }
            catch (RequiredValuesMissingException misingValuesex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(misingValuesex.Message + " " + misingValuesex.MissingRequiredParameters.Join(", "));
                Console.ResetColor();

                Environment.Exit(3);
            }
            catch (RequiredParametersMissingException misingex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(misingex.Message + " " + misingex.MissingRequiredParameters.Join(", "));
                Console.ResetColor();

                Environment.Exit(2);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e.Message);
                Console.ResetColor();

                Environment.Exit(1);
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}