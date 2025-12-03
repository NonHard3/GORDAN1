using System;
using System.Collections.Generic;

namespace _Project.Core
{
    public class EventBus
    {
        private readonly Dictionary<Type, List<Delegate>> _eventTable = new();

        public void Subscribe<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (!_eventTable.ContainsKey(type))
                _eventTable[type] = new List<Delegate>();

            _eventTable[type].Add(listener);
        }

        public void Unsubscribe<T>(Action<T> listener)
        {
            var type = typeof(T);
            if (_eventTable.ContainsKey(type))
                _eventTable[type].Remove(listener);
        }

        public void Publish<T>(T evt)
        {
            var type = typeof(T);
            if (_eventTable.TryGetValue(type, out var listeners))
            {
                foreach (var listener in listeners)
                    (listener as Action<T>)?.Invoke(evt);
            }
        }
    }
}
