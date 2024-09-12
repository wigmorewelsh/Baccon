using System.Linq.Expressions;
using System.Reflection;

namespace Baccon;

public class ActorProxy : DispatchProxy
{
    private ActorInstance _instance;

    public ActorProxy()
    {
    }

    public static TProxy CreateActor<TProxy>(ActorInstance instance) where TProxy : class
    {
        object proxy = Create<TProxy, ActorProxy>();
        ((ActorProxy)proxy)._instance = instance;
        return (TProxy)proxy;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        if (targetMethod?.ReturnType == typeof(Task))
        {
            var task = Task.Factory.StartNew(async () => { await (Task)targetMethod.Invoke(_instance.Actor, args); },
                CancellationToken.None, TaskCreationOptions.DenyChildAttach, scheduler: _instance.Scheduler).Unwrap();

            return task;
        }

        if (targetMethod?.ReturnType.IsGenericType == true &&
            targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var task = Task.Factory.StartNew(async () =>
            {
                var invoke = targetMethod.Invoke(_instance.Actor, args);
                await (Task)invoke;
                return (object)((dynamic)invoke).Result;
            }, CancellationToken.None, TaskCreationOptions.DenyChildAttach, scheduler: _instance.Scheduler).Unwrap();

            var resultType = targetMethod.ReturnType.GetGenericArguments()[0];
            var expr = Expression.Call(typeof(ActorProxy), nameof(Wrapper), new Type[] { resultType },
                Expression.Constant(task));
            var lambda = Expression.Lambda(expr).Compile();

            return lambda.DynamicInvoke();
        }

        throw new NotSupportedException("No");
    }

    public static async Task<TResult> Wrapper<TResult>(Task<object> task)
    {
        var o = await task;
        return (TResult)o;
    }
}