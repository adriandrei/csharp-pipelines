namespace csharp_pipelines
{
    /// <summary>
    /// Starting from this we can attach any context which might need along the way in our pipeline.
    /// Starting point can be deserialized ServiceBus Message.
    /// </summary>
    public abstract class Context
    {
        public bool ShouldStop { get; protected set; }
    }
}
