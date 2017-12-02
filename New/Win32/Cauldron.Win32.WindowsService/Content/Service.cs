using System.ServiceProcess;

namespace @namespace
{
    public class Service : ServiceBase
    {
        public Service()
        {
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override void OnPause()
        {
            base.OnPause();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            base.OnStop();
        }
    }
}