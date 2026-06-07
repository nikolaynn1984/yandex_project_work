using System.Collections.Concurrent;

namespace EventDomain.Extentions
{
    public class EventQueue<T>
    {
        private readonly ConcurrentQueue<T> journals;
        public event Action<List<T>> onNext;
        private readonly SemaphoreSlim _addRequestLock = new(1, 1);
        private readonly int limit;
        private readonly int millisecondsDelay;
        public EventQueue(int limit = int.MaxValue, int millisecondsDelay = 1000)
        {
            this.journals = new ConcurrentQueue<T>();
            this.limit = limit;
            this.millisecondsDelay = millisecondsDelay;
        }

        public async Task Add(T item)
        {
            journals.Enqueue(item);


            await Next();
        }



        public async Task Next()
        {
            //Console.WriteLine($"{DateTime.Now.ToString()}: Вызов метода Next {this.millisecondsDelay}");
            if (!await _addRequestLock.WaitAsync(1))
                return;

            //Console.WriteLine($"{DateTime.Now.ToString()}: Прошел проверку  Next {this.millisecondsDelay}");
            await Task.Delay(this.millisecondsDelay);
            //Console.WriteLine($"{DateTime.Now.ToString()}: Прошел ожидание  Next {this.millisecondsDelay}");
            if (this.journals.Count > 0)
            {
                sendEvent();
                //Console.WriteLine($"{DateTime.Now.ToString()}: Отправил  Next {this.millisecondsDelay}");
            }
            _addRequestLock.Release();
        }

        private void sendEvent()
        {
            List<T> items = new List<T>();
            int count = 0;
            while (this.journals.TryDequeue(out T item) && count <= this.limit)
            {
                items.Add(item);
                count++;
            }

            if (items.Count > 0)
                this.onNext?.Invoke(items);
        }
    }
}
