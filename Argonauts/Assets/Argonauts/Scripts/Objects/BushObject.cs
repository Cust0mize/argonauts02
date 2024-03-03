using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BushObject : NormalResourceObject {
	[SerializeField]
	private Sprite notGrowedSprite;
	[SerializeField]
	private Sprite notRipedSprite;
	[SerializeField]
	private Sprite ripedSprite;
	[SerializeField]
	private string growKeyAction;
	[SerializeField]
	private GameObject takeWorkPrefab;

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
	private bool isRiped;
	[SerializeField]
	private bool isGrowed;
	private bool lastIsGrowed;
	private List<Resource> takeNeedResources;

    [SerializeField] private ParticleSystem ripeEffect;

	public float DurationGrow = 3F;
    public float DurationGrowFruits = 3F;

	public float DurationGrowTotal {
		get {
			return DurationGrow;
		}
	}

	public bool IsGrowed {
		get {
			return isGrowed;
		}
		set {
			isGrowed = value; 

			if (!lastIsGrowed && value) {
				RequiredResources = takeNeedResources;
                CountStages = CountCharacters();
				StartCoroutine (Growing ());
			}
	
			lastIsGrowed = true;
			UpdateSprite ();
		}
	}

	public bool IsRiped {
		get {
			return isRiped;
		}
		set {
			isRiped = value; 

			UpdateSprite ();
		}
	}

	public override bool CanBeUsed () {
		return IsRiped;
	}

	private void UpdateSprite () {
		if (IsGrowed)
			GetComponent<SpriteRenderer> ().sprite = IsRiped ? ripedSprite : notRipedSprite;
		else
			GetComponent<SpriteRenderer> ().sprite = notGrowedSprite;

		DestroyImmediate (Overlay.gameObject);
		Overlay = null;
		InitOverlay ();
		Overlay.gameObject.SetActive (true);
	}

	protected override void Awake () {
        base.Awake();

		InitOverlay ();

        PlayerAction pl1 = ConfigManager.Instance.GetPlayerAction (KeyAction);
		PlayerAction pl2 = ConfigManager.Instance.GetPlayerAction (growKeyAction);

        if (pl2.NeedResources != null) {
            RequiredResources = new List<Resource>(pl2.NeedResources);
            CountStages = CountCharacters();
        }
		if (pl1.NeedResources != null)
			takeNeedResources = new List<Resource> (pl1.NeedResources);
		if (pl1.GetResources != null)
			Resources = new List<Resource> () { pl1.GetResources };

        DurationWork = ConfigManager.Instance.GetDurationAction(KeyAction);
		DurationGrow = ConfigManager.Instance.GetDurationAction (growKeyAction);
	}

	public override void Start () {
		base.Start ();
		IsGrowed = isGrowed;
	}

	public override void OnPointerEnter (UnityEngine.EventSystems.PointerEventData eventData) {
		if (!Available)
			return;

		if (IsGrowed) {
            StringBuilder sb = new StringBuilder();
            string[] keyParts = KeyAction.Split('_');
            for (int i = 1; i < keyParts.Length; i++) {
                sb.Append("_");
                sb.Append(keyParts[i]);
            }

            GameGUIManager.I.ShowActionInfo (LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, Resources, this);
		} else {
            StringBuilder sb = new StringBuilder();
            string[] keyParts = growKeyAction.Split('_');
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
		if (IsGrowed) {
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
			string[] keyParts = growKeyAction.Split('_');
			for (int i = 1; i < keyParts.Length; i++) {
				sb.Append("_");
				sb.Append(keyParts[i]);
			}

			GameGUIManager.I.OnObjectClickedMobile(LocalizationManager.GetLocalizedString(string.Format("title{0}", sb)), RequiredResources, Resources, this);
		}
#endif
	}

	public override IEnumerator Use (System.Action<List<Resource>> callback) {
		if (IsGrowed) {
            yield return new WaitForSeconds(DurationWork);
            callback.Invoke (ObjectCopier.Clone(Resources));
            LevelTaskController.I.UpdateTask (KeyAction, 1);
			IsRiped = false;

            int score = 0;
            foreach (Resource r in Resources) score += r.Count * 10;
            GameManager.I.AddScores(transform.position, score);
		} else {
			if (DurationGrowTotal > 1F)
				WorkBar.gameObject.SetActive (true);

			float timer = DurationGrow;
			while (timer >= 0) {
                timer -= DeltaTimeWork;
				if (DurationGrowTotal > 1F)
					WorkBar.UpdateBar (DurationGrowTotal - timer, DurationGrow, false);
				yield return null;
			}

			IsGrowed = true;

			LevelTaskController.I.UpdateTask (growKeyAction, 1);

			if (DurationGrowTotal > 1F)
				WorkBar.gameObject.SetActive (false);
		}

        SendedCharacters.Clear();

        PlayOnEndUse();
	}

	private IEnumerator Growing () {
		while (true) {
			while (IsRiped)
				yield return null;

            if (DurationGrowFruits > 1F)
				TakeWorkBar.gameObject.SetActive (true);

            float timer = DurationGrowFruits;
			while (timer >= 0) {
				timer -= Time.deltaTime;
                if (DurationGrowFruits > 1F)
                    TakeWorkBar.UpdateBar (DurationGrowFruits - timer, DurationGrowFruits, false);
				yield return null;
			}
				
			IsRiped = true;
            CheckShouldPing();
            AudioWrapper.I.PlaySfx("bush_ready");
            ripeEffect.Play();

            if (DurationGrowFruits > 1F)
				TakeWorkBar.gameObject.SetActive (false);
		}
	}
}
