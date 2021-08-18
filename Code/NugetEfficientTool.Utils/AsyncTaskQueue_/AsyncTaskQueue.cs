﻿using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NugetEfficientTool.Utils
{
    /// <summary>
    /// 异步任务队列
    /// </summary>
    public class AsyncTaskQueue : IDisposable
    {
        /// <summary>
        /// 异步任务队列
        /// </summary>
        public AsyncTaskQueue()
        {
            _autoResetEvent = new AutoResetEvent(false);
            _thread = new Thread(InternalRunning) { IsBackground = true };
            _thread.Start();
        }

        #region 执行

        /// <summary>
        /// 执行异步操作
        /// </summary>
        /// <typeparam name="T">返回结果类型</typeparam>
        /// <param name="func">异步操作</param>
        /// <returns>IsValid:异步操作是否有效(多任务时，如果设置了<see cref="AutoCancelPreviousTask"/>,只会保留最后一个任务有效)；result:异步操作结果</returns>
        public async Task<AsyncTaskExecuteResult<T>> ExecuteAsync<T>(Func<Task<T>> func)
        {
            var task = GetExecutableTask(func);
            var result = await await task;
            if (!task.IsValid)
            {
                result = default(T);
            }
            return new AsyncTaskExecuteResult<T>(task.IsValid,result);
        }

        /// <summary>
        /// 执行异步操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public async Task<bool> ExecuteAsync<T>(Func<Task> func)
        {
            var task = GetExecutableTask(func);
            await await task;
            return task.IsValid;
        }

        #endregion

        #region 添加任务

        /// <summary>
        /// 获取待执行任务
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private AwaitableTask GetExecutableTask(Action action)
        {
            var awaitableTask = new AwaitableTask(new Task(action));
            AddPenddingTaskToQueue(awaitableTask);
            return awaitableTask;
        }

        /// <summary>
        /// 获取待执行任务
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="function"></param>
        /// <returns></returns>
        private AwaitableTask<TResult> GetExecutableTask<TResult>(Func<TResult> function)
        {
            var awaitableTask = new AwaitableTask<TResult>(new Task<TResult>(function));
            AddPenddingTaskToQueue(awaitableTask);
            return awaitableTask;
        }

        /// <summary>
        /// 添加待执行任务到队列
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        private void AddPenddingTaskToQueue(AwaitableTask task)
        {
            //添加队列，加锁。
            lock (_queue)
            {
                _queue.Enqueue(task);
                //开始执行任务
                _autoResetEvent.Set();
            }
        }

        #endregion

        #region 内部运行

        private void InternalRunning()
        {
            while (!_isDisposed)
            {
                if (_queue.Count == 0)
                {
                    //等待后续任务
                    _autoResetEvent.WaitOne();
                }
                while (TryGetNextTask(out var task))
                {
                    //如已从队列中删除
                    if (!task.Executable) continue;
                    //添加是否已释放的判断
                    if (!_isDisposed)
                    {
                        if (UseSingleThread)
                        {
                            task.RunSynchronously();
                        }
                        else
                        {
                            task.Start();
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 上一次异步操作
        /// </summary>
        private AwaitableTask _lastDoingTask;
        private bool TryGetNextTask(out AwaitableTask task)
        {
            task = null;
            while (_queue.Count > 0)
            {
                //获取并从队列中移除任务
                if (_queue.TryDequeue(out task) && (!AutoCancelPreviousTask || _queue.Count == 0))
                {
                    //设置进行中的异步操作无效
                    _lastDoingTask?.MarkTaskInvalid();
                    _lastDoingTask = task;
                    return true;
                }
                //并发操作，设置任务不可执行
                task.SetNotExecutable();
            }
            return false;
        }

        #endregion

        #region dispose

        /// <inheritdoc />
        public void Dispose()
        {
            if (_isDisposed) return;
            _autoResetEvent.Dispose();
            _thread = null;
            _autoResetEvent = null;
            _isDisposed = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 析构任务队列
        /// </summary>
        ~AsyncTaskQueue() => Dispose();

        #endregion

        #region 属性及字段

        /// <summary>
        /// 是否使用单线程完成任务.
        /// </summary>
        public bool UseSingleThread { get; set; } = true;

        /// <summary>
        /// 自动取消以前的任务。
        /// </summary>
        public bool AutoCancelPreviousTask { get; set; } = false;

        private bool _isDisposed;
        private readonly ConcurrentQueue<AwaitableTask> _queue = new ConcurrentQueue<AwaitableTask>();
        private Thread _thread;
        private AutoResetEvent _autoResetEvent;

        #endregion

    }
    public class AsyncTaskExecuteResult<T>
    {
        public AsyncTaskExecuteResult(bool isValid, T result)
        {
            IsValid = isValid;
            Result = result;
        }
        /// <summary>
        /// 异步操作是否有效(多任务时，如果设置了"AutoCancelPreviousTask",只会保留最后一个任务有效)
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 异步操作结果
        /// </summary>
        public T Result { get; set; }
    }
}
