using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelPathBlocked : MonoBehaviour {
	public void Open () {
		gameObject.SetActive (true);
        StopAllCoroutines();
		StartCoroutine (DelayClose ());
	}

	IEnumerator DelayClose () {
		yield return new WaitForSeconds (1.4F);
		Close ();
	}

	public void Close () {
		gameObject.SetActive (false);
	}
}
