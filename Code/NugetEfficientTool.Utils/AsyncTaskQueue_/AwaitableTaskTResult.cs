using System;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    /// <summary>
    /// 可等待的任务
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class AwaitableTask<TResult> : AwaitableTask
    {
        private readonly Task<TResult> _task;
        /// <summary>
        /// 初始化可等待的任务
        /// </summary>
        /// <param name="task">需要执行的任务</param>
        public AwaitableTask(Task<TResult> task) : base(task) => _task = task;

        #region TaskAwaiter

        /// <summary>
        /// 获取任务等待器
        /// </summary>
        /// <returns></returns>
        public new TaskAwaiter GetAwaiter() => new TaskAwaiter(this);

        /// <summary>
        /// 任务等待器
        /// </summary>
        [HostProtection(SecurityAction.LinkDemand, ExternalThreading = true, Synchronization = true)]
        public new struct TaskAwaiter : INotifyCompletion
        {
            private readonly AwaitableTask<TResult> _awaitableTask;

            /// <summary>
            /// 初始化任务等待器
            /// </summary>
            /// <param name="awaitableTask"></param>
            public TaskAwaiter(AwaitableTask<TResult> awaitableTask) => _awaitableTask = awaitableTask;

            /// <summary>
            /// 任务是否已完成。
            /// </summary>
            public bool IsCompleted => _awaitableTask._task.IsCompleted;

            /// <inheritdoc />
            public void OnCompleted(Action continuation)
            {
                var This = this;
                _awaitableTask._task.ContinueWith(t =>
                {
                    if (This._awaitableTask.Executable) continuation?.Invoke();
                });
            }

            /// <summary>
            /// 获取任务结果。
            /// </summary>
            /// <returns></returns>
            public TResult GetResult() => _awaitableTask._task.Result;
        }

        #endregion
    }
}
