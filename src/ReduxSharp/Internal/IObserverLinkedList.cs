namespace ReduxSharp.Internal
{
    internal interface IObserverLinkedList<T>
    {
        void UnsubscribeNode(ObserverNode<T> node);
    }
}
