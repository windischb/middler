using System.Linq;
using System.Threading.Tasks;
using Nito.AsyncEx.Synchronous;
using Reflectensions.ExtensionMethods;
using Scripter.Shared;

namespace middler.TaskHelper.ScripterModule
{
    public class TaskHelperModule: IScripterModule
    {
       
        public object Await(Task task)
        {
            var type = task.GetType();
            if (type.GenericTypeArguments.Any())
            {
                return task.ConvertToTaskOf<object>().WaitAndUnwrapException();
            }

            task.WaitAndUnwrapException();
            return null;
        }


        public void AwaitVoid(Task task)
        {
            Await(task);
        }

        public void WaitAll(params Task[] tasks)
        {
            Task.WaitAll(tasks);
        }

        public Task WhenAll(params Task[] tasks)
        {
            return Task.WhenAll(tasks);
        }

    }
}
