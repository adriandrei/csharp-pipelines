namespace csharp_pipelines
{
    using Microsoft.Extensions.Logging;
    using System.Threading;
    using System.Threading.Tasks;
    /// <summary>
    /// The base Block class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class AsyncBlock<T> where T : Context
    {
        protected AsyncBlock()
        {
        }

        public abstract bool ShouldExecuteContext(T context, ILogger logger);
        public abstract Task Execute(T context, ILogger logger, CancellationToken? cancellationToken = null);
    }
}
