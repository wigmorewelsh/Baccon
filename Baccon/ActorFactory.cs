using System.Collections.Concurrent;

namespace Baccon;

public class ActorFactory
{
    private ConcurrentDictionary<string, ActorInstance> _actors = new();
    
    public TActor GetActor<TActor>(string name) where TActor : class
    {
        if (!_actors.TryGetValue(name, out var instance))
        {
            instance = new ActorInstance<TActor>();
            _actors.TryAdd(name, instance);
        }
        var typedInstance = (ActorInstance<TActor>)instance;
        return typedInstance.GetActor();
    }
}