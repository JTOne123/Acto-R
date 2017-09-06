using Castle.DynamicProxy;
using System;
using System.Threading.Tasks;

namespace ActoR
{
    partial class ActorInterceptor<T> : IInterceptor
    {
        private readonly T m_T;
        private readonly ActorEngine m_ActorEngine;

        public ActorInterceptor(T t, ActorAffinity affintiy)
        {
            m_T = t;
            m_ActorEngine = new ActorEngine(affintiy);
        }

        public void Intercept(IInvocation invocation)
        {
            bool containsReturnValue = invocation.Method.ReturnType.IsConstructedGenericType;
            var tcs = new TaskCompletionSource<object>();

            m_ActorEngine.Post(() =>
            {
                try
                {
                    var taskReturned = (Task)invocation.Method.Invoke(m_T, invocation.Arguments);

                    taskReturned.ContinueWith(previous =>
                    {
                         if (previous.IsFaulted)
                            tcs.SetException(previous.Exception);
                        else if (previous.IsCanceled)
                            tcs.SetCanceled();
                        else if (previous.IsCompleted)
                            tcs.SetResult(containsReturnValue ? ((dynamic)previous).Result :"No result");
                        else
                            tcs.SetException(previous.Exception);
                    });
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
                return tcs.Task;
            });

            if (containsReturnValue)
                invocation.ReturnValue = tcs.Task.ChangeTaskType(invocation.Method.ReturnType);
            else
                invocation.ReturnValue = tcs.Task.AsNonReturningTask();
        }

        
    }
}
