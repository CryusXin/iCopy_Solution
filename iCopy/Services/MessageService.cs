using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace iCopy.Services
{
    public class MessageService
    {
        private static readonly Lazy<MessageService> _instance = new Lazy<MessageService>(() => new MessageService());
        public static MessageService Instance => _instance.Value;

        private readonly ConcurrentDictionary<Type, List<Action<object>>> _handlers
            = new ConcurrentDictionary<Type, List<Action<object>>>();

        private MessageService() { }

        public void Register<T>(Action<T> handler)
        {
            var type = typeof(T);
            var wrapper = new Action<object>(obj => handler((T)obj));

            _handlers.AddOrUpdate(
                type,
                _ => new List<Action<object>> { wrapper },
                (_, list) =>
                {
                    list.Add(wrapper);
                    return list;
                });
        }

        public void Unregister<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var list))
            {
                var wrapper = new Action<object>(obj => handler((T)obj));
                list.RemoveAll(h => h.Target == wrapper.Target);
            }
        }

        public void Send<T>(T message)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var list))
            {
                foreach (var handler in list.ToArray())
                {
                    handler(message);
                }
            }
        }
    }
}
