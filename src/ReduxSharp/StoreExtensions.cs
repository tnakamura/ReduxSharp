using System;
using System.Threading.Tasks;

namespace ReduxSharp
{
    /// <summary>
    /// Extensions for <see cref="IStore{TState}"/>.
    /// </summary>
    public static class StoreExtensions
    {
        /// <summary>
        /// Dispatches an action creator.
        /// </summary>
        /// <param name="store">A store.</param>
        /// <param name="actionCreator">
        /// A function that creates an action.
        /// </param>
        public static void Dispatch<TState>(this IStore<TState> store, ActionCreator<TState> actionCreator)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (actionCreator == null) throw new ArgumentNullException(nameof(actionCreator));

            var action = actionCreator(store.State, store);
            if (action != null)
            {
                store.Dispatch(action);
            }
        }

        /// <summary>
        /// Dispatches an async action creator.
        /// </summary>
        /// <param name="store">A store.</param>
        /// <param name="asyncActionCreator">
        /// A function that creates and dispatches actions asynchronously.
        /// </param>
        /// <returns>A task that represents the asynchronous dispatch actions.</returns>
        public async static Task Dispatch<TState>(this IStore<TState> store, AsyncActionCreator<TState> asyncActionCreator)
        {
            if (store == null) throw new ArgumentNullException(nameof(store));
            if (asyncActionCreator == null) throw new ArgumentNullException(nameof(asyncActionCreator));

            await asyncActionCreator(store.State, store, actionCreator =>
            {
                var action = actionCreator(store.State, store);
                if (action != null)
                {
                    store.Dispatch(action);
                }
            }).ConfigureAwait(false);
        }
    }
}
