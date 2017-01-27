using System;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            ConsoleColor color;
            Enum.TryParse<ConsoleColor>(args[0], out color);

            var output = new string[args.Length - 1];
            Array.Copy(args, 1, output, 0, output.Length);

            Console.ForegroundColor = color;
            Console.WriteLine(string.Join(" ", output));
        }
        catch
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid Parameters.");
        }
        finally
        {
            Console.ResetColor();
        }
    }
}