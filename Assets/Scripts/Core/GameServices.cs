using System;
using System.Collections.Generic;

namespace TankSurvival.Core
{
    public static class GameServices
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static void Register<T>(T instance)
        {
            _services[typeof(T)] = instance;
        }

        public static bool TryGet<T>(out T instance)
        {
            if (_services.TryGetValue(typeof(T), out object service))
            {
                instance = (T)service;
                return true;
            }

            instance = default(T);
            return false;
        }

        public static T Get<T>()
        {
            return TryGet(out T instance) ? instance : default(T);
        }
    }
}
