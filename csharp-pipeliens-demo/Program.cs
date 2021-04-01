using csharp_pipelines;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace csharp_pipeliens_demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = new ServiceCollection()
                .AddLogging(cfg =>
                {
                    cfg.AddConsole();
                })
                .BuildServiceProvider();

            Demo(provider).GetAwaiter().GetResult();
            Console.ReadKey();
        }

        static async Task Demo(IServiceProvider provider)
        {
            var context = new SampleContext()
            {
                WaitTime = 1000,
                PrintThis = 5
            };

            var logger = provider.GetService<ILogger<Pipeline<SampleContext>>>();

            var pipeline = new Pipeline<SampleContext>(context, logger);
            pipeline.AddBlock(new WaitableBlock());
            pipeline.AddBlock(new PrintableBlock());
            pipeline.AddBlock(new WaitableBlock());
            pipeline.AddBlock(new PrintableBlock());
            pipeline.AddBlock(new WaitableBlock());
            pipeline.AddBlock(new PrintableBlock());

            await pipeline.Execute(async (context, logger) =>
            {
                logger.LogInformation("Waiting in between Tasks");
                await Task.Delay(500);
            });
        }
    }

    public class WaitableBlock : AsyncBlock<SampleContext>
    {
        public override async Task Execute(SampleContext context, ILogger logger, CancellationToken? cancellationToken = null)
        {
            await Task.Delay(context.WaitTime);
        }

        public override bool ShouldExecuteContext(SampleContext context, ILogger logger)
        {
            return true;
        }
    }

    public class PrintableBlock : AsyncBlock<SampleContext>
    {
        public override async Task Execute(SampleContext context, ILogger logger, CancellationToken? cancellationToken = null)
        {
            Console.WriteLine(context.PrintThis);
        }

        public override bool ShouldExecuteContext(SampleContext context, ILogger logger)
        {
            return true;
        }
    }

    public class SampleContext : Context
    {
        public int PrintThis { get; set; }
        public int WaitTime { get; set; }
    }
}
