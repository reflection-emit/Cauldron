using System.Threading;
using System.Threading.Tasks;

namespace Cauldron.Interception.Cecilator
{
    internal sealed class CecilatorCancellationToken
    {
        private static CecilatorCancellationToken cecilatorCancellation;
        private static object syncObject = new object();

        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public static CecilatorCancellationToken Current
        {
            get
            {
                if (cecilatorCancellation == null)
                {
                    lock (syncObject)
                    {
                        cecilatorCancellation = new CecilatorCancellationToken();
                    }
                }

                return cecilatorCancellation;
            }
        }

        public bool IsCancellationRequested => this.tokenSource.IsCancellationRequested;

        public static implicit operator CancellationToken(CecilatorCancellationToken token) => token.tokenSource.Token;

        public static implicit operator CancellationTokenSource(CecilatorCancellationToken token) => token.tokenSource;

        public static implicit operator ParallelOptions(CecilatorCancellationToken token) => new ParallelOptions
        {
            CancellationToken = token.tokenSource.Token,
            MaxDegreeOfParallelism = System.Environment.ProcessorCount * 2
        };

        public void Cancel() => this.tokenSource.Cancel();

        public void ThrowIfCancellationRequested() => this.tokenSource.Token.ThrowIfCancellationRequested();
    }
}