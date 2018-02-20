using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

namespace AvaloniaAV
{
    public static class ObservableExtensions
    {
        private class DisposingSubject<T> : ISubject<T>
            where T : IDisposable
        {
            private readonly ISubject<T> inner = new Subject<T>();
            private readonly WeakReference<IDisposable> lastValue = new WeakReference<IDisposable>(null);

            public void OnCompleted()
            {
                inner.OnCompleted();
            }

            public void OnError(Exception error)
            {
                inner.OnError(error);
            }

            public void OnNext(T value)
            {
                inner.OnNext(value);
                if (lastValue.TryGetTarget(out var disposable))
                {
                    disposable.Dispose();
                }
                lastValue.SetTarget(value);
            }

            public IDisposable Subscribe(IObserver<T> observer)
            {
                return inner.Subscribe(observer);
            }
        }

        public static IObservable<T> DisposeCurrentOnNext<T>(this IObservable<T> observable)
            where T: IDisposable
        {
            var subject = new DisposingSubject<T>();
            observable.Subscribe(subject);
            return subject;
        }
    }
}
