using System.Collections.Concurrent;

namespace Baccon;

public class ActorFactory
{
    private ConcurrentDictionary<string, ActorInstance> _actors = new();
    
    public TContract GetActor<TContract, TActor>(string name) where TActor : class where TContract : class
    {
        if (!_actors.TryGetValue(name, out var instance))
        {
            instance = new ActorInstance<TContract, TActor>();
            _actors.TryAdd(name, instance);
        }
        var typedInstance = (ActorInstance<TContract, TActor>)instance;
        return typedInstance.GetActor();
    }
}