using System;
using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// Create an asynchronous action.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    /// <param name="store">A store</param>
    /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
    public delegate Task AsyncActionCreator<TState>(IStore<TState> store);

    /// <summary>
    /// 
    /// </summary>
    public static class StoreExtensions
    {
        /// <summary>
        /// Dispatches an asynchronous action.
        /// </summary>
        /// <typeparam name="TState">A type of root state tree</typeparam>
        /// <param name="store">A store</param>
        /// <param name="asyncActionCreator"></param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public static async Task DispatchAsync<TState>(
            this IStore<TState> store,
            AsyncActionCreator<TState> asyncActionCreator)
        {
            if (asyncActionCreator == null) throw new ArgumentNullException(nameof(asyncActionCreator));
            await asyncActionCreator(store).ConfigureAwait(false);
        }
    }
}
