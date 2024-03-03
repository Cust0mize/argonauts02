using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollOfTheFieldController : MonoBehaviour {
	[SerializeField] private ParticleSystem[] rollers;
	[SerializeField] private float delayBetweenPlay = 10f;

	private void Start() {
		StartCoroutine(Worker());
	}

	IEnumerator Worker() {
		while (true) {
			foreach (ParticleSystem ps in rollers) {
				ps.Play();
				yield return new WaitForSeconds(ps.main.duration);
			}
			yield return new WaitForSeconds(delayBetweenPlay);
		}
	}
}
