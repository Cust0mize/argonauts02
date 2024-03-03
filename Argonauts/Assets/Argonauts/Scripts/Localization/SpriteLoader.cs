using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteLoader : MonoBehaviour {
	private void Start () {
		DontDestroyOnLoad (gameObject);
	}

	public void Suicide () {
		Destroy (gameObject);
	}
}
