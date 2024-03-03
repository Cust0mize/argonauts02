using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BaseInObject : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
	[HideInInspector]
	public LayerObject LayerObject;

    public string KeyAction;

    public List<Resource> RequiredResources;

	public BaseCharacter.ActionStages ActionStage;

    public Vector2 OffsetPanelsTop = new Vector2(0f, 60f);
    public Vector2 OffsetPanelsRight = new Vector2(60f, 0f);
    public Vector2 OffsetPanelsBot = new Vector2(0f, -60f);
    public Vector2 OffsetPanelsLeft = new Vector2(-60f, 0f);

    public int CountStages = 1;
    public int WaitingWorkers = 0;
    public List<BaseCharacter> SendedCharacters = new List<BaseCharacter>();

    [SerializeField] private int orderCounts;
    public int OrderCounts {
        get {
            return orderCounts;
        }
        set {
            orderCounts = value;
        }
    }
    public bool MultiplyWorkers = false;
    public int MaxMultiplyWorkers = 0;

	[SerializeField]
	GameObject flagprefab;
	[SerializeField]
	GameObject checkmarkprefab;
	[SerializeField]
	GameObject workbarprefab;
    [SerializeField] 
    protected Vector2 offsetWorkBar;
	[SerializeField]
	protected bool workPointsForeverFree;
	[SerializeField]
	protected GameObject effectUsePrefab;
    [SerializeField] 
    protected GameObject effectOnEndUse;

	protected GameObject currentUseEffect;

	GameObject checkmark;
    GameObject flag;
	HudBar workBar;

    IEnumerator noPathPulsate;
	IEnumerator disablerNoPathPulsate;
    bool isPulsating = false;

	protected virtual GameObject Checkmark {
		get {
			if (checkmark == null) {
				checkmark = Instantiate(checkmarkprefab, GameManager.I.Canvas2D.transform);
                checkmark.transform.position = transform.position - Vector3.up * 0.05F;
				checkmark.SetActive(false);
			}
			return checkmark;
		}
	}

    protected virtual ObjectFlag Flag {
        get {
            if (flag == null) {
                flag = Instantiate(flagprefab, GameManager.I.Canvas2D.transform);
                flag.transform.position = transform.position - Vector3.up * 0.05F;
                flag.SetActive(false);
            }
            return flag.GetComponent<ObjectFlag>();
        }
    }

	protected virtual HudBar WorkBar {
		get {
			if (workBar == null) {
				GameObject b = Instantiate(workbarprefab, GameManager.I.Canvas2D.transform);
				workBar = b.GetComponent<HudBar>();
				workBar.transform.position = transform.position;
				workBar.transform.position = new Vector2(workBar.transform.position.x + offsetWorkBar.x, workBar.transform.position.y + offsetWorkBar.y);
				b.SetActive(false);
			}
			return workBar;
		}
	}

	public bool CheckmarkIsPlaced {
		get {
            return Checkmark.transform.parent.gameObject.activeSelf;
		}
	}

    protected SpriteRenderer Overlay { get; set; }
    protected IEnumerator ping;

	public bool Clickable = true;
	public Node Node;
	public bool Available = false;
    public bool CanPing = true;

	public List<GameObject> WorkPoints;
	public List<bool> OccupiedPoints;
    private Dictionary<GameObject, Node> workPointNodes = new Dictionary<GameObject, Node>();

	public virtual event Action<BaseInObject> OnClick = delegate { };
	public virtual event Action<BaseInObject> OnEnter = delegate { };
    public virtual event Action<BaseInObject> OnBecameAvailable = delegate { };
    public virtual event Action<Node> OnDestroyed = delegate { };

    protected float DeltaTimeWork {
        get {
            return (GameManager.I.WorkBonusActive ? Time.deltaTime * 2 : Time.deltaTime) *
                (GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.CharacterWorkSpeed) ? 1.1F : 1F);
        }
    }

    private bool lastResourceExist;
    private bool lastPathExist;
    private bool firstAvailableChecked = false;

    protected bool noResourcePing = false;

    private List<List<Node>> blockedPaths = new List<List<Node>>();

	protected virtual void Awake() {
        if (CanPing)
            InitOverlay();

        CountStages = CountCharacters();

		InitOccupiedPoints();

		StartCoroutine(CheckAvailable());
	}

    protected virtual void OnDestroy () {
        if(GameManager.Inited){
            GameManager.I.OnResourcesChanged -= GameManagerOnResourcesChanged;
            GameManager.I.OnObjectDestroyedEvent -= GameManagerOnObjectDestroyed;
        }
    }

    private IEnumerator CheckAvailable() {
		while (!GameManager.I.GameInited)
			yield return null;
		yield return new WaitForSeconds(1F);

        if (GameManager.I.AnyTorchExist) {
            Available = GameManager.I.TorchExistInPosition(transform.position);
        } else {
            Available = true;
        }

        CalculateAllPossiblePaths();

        GameManager.I.OnResourcesChanged += GameManagerOnResourcesChanged;
        GameManager.I.OnObjectDestroyedEvent += GameManagerOnObjectDestroyed;

        GameManagerOnResourcesChanged(GameResourceManager.I.Resources);
        GameManagerOnObjectDestroyed(null);

        firstAvailableChecked = true;
	}

    private void CalculateAllPossiblePaths () {
        Node endNode = GameManager.I.CampObject.Node;

        if (RequiredResources.Exists(x => x.Type == Resource.Types.Jaison)) {
            if (GameResourceManager.I.Jason != null) endNode = GameResourceManager.I.Jason.Home.Node;
        } else if (RequiredResources.Exists(x => x.Type == Resource.Types.Medea)) {
            if (GameResourceManager.I.Medea != null) endNode = GameResourceManager.I.Medea.Home.Node;
        }

        List<List<Node>> paths = AstarAlgorithm.CalculateAllPaths(endNode, Node);
        foreach (List<Node> path in paths) {
            path.RemoveAll(x => !GameManager.I.IsObject(x));
        }
        foreach (List<Node> path in paths) {
            path.Remove(endNode);
            path.Remove(Node);
            path.Reverse();
        }
        blockedPaths = paths;
    }

    //call only torch activate object
    public void OnBecomeAvailableFromTorch() {
        if (firstAvailableChecked)
            GameManagerOnObjectDestroyed(null);
    }

    protected int CountCharacters() {
        return GameResourceManager.I.GetCountResource(Resource.Types.Worker, RequiredResources)
        + GameResourceManager.I.GetCountResource(Resource.Types.Jaison, RequiredResources)
        + GameResourceManager.I.GetCountResource(Resource.Types.Medea, RequiredResources);
    }

    private void GameManagerOnResourcesChanged (List<Resource> resources) {
        OnResourcesChanged(resources);
    }

    private void GameManagerOnObjectDestroyed (Node obj) {
        OnObjectDestroyed(obj);
    }

	protected void InitOccupiedPoints() {
		OccupiedPoints = new List<bool>();
		for (int i = 0; i < WorkPoints.Count; i++)
			OccupiedPoints.Add(false);
	}

	public virtual IEnumerator Use(Action<List<Resource>> callback) {
		yield return null;
	}

	public virtual void OnPointerClick(PointerEventData eventData) {
		OnClick.Invoke(this);
	}

	public virtual void OnPointerEnter(PointerEventData eventData) {
		OnEnter.Invoke(this);
	}

	public void OnPointerExit(PointerEventData eventData) {
        GameGUIManager.I.HideNoResourcesPanel();
		GameGUIManager.I.HideActionInfo();
	}

	public void SetCheckmarkActivity(bool value) {
        if (value)
            Flag.SetActive(false);
        
		Checkmark.SetActive(value);
    }

    public void SetFlagActivity(bool value, int number) {
        if (value)
            Checkmark.SetActive(false);

        Flag.SetNumber(number);
        Flag.SetActive(value);
    }

    public void UpdateFlag(int number) {
        Flag.SetNumber(number);
    }

    public virtual void Start() {
        
    }

    public virtual void InitOverlay () {
        if (Overlay != null)
            return;

        GameObject overlay = new GameObject("Overlay - " + name);
        overlay.transform.SetParent(gameObject.transform);
        overlay.transform.localScale = Vector3.one;
        overlay.transform.localPosition = Vector3.zero;
        overlay.transform.localEulerAngles = Vector3.zero;

        SpriteRenderer sr = overlay.AddComponent<SpriteRenderer>();
        sr.sprite = GetComponent<SpriteRenderer>().sprite;
        sr.material = ResourceManager.I.GetOverlayObjectMaterial();
        sr.color = new Color(1f, 1f, 1f, 0f);

        Overlay = sr;

        overlay.gameObject.SetActive(false);
    }

    protected virtual void OnResourcesChanged (List<Resource> resources) {
        if (OverseerController.I.ObjectInOrder(this) || !CanBeUsed() || !Available || !Clickable) return;

        lastResourceExist = GameManager.I.RequiredResourcesExistMaxCharacters(RequiredResources);

        CheckShouldPing();
    }

    protected virtual void OnObjectDestroyed (Node obj) {
        if (lastPathExist) {
            CheckShouldPing();
            return;
        }

        if (OverseerController.I.ObjectInOrder(this) || !CanBeUsed() || !Available || !Clickable) return;

        if (blockedPaths.Count.Equals(0)) {
            lastPathExist = true;
        } else {
            int countEmptyPaths = 0;
            foreach (List<Node> nodes in blockedPaths) {
                if (nodes.Count.Equals(0)) countEmptyPaths++;
            }

            if (countEmptyPaths > 0) {
                lastPathExist = true;
            } else {
                foreach (List<Node> nodes in blockedPaths) {
                    bool isFree = true;
                    foreach (Node node in nodes) {
                        if (!node.IsFree) isFree = false;
                    }

                    if (isFree) {
                        lastPathExist = true;
                        break;
                    }
                }
            }
        }

        CheckShouldPing();
    }

    public virtual void CheckShouldPing(){
        if (OverseerController.I.ObjectInOrder(this) || !CanBeUsed() || !Available || !Clickable)  {
            StopPing();
            return;
        }

        if (lastPathExist && lastResourceExist) {
            OnBecameAvailable.Invoke(this);

            StartPing();
        } else {
            StopPing();
        }
    }

    private void StartPing() {
        if (!CanPing) return;

        if (ping == null) {
            ping = Ping();
            if (CanBeUsed() && Clickable && Available)
                StartCoroutine(ping);
        }
    }

    public void StopPing () {
        if (!CanPing) return;

        if (ping != null) {
            StopCoroutine(ping);
            ping = null;
        }
        Overlay.gameObject.SetActive(false);
    }

	public bool FreePointExist() {
		foreach (bool b in OccupiedPoints)
			if (!b)
				return true;
		return false;
	}

	public virtual bool EnoughResources() {
		if (RequiredResources.Count.Equals(0))
			return true;
		return GameManager.I.RequiredResourcesExist(RequiredResources);
	}

    public virtual bool EnoughResourcesWithCharacters () {
        if (RequiredResources.Count.Equals(0))
            return true;
        return GameManager.I.RequiredResourcesExistMaxCharacters(RequiredResources);
    }

    public virtual bool EnoughCharacters() {
        if (RequiredResources.Count.Equals(0))
            return true;
        return GameManager.I.RequiredCharactersExits(RequiredResources);
    }

	public virtual bool CanBeUsed() {
		return true;
	}

	public GameObject GetFirstFreePoint(Vector3 pos, bool occupy = true) {
		float distance = float.MaxValue;
		int index = -1;

		int countPoints = OccupiedPoints.Count;
		for (int i = 0; i < countPoints; i++) {
			if (!OccupiedPoints[i]) {
				float d = Vector3.Distance(WorkPoints[i].transform.position, pos);

				if (d < distance) {
					index = i;
					distance = d;
				}
			}
		}

		if (index == -1)
			return null;
		else {
			if (occupy) {
				if (!workPointsForeverFree) {
					OccupiedPoints[index] = true;
				}
			}
			return WorkPoints[index];
		}
	}

    public Node GetFirstFreePointAsNode(Vector3 pos, bool occupy = true) {
        GameObject workPoint = GetFirstFreePoint(pos, occupy);
        if (workPoint == null) return null;
        if (!workPointNodes.ContainsKey(workPoint)) {
            workPointNodes.Add(workPoint, new Node(workPoint.transform.position));
        }
        return workPointNodes[workPoint];
    }

    public GameObject GetFirstWorkPoint() {
		return WorkPoints[0];
	}

    public void FreePoint(GameObject g) {
		int countPoints = WorkPoints.Count;
		for (int i = 0; i < countPoints; i++)
			if (WorkPoints[i] == g)
				OccupiedPoints[i] = false;
	}

    public void PingNoResource() {
        StopPing();
        StartCoroutine(NoResourcePing(3, 10f));
    }

	public void StartNoPathPulsate(int pulsationCount = 2, float pulsationSpeed = 3f) {
        if (noPathPulsate != null || isPulsating){
            StopNoPathPulsate();
            return;
        }
        
        noPathPulsate = NoPathPulsate(pulsationCount, pulsationSpeed);
		StartCoroutine(noPathPulsate);
	}

	public void StopNoPathPulsate() {
		if (noPathPulsate != null)
			StopCoroutine(noPathPulsate);

		SpriteRenderer s = GetComponent<SpriteRenderer>();
		s.color = new Color(1f, 1f, 1f, 1F);

		noPathPulsate = null;
        isPulsating = false;
	}

    protected virtual IEnumerator NoResourcePing(int pulsationCount, float pulsationSpeed) {
        if (Overlay == null) yield break;

        Overlay.gameObject.SetActive(true);

        Color col = Overlay.color;

        noResourcePing = true;

        bool increase = true;
        int pulsationCountCurrent = 0;
        float value = 0F;

        while (pulsationCountCurrent < pulsationCount) {
            if (increase) {
                if (value > 1F)
                    increase = false;
                else
                    value += Time.fixedDeltaTime * pulsationSpeed;
            } else {
                if (value < 0F) {
                    increase = true;
                    pulsationCountCurrent++;
                } else
                    value -= Time.fixedDeltaTime * pulsationSpeed;
            }

            col.a = value;
            Overlay.color = col;

            yield return null;
        }

        noResourcePing = false;

        StopPing();

        if (lastPathExist && lastResourceExist) {
            StartPing();
        }
    }

    private IEnumerator NoPathPulsate (int pulsationCount, float pulsationSpeed) {
        SpriteRenderer s = GetComponent<SpriteRenderer>();
        s.color = new Color(1f, 1f, 1f, s.color.a);

        bool increase = true;
        int pulsationCountCurrent = 0;
        float value = 0F;

        while (pulsationCountCurrent < pulsationCount) {
			s.color = Color.Lerp(Color.white, new Color(1.0f, 0.3f, 0.3f), value);

            if (increase) {
                if (value > 1F)
                    increase = false;
                else
                    value += Time.fixedDeltaTime * pulsationSpeed;
            } else {
                if (value < 0F)
                {
                    increase = true;
                    pulsationCountCurrent++;
                }
                else
                    value -= Time.fixedDeltaTime * pulsationSpeed;
            }

            yield return null;
        }
    }

    protected IEnumerator Ping () {
        bool increase = true;
        Color col = Overlay.color;

        Overlay.gameObject.SetActive(true);

        while (noResourcePing) {
            yield return null;
        }

        while (true) {
            if (increase) {
                col.a += 0.5F * Time.fixedDeltaTime;
                if (col.a >= 0.5F)
                    increase = false;
            } else {
                col.a -= 0.5F * Time.fixedDeltaTime;
                if (col.a <= 0)
                    increase = true;
            }
            Overlay.color = col;
            yield return null;
        }
    }

    public virtual void PlayOnEndUse(){
        if (effectOnEndUse == null) return;
        GameObject effect = Instantiate(effectOnEndUse, transform.position + Vector3.up * 0.01f, Quaternion.identity);
        effect.GetComponent<ParticleSystem>().Play(true);
    }

    private Color AnalyzeColor (Texture2D tex) {
        float r = 0;
        float g = 0;
        float b = 0;
        for (int x = 0; x < 20; x++) {
            for (int y = 0; y < 20; y++) {

                float ux = x / 20f;
                float uy = y / 20f;
                Color c = tex.GetPixelBilinear(ux, uy);
                c.a = 1f;
                r += c.r;
                g += c.g;
                b += c.b;
            }
        }

        Color cr = Color.white;
        cr.a = 1f;
        cr.r = r / 200f;
        cr.g = g / 200f;
        cr.b = b / 200f;

        return cr;
    }

    public virtual void PlayOneShotUseEffect(Vector3 positionUse, Vector3 positionChar, bool checkOrder = true) {
        if (effectUsePrefab == null) return;
        GameObject effect = Instantiate(effectUsePrefab, positionUse, Quaternion.identity);

        if (checkOrder) {
            effect.GetComponent<Renderer>().sortingOrder = (transform.position.y < positionChar.y) ? -1 : 1;
            if (effect.transform.childCount > 0)
                effect.transform.GetChild(0).GetComponent<Renderer>().sortingOrder = (transform.position.y < positionChar.y) ? -1 : 1;
        }

        effect.GetComponent<ParticleSystem>().Play(true);
    }
}