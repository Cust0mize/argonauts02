using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalObject : BuildObject {
	public override event Action<int> OnSuccessUpgrade = delegate { };
	public BuildObjectController MirrorPortalController;

    [SerializeField] private GameObject activateVfx;
    [SerializeField] private string PortalKey;

    public override event Action<Node> OnDestroyed = delegate { };

    public override string BuildKey {
        get {
            return PortalKey;
        }
    }

    protected override void Awake() {
        base.Awake();

        StartCoroutine(IECheckFreeNode());
        StartCoroutine(IEJoinPortals());
    }

    private IEnumerator IEJoinPortals() {
        float timeOut = 15F;
        GameManager.I.Portals.GetType();

        while (timeOut > 0 && (MirrorPortalController == null || MirrorPortalController.BaseBuildObject == null)) {
            GameManager.I.Portals.GetType();
            timeOut -= Time.deltaTime;
            yield return null;
        }

        if (MirrorPortalController != null && MirrorPortalController.BaseBuildObject != null) {
            GameManager.I.Graph.AddNode(Node, MirrorPortalController.BaseBuildObject.Node);
            Debug.LogFormat("{0} and {1} success joined", Node, MirrorPortalController.BaseBuildObject.Node);
        }
    }

    private IEnumerator IECheckFreeNode() {
        yield return new WaitForSeconds(0.1f);
        if (Upgrade == 1) {
            Node.IsFree = true;
            OnDestroyed.Invoke(Node);

            if (GameManager.I.PassedTime > 3f) {
                Instantiate(activateVfx, transform.position, Quaternion.identity);
            }
        }
    }

    public void Transfer (BaseCharacter gnome) {
		gnome.transform.position = MirrorPortalController.BaseBuildObject.transform.position;
	}

	protected override void OnUpgrade (int upgrade) {
        if (upgrade == 1) {
            NotificationCenter.DefaultCenter().PostNotification(this, "OnPortalActivated");
            AudioWrapper.I.PlaySfx("portal_opened");
        }

        LevelTaskController.I.UpdateTask(KeyAction, 1);
        AwardHandler.I.UpdateAward("award_builder", 1);

        StartCoroutine(IECheckFreeNode());

        GameManager.I.Portals.GetType();
		OnSuccessUpgrade.Invoke(upgrade);

        if (MirrorPortalController != null && MirrorPortalController.BaseBuildObject != null)
            MirrorPortalController.BaseBuildObject.GetComponent<PortalObject>().SetUpgrade(MirrorPortalController.BaseBuildObject.Upgrade + 1);
	}

	protected void SetUpgrade (int value) {
        StartCoroutine(IECheckFreeNode());

        OnSuccessUpgrade.Invoke(value);
	}
}
