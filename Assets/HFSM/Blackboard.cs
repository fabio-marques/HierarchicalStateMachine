using System.Collections;
using System.Collections.Generic;
using System.Globalization;

namespace HFSM
{
    public class Blackboard : IEnumerable<KeyValuePair<string, float>>
    {
        private readonly Dictionary<string, float> _values = new Dictionary<string, float>();

        public event System.Action OnUpdate;

        public void SetDirty() => OnUpdate?.Invoke();
    
        //Setters
        public void Set(string key, int value)
        {
            if (value != default) Store(key, value);
            else Remove(key);
        }
        public void Set(string key, bool value)
        {
            if (value) Store(key, 1);
            else Remove(key);
        }
        public void Set(string key, float value)
        {
            if (value != default) Store(key, value);
            else Remove(key);
        }

        public int Add(string key, int value = 1)
        {
            var newValue = GetIntValue(key) + value;
            Set(key, newValue);
            return newValue;
        }
        public float Add(string key, float value = 1)
        {
            var newValue = GetFloatValue(key) + value;
            Set(key, newValue);
            return newValue;
        }
        
        public void CopyKeyValuesFrom(Blackboard targetBlackboard)
        {
            foreach (var pair in targetBlackboard)
            {
                Set(pair.Key, pair.Value);
            }
        }
    
        //Getters
        public float GetFloatValue(string key) => Contains(key) ? _values[key] : default;
        public bool GetBool(string key) => Contains(key);
        public int GetIntValue(string key) => (int)GetFloatValue(key);
        public string GetString(string key) => Contains(key) ? _values[key].ToString(CultureInfo.InvariantCulture) : string.Empty;
    
        public bool TryGetBool(string key, out bool value)
        {
            value = default;
            return TryGet(key, out _);
        }
    
        public bool TryGetInt(string key, out int value)
        {
            value = default;
            if (!TryGet(key, out var floatValue)) return false;
            value = (int) floatValue;
            return true;

        }
    
        public bool TryGetString(string key, out string value)
        {
            value = default;
            if (!TryGet(key, out var floatValue)) return false;
            value = floatValue.ToString(CultureInfo.InvariantCulture);
            return true;
        }

        private bool TryGet(string key, out float value)
        {
            return _values.TryGetValue(key, out value);
        }

        //Storage
        private void Store(string key, float value)
        {
            if (_values.TryGetValue(key, out var oldValue) && oldValue.Equals(value)) return;
            _values[key] = value;
            SetDirty();
        }
    
        public void Remove(string key)
        {
            if (_values.Remove(key)) SetDirty();
        }

        public bool Contains(string key)
        {
            return _values.ContainsKey(key);
        }

        public void Clear()
        {
            if (_values.Count <= 0) return;
            _values.Clear();
            SetDirty();
        }
    
        //Enumerator
        IEnumerator<KeyValuePair<string, float>> IEnumerable<KeyValuePair<string, float>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, float>>)_values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, float>>)_values).GetEnumerator();
        }
    }
}