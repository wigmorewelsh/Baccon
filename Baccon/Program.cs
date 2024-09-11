namespace Baccon;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var factory = new ActorFactory();
        var actor = factory.GetActor<SomeActor>("SomeActor");
        await actor.RunThing();
    }
}

public class SomeActor
{
    public virtual async Task RunThing()
    {
        Console.WriteLine("Called Actor");
        Console.WriteLine($"In scheduler {TaskScheduler.Current}");
    }
}