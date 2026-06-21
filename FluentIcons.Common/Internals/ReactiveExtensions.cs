using System;

namespace FluentIcons.Common.Internals;

internal static class ReactiveExtensions
{
    public static IObservable<T> SkipOne<T>(this IObservable<T> source)
    {
        return new SkipOneObservable<T>(source);
    }

    private class SkipOneObservable<T>(IObservable<T> source) : IObservable<T>
    {
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return source.Subscribe(new Observer(observer));
        }

        private class Observer(IObserver<T> observer) : IObserver<T>
        {
            private bool _isFirst = true;
            public void OnCompleted() => observer.OnCompleted();
            public void OnError(Exception error) => observer.OnError(error);

            public void OnNext(T value)
            {
                if (_isFirst)
                    _isFirst = false;
                else
                    observer.OnNext(value);
            }
        }
    }
}
