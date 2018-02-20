using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

namespace AvaloniaAV
{
    public static class ObservableExtensions
    {
        public static IObservable<IList<T>> SlidingBuffer<T>(this IObservable<T> observable, int windowSize)
        {
            return Enumerable.Range(1, windowSize - 1)
                .Select(i => observable.Take(i).ToList())
                .Merge()
                .Concat(Enumerable.Range(1, windowSize)
                    .Select(i => observable.Skip(i).Buffer(i))
                    .Merge());
        }

        public static IObservable<T> DisposeCurrentOnNext<T>(this IObservable<T> observable)
            where T: IDisposable
        {
            return observable.SlidingBuffer(2).Select(items =>
            {
                if (items.Count == 2)
                {
                    items[0]?.Dispose();
                    return items[1];
                }
                else
                {
                    return items[0];
                }
            });
        }
    }
}
