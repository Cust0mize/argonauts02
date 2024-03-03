using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SwitchObject : BaseInObject {
	[SerializeField] private Sprite switchActive;
	[SerializeField] private Sprite switchInactive;
	[HideInInspector] public string BridgeID;

	private BridgeObject Bridge;
	private SpriteRenderer spriteRenderer;

	public SpriteRenderer SpriteRenderer {
		get {
			if (spriteRenderer == null) {
				spriteRenderer = GetComponent<SpriteRenderer> ();
			}
			return spriteRenderer;
		}
	}

	public override void Start () {
		base.Start ();

		StartCoroutine (DelayInit ());
	}

	protected override void Awake () {
		base.Awake ();

		PlayerAction pl = ConfigManager.Instance.GetPlayerAction (KeyAction);
        if (pl.NeedResources != null) {
            RequiredResources = new List<Resource>(pl.NeedResources);
            CountStages = CountCharacters();
        }
	}

	public override void OnPointerEnter (UnityEngine.EventSystems.PointerEventData eventData) {
		if (!Available || !Clickable)
			return;

        StringBuilder sb = new StringBuilder();
        string[] keyParts = KeyAction.Split('_');
        for (int i = 1; i < keyParts.Length; i++) {
            sb.Append("_");
            sb.Append(keyParts[i]);
        }

        GameGUIManager.I.ShowActionInfo (LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, null, this);
	}

	public override void OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) {
		base.OnPointerClick (eventData);

		if (!Available || !Clickable)
			return;

#if !UNITY_STANDALONE
		StringBuilder sb = new StringBuilder();
		string[] keyParts = KeyAction.Split('_');
		for (int i = 1; i < keyParts.Length; i++) {
			sb.Append("_");
			sb.Append(keyParts[i]);
		}

		GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, null, this);
#endif
	}

	private IEnumerator DelayInit () {
		while (!GameManager.I.GameInited)
			yield return null;
		Bridge = GameManager.I.GetObject (BridgeID).GetComponent<BridgeObject> ();

		SpriteRenderer.sprite = Bridge.IsActive ? switchActive : switchInactive;
	}

	public override IEnumerator Use (System.Action<List<Resource>> callback) {
        WaitingWorkers++;
        while (WaitingWorkers < CountCharacters()) {
            yield return null;
        }

        CountStages--;
        if (CountStages <= 0) {
            LevelTaskController.I.UpdateTask(KeyAction, 1);

            Bridge.Open();
            AudioWrapper.I.PlaySfx("bridge_switched");

            int score = 0;
            foreach (Resource r in RequiredResources) score += r.Count * 10;
            GameManager.I.AddScores(transform.position, score);

            SpriteRenderer.sprite = Bridge.IsActive ? switchActive : switchInactive;

            Available = false;
            Clickable = false;

            PlayOnEndUse();

            WaitingWorkers = 0;
            SendedCharacters.Clear();
            CountStages = CountCharacters();
        }

		yield return null;
	}
}
