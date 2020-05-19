using Entity;
using MongoDB.Driver;
using Operation.Common;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Operation
{
    public class ConnectionThrottlingPipeline
    {
        private readonly Semaphore openConnectionSemaphore;

        public ConnectionThrottlingPipeline()
        {
            //Only grabbing half the available connections to hedge against collisions.
            //If you send every operation through here
            //you should be able to use the entire connection pool.
            openConnectionSemaphore = new Semaphore(BaseMongoHelper.Client.Settings.MaxConnectionPoolSize / 2,
                BaseMongoHelper.Client.Settings.MaxConnectionPoolSize / 2);
        }

        public async Task<T> AddRequest<T>(Task<T> task) where T : BaseModel
        {
            try
            {
                openConnectionSemaphore.WaitOne();
                var result = await task;
                openConnectionSemaphore.Release();

                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                throw e;
            }
        }

        public async Task<long> AddRequest(Task<long> task)
        {
            try
            {
                openConnectionSemaphore.WaitOne();
                var result = await task;
                openConnectionSemaphore.Release();

                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                throw e;
            }
        }

        public async Task<bool> AddRequest(Task<bool> task)
        {
            try
            {
                openConnectionSemaphore.WaitOne();
                var result = await task;
                openConnectionSemaphore.Release();

                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                throw e;
            }
        }

        public async Task<List<T>> AddRequest<T>(Task<List<T>> task) where T : BaseModel
        {
            try
            {
                openConnectionSemaphore.WaitOne();
                var result = await task;
                openConnectionSemaphore.Release();

                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                throw e;
            }
        }

        public async Task AddRequest(Task task)
        {
            try
            {
                openConnectionSemaphore.WaitOne();
                await task;
                openConnectionSemaphore.Release();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public T AddRequest<T>(Func<T> func) where T : BaseModel
        {
            try
            {
                openConnectionSemaphore.WaitOne();
                var result = func.Invoke();
                openConnectionSemaphore.Release();
                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                throw e;
            }
        }

        public long AddRequest(Func<long> func)
        {
            try
            {
                openConnectionSemaphore.WaitOne();
                var result = func.Invoke();
                openConnectionSemaphore.Release();
                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                throw e;
            }
        }

        public bool AddRequest(Func<bool> func)
        {
            try
            {
                openConnectionSemaphore.WaitOne();
                var result = func.Invoke();
                openConnectionSemaphore.Release();
                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                throw e;
            }
        }

        public List<T> AddRequest<T>(Func<List<T>> func) where T : BaseModel
        {
            try
            {
                openConnectionSemaphore.WaitOne();
                var result = func.Invoke();
                openConnectionSemaphore.Release();
                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                throw e;
            }
        }

        public void AddRequest(Action action)
        {
            try
            {
                openConnectionSemaphore.WaitOne();
                action.Invoke();
                openConnectionSemaphore.Release();
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                throw e;
            }
        }
    }
}
