using System;
using System.Collections.Generic;

namespace Game.CMS
{
    public class DummySettingsProvider : ISettingsProvider
    {
        Dictionary<Type, Dictionary<string, object>> settings;

        public DummySettingsProvider()
        {
            settings = new()
            {
                {
                    typeof(int), new Dictionary<string, object>()
                    {
                        { "elementsPerContent", 15 },
                    }
                },
            };
        }
        public T GetData<T>(string id)
        {
            if (!settings.TryGetValue(typeof(T), out var data))
            {
                throw new KeyNotFoundException($"Type {typeof(T)} does not preset in dictionary");
            }
            if (!data.TryGetValue(id, out var value))
            {
                throw new KeyNotFoundException($"Id {id} does not preset in dictionary");
            }
            if (value is T castedValue)
                return castedValue;
            throw new InvalidCastException($"Id {id} present in other type: {value.GetType()}");
        }
    }
}