using System;
using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// Create an asynchronous action.
    /// </summary>
    /// <typeparam name="TState">A type of root state tree</typeparam>
    /// <param name="store">A store</param>
    /// <returns>A task that completes when the action creator's asynchronous operation completes.</returns>
    public delegate Task AsyncActionCreator<TState>(IStore<TState> store);

    /// <summary>
    /// Extension methods for a <see cref="IStore{TState}"/>.
    /// </summary>
    public static class StoreExtensions
    {
        /// <summary>
        /// Dispatches an asynchronous action.
        /// </summary>
        /// <typeparam name="TState">A type of root state tree</typeparam>
        /// <param name="store">A store</param>
        /// <param name="asyncActionCreator">The asynchronous operation that can dispatch actions to the store.</param>
        /// <returns>A task that completes when the supplied action creator completes.</returns>
        public static async Task DispatchAsync<TState>(
            this IStore<TState> store,
            AsyncActionCreator<TState> asyncActionCreator)
        {
            if (asyncActionCreator == null) throw new ArgumentNullException(nameof(asyncActionCreator));
            await asyncActionCreator(store).ConfigureAwait(false);
        }
    }
}
