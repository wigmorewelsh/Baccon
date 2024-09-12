namespace Baccon;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        var factory = new ActorFactory();
        var actor = factory.GetActor<ISomeActor, SomeActor>("SomeActor");
        await actor.RunThing();
        var result = await actor.ReturnInt();
        Console.WriteLine($"Result: {result}");
    }
}

public interface ISomeActor
{
    Task RunThing();
    Task<int> ReturnInt();
}

public class SomeActor : ISomeActor
{
    public async Task RunThing()
    {
        Console.WriteLine("Called Actor");
        Console.WriteLine($"In scheduler {TaskScheduler.Current}");
    }
    
    public async Task<int> ReturnInt()
    {
        Console.WriteLine("Called Actor ReturnInt");
        return 42;
    }
}