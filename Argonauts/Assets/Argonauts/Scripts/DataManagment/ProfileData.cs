using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[Serializable]
public class ProfileData : PersistentObject {
	public string Name;

	public ProfileData (string name , Dictionary<string , object> values) {
		Name = name;
		Values = values;
	}

	public ProfileData (string name) {
		Name = name;
	}

	public ProfileData (SerializationInfo info , StreamingContext context) {
		Values = (Dictionary<string , object>)info.GetValue("values" , typeof(Dictionary<string , object>));
		Name = info.GetString("name");
	}

	public override void GetObjectData (SerializationInfo info , StreamingContext context) {
		base.GetObjectData(info , context);

		info.AddValue("name" , Name);
	}
}
