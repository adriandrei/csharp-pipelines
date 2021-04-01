# csharp-pipelines

Define the Context, and the Asynchronous Blocks, configure your Pipeline and execute it.

Example:

```csharp
public class SampleContext : Context
{
    public int PrintThis { get; set; }
    public int WaitTime { get; set; }
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

var context = new SampleContext()
{
    WaitTime = 1000,
    PrintThis = 5
};

var pipeline = new Pipeline<SampleContext>(context, logger);
pipeline.AddBlock(new WaitableBlock());
pipeline.AddBlock(new PrintableBlock());

await pipeline.Execute();
```