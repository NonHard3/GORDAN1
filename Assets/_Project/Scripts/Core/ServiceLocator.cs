using System;
using System.Collections.Generic;

namespace _Project.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
                _services[type] = service;
            else
                _services.Add(type, service);
        }

        public static T Get<T>()
        {
            var type = typeof(T);
            return _services.ContainsKey(type) ? (T)_services[type] : throw new Exception($"Service {type} not found.");
        }

        public static void Clear() => _services.Clear();
    }
}
