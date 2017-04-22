using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace SortMyFiles
{
    public class Queue
    {
        private Subject<object> _inner = new Subject<object>();

        public IObservable<T> Register<T>()
        {
            return _inner.OfType<T>().Publish().RefCount();
        }

        public void Publish<T>(T message)
        {
            _inner.OnNext(message);
        }
    }
}
