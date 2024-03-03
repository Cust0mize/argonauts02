using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Resource {
	public enum Types { Food, Stone, Wood, Gold, Worker, Jaison, Medea, Gem }
	public Types Type;
	public int Count;

	public Resource (Types type , int count) {
		Type = type;
		Count = count;
	}

	public override string ToString () {
		return string.Format("{0} - {1}" , Type , Count);
	}
}
