using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchObject : BuildObject {
	[SerializeField] GameObject light;
	[SerializeField] float radiusActivate = 350F;

    public override bool CanUpgrade {
        get {
            return !Upgrade.Equals(1);
        }
        set {
            base.CanUpgrade = value;
        }
    }

	public float RadiusActivate {
		get {
			return radiusActivate;
		}
	}

    public override void Start() {
        base.Start();
        StartCoroutine(InitLight());

        if (Upgrade >= 1) Node.IsFree = true;
    }

    private IEnumerator InitLight () {
        while (!GameManager.I.GameInited)
            yield return null;

		if (light != null) {
			StartCoroutine (GrowLightAlpha ());
		}

		if (light != null)
			light.transform.SetParent (null);

		if (Upgrade == 1 && light != null) {
			GameManager.I.ActivateObjectsInPosition (light.transform.position, radiusActivate);
		}
	}

	private IEnumerator GrowLightAlpha () {
		light.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, 0F);

		float a = 0;
		while (a < 1) {
			light.GetComponent<SpriteRenderer> ().color = new Color (1f, 1f, 1f, a);
			a += Time.deltaTime * 1.5F;
			yield return null;
		}
	}

    protected override void OnDestroy () {
        base.OnDestroy();

		if (light != null)
			Destroy (light);
	}

	void OnDrawGizmos () {
		if (light != null) {
			Gizmos.color = Color.green;
			Gizmos.DrawWireSphere (light.transform.position, radiusActivate);
		}
	}
}
