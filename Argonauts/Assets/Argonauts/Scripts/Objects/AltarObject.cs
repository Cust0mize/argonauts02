using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AltarObject : NormalResourceObject {
	[SerializeField]
    private string activateKeyAction;
	[SerializeField]
	private GameObject takeWorkPrefab;
    [SerializeField]
    private Sprite notActivatedSprite;
    [SerializeField]
    private Sprite activatedSprite;
    [SerializeField]
    private GameObject sign;
    [SerializeField]
    private GameObject energy;

	private HudBar TakeWorkBar {
		get {
			if (takeWorkBar == null) {
				GameObject b = Instantiate (takeWorkPrefab, GameManager.I.Canvas2D.transform);
				takeWorkBar = b.GetComponent<HudBar> ();
				takeWorkBar.transform.position = transform.position;
				takeWorkBar.transform.position = new Vector2 (takeWorkBar.transform.position.x + offsetWorkBar.x, takeWorkBar.transform.position.y + offsetWorkBar.y);
				b.SetActive (false);
			}
			return takeWorkBar;
		}
	}

	private HudBar takeWorkBar;
	[SerializeField]
    private bool isReady;
	[SerializeField]
    private bool isActivated;
    private bool lastIsActivated;
	private List<Resource> takeNeedResources;

    public float DurationActivate = 3F;
    public float DurationReady = 3F;

    public float DurationActivateTotal {
		get {
			return DurationActivate;
		}
	}

    public bool IsActivated {
		get {
			return isActivated;
		}
		set {
			isActivated = value; 

			if (!lastIsActivated && value) {
				RequiredResources = takeNeedResources;
                CountStages = CountCharacters();
				StartCoroutine (Ready ());
			}
	
			lastIsActivated = value;
            UpdateSprite();
		}
	}

    public bool IsReady {
		get {
			return isReady;
		}
		set {
			isReady = value; 

            UpdateSprite();
		}
	}

	public override bool CanBeUsed () {
        if (isActivated) {
            return IsReady;
        }
        return true;
	}

    private void UpdateSprite() {
        if (isActivated) {
            if (IsReady) {
                sign.gameObject.SetActive(false);
                sign.GetComponent<TweenScale>().enabled = false;
                energy.gameObject.SetActive(true);
            } else {
                sign.gameObject.SetActive(true);
                sign.GetComponent<TweenScale>().PlayForward();
                energy.gameObject.SetActive(false);
            }
        }

        GetComponent<SpriteRenderer>().sprite = IsActivated ? activatedSprite : notActivatedSprite;

        DestroyImmediate(Overlay.gameObject);
        Overlay = null;
        InitOverlay();
        Overlay.gameObject.SetActive(true);
    }

	protected override void Awake () {
        base.Awake();

		InitOverlay ();

        PlayerAction pl1 = ConfigManager.Instance.GetPlayerAction (KeyAction);
		PlayerAction pl2 = ConfigManager.Instance.GetPlayerAction (activateKeyAction);

        if (pl2.NeedResources != null) {
            RequiredResources = new List<Resource>(pl2.NeedResources);
            CountStages = CountCharacters();
        }
		if (pl1.NeedResources != null)
			takeNeedResources = new List<Resource> (pl1.NeedResources);
		if (pl1.GetResources != null)
			Resources = new List<Resource> () { pl1.GetResources };

        DurationWork = ConfigManager.Instance.GetDurationAction(KeyAction);
		DurationActivate = ConfigManager.Instance.GetDurationAction (activateKeyAction);
	}

	public override void Start () {
		base.Start ();
		IsActivated = isActivated;
	}

	public override void OnPointerEnter (UnityEngine.EventSystems.PointerEventData eventData) {
		if (!Available)
			return;

		if (IsActivated) {
            StringBuilder sb = new StringBuilder();
            string[] keyParts = KeyAction.Split('_');
            for (int i = 1; i < keyParts.Length; i++) {
                sb.Append("_");
                sb.Append(keyParts[i]);
            }

            GameGUIManager.I.ShowActionInfo (LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, Resources, this);
		} else {
            StringBuilder sb = new StringBuilder();
            string[] keyParts = activateKeyAction.Split('_');
            for (int i = 1; i < keyParts.Length; i++) {
                sb.Append("_");
                sb.Append(keyParts[i]);
            }

            GameGUIManager.I.ShowActionInfo (LocalizationManager.GetLocalizedString (LocalizationManager.GetLocalizedString(string.Format("title{0}", sb))), RequiredResources, Resources, this);
		}
	}

	public override void OnPointerClick (UnityEngine.EventSystems.PointerEventData eventData) {
		base.OnPointerClick (eventData);

		if (!Available)
			return;

#if !UNITY_STANDALONE
        if (IsActivated) {
            StringBuilder sb = new StringBuilder();
            string[] keyParts = KeyAction.Split('_');
            for (int i = 1; i < keyParts.Length; i++) {
                sb.Append("_");
                sb.Append(keyParts[i]);
            }

            GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, Resources, this);
        }
        else {
            StringBuilder sb = new StringBuilder();
            string[] keyParts = activateKeyAction.Split('_');
            for (int i = 1; i < keyParts.Length; i++) {
                sb.Append("_");
                sb.Append(keyParts[i]);
            }

            GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, Resources, this);
        }
#endif
    }

    public override IEnumerator Use (System.Action<List<Resource>> callback) {
		if (IsActivated) {
            yield return new WaitForSeconds(DurationWork);
            GameManager.I.AddFillBonusPrecent(100F);
            LevelTaskController.I.UpdateTask (KeyAction, 1);
			IsReady = false;

            int score = 0;
            foreach (Resource r in Resources) score += r.Count * 10;
            GameManager.I.AddScores(transform.position, score);
        } else {
			if (DurationActivateTotal > 1F)
				WorkBar.gameObject.SetActive (true);

			float timer = DurationActivate;
			while (timer >= 0) {
                timer -= DeltaTimeWork;
				if (DurationActivateTotal > 1F)
					WorkBar.UpdateBar (DurationActivateTotal - timer, DurationActivate, false);
				yield return null;
			}

			IsActivated = true;

            GameManager.I.AddScores(transform.position, 50);
            LevelTaskController.I.UpdateTask (activateKeyAction, 1);

			if (DurationActivateTotal > 1F)
				WorkBar.gameObject.SetActive (false);
		}

        SendedCharacters.Clear();

        PlayOnEndUse();
	}

    private IEnumerator Ready () {
		while (true) {
            while (IsReady || !GameManager.I.GameStarted)
                yield return null;

            if (DurationReady > 1F)
				TakeWorkBar.gameObject.SetActive (true);

            float timer = DurationReady;
			while (timer >= 0) {
				timer -= Time.deltaTime;
                if (DurationReady > 1F)
                    TakeWorkBar.UpdateBar (DurationReady - timer, DurationReady, false);
				yield return null;
			}
				
			IsReady = true;
            AudioWrapper.I.PlaySfx("altar_ready");
            CheckShouldPing();

            if (DurationReady > 1F)
				TakeWorkBar.gameObject.SetActive (false);
		}
	}
}
