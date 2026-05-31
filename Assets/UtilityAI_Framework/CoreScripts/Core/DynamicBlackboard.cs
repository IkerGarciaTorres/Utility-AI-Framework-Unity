using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

namespace UtilityAI
{
    public class DynamicBlackboard : MonoBehaviour
    {
        private Dictionary<string, object> map = new Dictionary<string, object>();
        private Dictionary<string, FieldInfo> fields = new Dictionary<string, FieldInfo>();
        private bool initialized = false;

        private void Initialize()
        {
            FieldInfo[] allFields = this.GetType().GetFields();
            foreach (FieldInfo field in allFields)
            {
                string name = field.Name.ToUpper();
                fields.Add(name, field);
            }

            initialized = true;
        }

        public T Get<T>(string key)
        {
            object value;

            if (!initialized) Initialize();

            if (typeof(T).Equals(typeof(float)))
            {
                float theFloat;
                if (float.TryParse(key, NumberStyles.Float, CultureInfo.InvariantCulture, out theFloat))
                {
                    value = theFloat;
                    return (T)value;
                }
                else if (key.ToUpper().EndsWith('F'))
                {
                    if (float.TryParse(key.Substring(0, key.Length - 1), NumberStyles.Float, CultureInfo.InvariantCulture, out theFloat))
                    {
                        value = theFloat;
                        return (T)value;
                    }
                }
            }
            else if (typeof(T).Equals(typeof(int)))
            {
                int theInt;
                if (int.TryParse(key, out theInt))
                {
                    value = theInt;
                    return (T)value;
                }
            }
            else if (typeof(T).Equals(typeof(bool)))
            {
                bool theBool;
                if (bool.TryParse(key, out theBool))
                {
                    value = theBool;
                    return (T)value;
                }
            }
            else if (typeof(T).Equals(typeof(string)))
            {
                if (key == null)
                    Debug.LogWarning("Null key");

                string upperName = key.ToUpper();
                if (!fields.ContainsKey(upperName) && !map.ContainsKey(upperName))
                {
                    value = key;
                    return (T)value;
                }
            }

            return InnerGet<T>(key);
        }

        private T InnerGet<T>(string name)
        {
            object value = null;

            name = name.ToUpper();
            if (fields.ContainsKey(name))
                value = fields[name].GetValue(this);
            else if (map.ContainsKey(name))
                value = map[name];
            else
                Debug.LogWarning("Unknown key in blackboard: " + name);

            if (value == null) 
                return default(T);

            try
            {
                return (T)System.Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);
            }
            catch
            {
                return (T)value;
            }
        }

        public void Put(string name, object value)
        {
            if (!initialized) Initialize();
            name = name.ToUpper();
            if (fields.ContainsKey(name))
                fields[name].SetValue(this, value);
            else
                map[name] = value;
        }

        public void PutIfNotPresent(string name, object value)
        {
            if (!Exists(name))
                Put(name, value);
        }

        public bool Exists(string key)
        {
            if (!initialized) 
                Initialize();

            key = key.ToUpper();
            return map.ContainsKey(key) || fields.ContainsKey(key);
        }

        public void Dump()
        {
            if (!initialized) 
                Initialize();

            Debug.Log("--- Dumping fields ---");
            foreach (string s in fields.Keys)
            {
                Debug.Log(s);
            }
        }
    }
}