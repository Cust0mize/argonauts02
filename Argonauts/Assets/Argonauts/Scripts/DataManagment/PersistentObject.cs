using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

[Serializable]
abstract public class PersistentObject : ISerializable {
	public Dictionary<string , object> Values = new Dictionary<string , object>();

	public virtual void GetObjectData (SerializationInfo info , StreamingContext context) {
		info.AddValue("values" , Values);
	}

	public object GetValue (string key) {
		if (Values.ContainsKey(key)) {
			return Values[key];
		}
		return null;
	}

    public T GetValue<T> (string key, object def = null) {
		object val = GetValue(key);
        if (val == null && def != null) return (T)def;
        else if (val == null && def == null) return default(T);
		return (T)val;
	}

	public void SetValue (string key , object value) {
		if (Values.ContainsKey(key)) Values[key] = value;
		else Values.Add(key , value);
	}
}
