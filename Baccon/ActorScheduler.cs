using System.Threading.Channels;

namespace Baccon;

public class ActorScheduler : TaskScheduler
{
    private Channel<Task> tasks = Channel.CreateUnbounded<Task>();

    protected override IEnumerable<Task>? GetScheduledTasks()
    {
        return null;
    }

    protected override void QueueTask(Task task)
    {
        tasks.Writer.TryWrite(task);
    }

    public Task ExecuteTasks()
    {
        async Task InnerTask()
        {
            while (true)
            {
                var task = await tasks.Reader.ReadAsync();
                TryExecuteTask(task);
            }
        }

        return Task.Run(InnerTask);
    }

    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        return !taskWasPreviouslyQueued && TryExecuteTask(task);
    }
}