using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseResourceObject : BaseInObject {
	public List<Resource> Resources;
	public float DurationWork = 5f;

	public float DurationWorkTotal {
		get {
			return DurationWork;
		}
	}

	[SerializeField]
	protected GameObject pickupEffectUsePrefab;

	public override IEnumerator Use(Action<List<Resource>> callback) {
		yield return null;
	}

    protected override void OnDestroy() {
        base.OnDestroy();
        StopUseEffect();
	}

	public void PlayPickupEffect(Vector3 position) {
		if (pickupEffectUsePrefab == null) return;
		GameObject effect = Instantiate(pickupEffectUsePrefab, position, Quaternion.identity);
		effect.GetComponent<ParticleSystem>().Play(true);
	}

    public override void PlayOneShotUseEffect(Vector3 positionUse, Vector3 positionChar, bool checkOrder = true) {
        if (DurationWork <= 0.5F) return;
        base.PlayOneShotUseEffect(positionUse, positionChar, checkOrder);
    }

    public void PlayUseEffect(Vector3 position, Vector3 positionChar, bool checkOrder = true) {
		if (DurationWork <= 0.5F) return;
		if (effectUsePrefab == null) return;
		if (currentUseEffect != null) return;
		currentUseEffect = Instantiate(effectUsePrefab, position, Quaternion.identity);

        if (checkOrder) {
            currentUseEffect.GetComponent<Renderer>().sortingOrder = (transform.position.y < positionChar.y) ? -1 : 1;
            if (currentUseEffect.transform.childCount > 0)
                currentUseEffect.transform.GetChild(0).GetComponent<Renderer>().sortingOrder = (transform.position.y < positionChar.y) ? -1 : 1;
        }
   
		currentUseEffect.GetComponent<ParticleSystem>().Play(true);
	}

	public void StopUseEffect() {
		if (currentUseEffect == null) return;
		currentUseEffect.GetComponent<ParticleSystem>().Stop(true);
		GameManager.I.StartCoroutine(IEDestroyAfterDelay(currentUseEffect, 1f));
	}

	private IEnumerator IEDestroyAfterDelay(GameObject obj, float delay) {
		yield return new WaitForSeconds(delay);
		Destroy(currentUseEffect);
	}
}
