namespace csharp_pipelines
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The main pipeline with which the client can interact.
    /// </summary>
    /// <typeparam name="T">The Context</typeparam>
    public class Pipeline<T> where T : Context, new()
    {
        private readonly List<AsyncBlock<T>> pipeline = new List<AsyncBlock<T>>();
        private readonly T context;
        private readonly ILogger logger;

        public Pipeline(T context, ILogger logger)
        {
            this.context = context;
            this.logger = logger;
        }

        /// <summary>
        /// Adds a new Block to the pipeline to be executed.
        /// </summary>
        /// <param name="block">The Block</param>
        /// <remarks>Order matters!</remarks>
        public void AddBlock(AsyncBlock<T> block)
        {
            this.pipeline.Add(block);
        }

        /// <summary>
        /// Executes the pipeline.
        /// </summary>
        /// <param name="executeBetweenSteps">Optional delegate to be executed in between steps.</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>A Task which encapsulates the asynchronous work to execute the Pipeline</returns>
        public async Task Execute(
            Func<T, ILogger, Task> executeBetweenSteps = null,
            CancellationToken? cancellationToken = null)
        {
            var totalCount = pipeline.Count;
            for (int i = 0; i < totalCount; i++)
            {
                var block = pipeline[i];

                if (!block.ShouldExecuteContext(this.context, this.logger))
                {
                    this.logger.LogInformation($"Block {i + 1} out of {totalCount}: {block.GetType().Name} cannot execute context. Aborting.");
                    return;
                }

                this.logger.LogInformation($"Entering block {i + 1} out of {totalCount}: {block.GetType().Name}");
                await block.Execute(this.context, this.logger, cancellationToken);
                this.logger.LogInformation($"Exited block {i + 1} out of {totalCount}: {block.GetType().Name}");

                if (i != totalCount - 1)
                {
                    T result = new T();
                    await executeBetweenSteps(result, this.logger);
                }

                if (this.context.ShouldStop)
                {
                    return;
                }
            }
        }
    }
}
