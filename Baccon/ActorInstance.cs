namespace Baccon;

public interface ActorInstance
{
    ActorScheduler Scheduler { get; }
    object Actor { get; }
}

public class ActorInstance<TContract, TActor> : ActorInstance where TActor : class where TContract : class
{
    public ActorScheduler Scheduler { get; }
    public object Actor { get; }

    private TContract Proxy { get; }

    public ActorInstance()
    {
        Scheduler = new ActorScheduler();
        WorkerTask = Scheduler.ExecuteTasks();
        var instance = Activator.CreateInstance<TActor>();
        Actor = instance;
        Proxy = ActorProxy.CreateActor<TContract>(this);
    }

    public Task WorkerTask { get; set; }

    public TContract GetActor()
    {
        return Proxy as TContract;
    }
}