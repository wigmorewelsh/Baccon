using Castle.DynamicProxy;

namespace Baccon;

public class ActorProxy : IAsyncInterceptor
{
    private readonly ActorInstance _instance;

    public ActorProxy(ActorInstance instance)
    {
        _instance = instance;
    }

    public void InterceptSynchronous(IInvocation invocation)
    {
        throw new NotSupportedException("No");
    }

    public void InterceptAsynchronous(IInvocation invocation)
    {
        var task = Task.Factory.StartNew(async () =>
        {
            await (Task)invocation.Method.Invoke(_instance.Actor, invocation.Arguments);
        }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, scheduler: _instance.Scheduler).Unwrap();
        
        _instance.Scheduler.AddTask(task);
        invocation.ReturnValue = task;
    }

    public void InterceptAsynchronous<TResult>(IInvocation invocation)
    {
        var task = Task.Factory.StartNew(async () =>
        {
            var result = await (Task<TResult>)invocation.Method.Invoke(_instance.Actor, invocation.Arguments);
            return (TResult)result;
        }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, scheduler: _instance.Scheduler).Unwrap();
        
        _instance.Scheduler.AddTask(task);
        invocation.ReturnValue = task;
    }
}