using System;
using System.Threading;
using System.Threading.Tasks;

namespace ReduxSharp.Internal
{
    internal sealed class AsyncLock
    {
        readonly SemaphoreSlim semaphore;

        public AsyncLock()
        {
            semaphore = new SemaphoreSlim(1, 1);
        }

        public async ValueTask<IDisposable> LockAsync()
        {
            await semaphore.WaitAsync();
            return new LockReleaser(semaphore);
        }

        struct LockReleaser : IDisposable
        {
            readonly SemaphoreSlim semaphore;

            public LockReleaser(SemaphoreSlim semaphore)
            {
                this.semaphore = semaphore;
            }

            public void Dispose()
            {
                semaphore.Release();
            }
        }
    }
}
