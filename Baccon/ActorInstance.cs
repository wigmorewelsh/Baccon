using Castle.DynamicProxy;

namespace Baccon;

public interface ActorInstance
{
    ActorScheduler Scheduler { get; }
    object Actor { get; }
}

public class ActorInstance<TActor> : ActorInstance where TActor : class
{
    public ActorScheduler Scheduler { get; }
    public object Actor { get; }

    private TActor Proxy { get; }

    public ActorInstance()
    {
        Scheduler = new ActorScheduler();
        WorkerTask = Scheduler.ExecuteTasks();
        var instance = Activator.CreateInstance<TActor>();
        Actor = instance;
        var interceptor = new ActorProxy(this);
        var generator = new ProxyGenerator();
        Proxy = generator.CreateClassProxy<TActor>(interceptor);
    }

    public Task WorkerTask { get; set; }

    public TActor GetActor()
    {
        return Proxy as TActor;
    }
}