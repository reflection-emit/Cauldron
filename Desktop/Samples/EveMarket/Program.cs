using System;

namespace EveMarket
{
    public static class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                var app = new App();
                app.Run();
            }
            catch
            {
                throw;
            }
        }
    }
}