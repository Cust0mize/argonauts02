using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Analytics;

public class GameManager : LocalSingletonBehaviour<GameManager> {
	[SerializeField] GameObject[] fogObjects;

	private float time;
    [SerializeField] private int scores;

    public float PassedTime {
        get {
            return time;
        }
    }

    public int Scores {
        get {
            return scores;
        } private set {
            scores = value;
            OnScoreChanged.Invoke(value);
        }
    }

    public float LevelTime { get; private set; }
    public float LevelStar1 { get; private set; }
    public float LevelStar2 { get; private set; }
    public float LevelStar3 { get; private set; }
    public int LevelNumber { get; private set; }

	public GameObject Canvas2D;
    public GameObject CanvasUI;

	public Graph Graph { get; private set; }

	private Dictionary<string, Node> PointNodes = new Dictionary<string, Node>();

	private BuildObjectController mainTentController;

	public event Action<float> OnTimeChanged = delegate { };

	private Dictionary<Node, BaseInObject> Objects = new Dictionary<Node, BaseInObject>();
	private Dictionary<string, BaseInObject> ObjectsOnID = new Dictionary<string, BaseInObject>();

	private OverseerController inputUserHandler;

	public bool GameInited {
		get {
			return inited;
		}
	}

	public OverseerController InputUserHandler {
		get {
			return OverseerController.I;
		}
	}

    private bool isPause = true;
	public bool IsPause {
		get {
			return isPause;
		}
		set {
			isPause = value;
			Time.timeScale = value ? 0f : 1f;
		}
	}

    private bool isFinishPanel = false;
    public bool IsFinishPanel {
        get {
            return isFinishPanel;
        }
        set {
            isFinishPanel = value;
        }
    }

	public List<BuildObjectController> PortalControllers = new List<BuildObjectController>();
    private List<BuildObjectController> TorchControllers = new List<BuildObjectController>();
    public List<BuildObjectController> PortControllers = new List<BuildObjectController>();

    public List<PortalObject> Portals {
		get {
			List<PortalObject> portals = new List<PortalObject>();
			foreach (BuildObjectController b in PortalControllers) {
				foreach (BuildObjectController bb in PortalControllers) {
					if (bb.BaseBuildObject.name.ToLower().Contains(b.BaseBuildObject.name.ToLower()) && bb != b) {
						bb.BaseBuildObject.GetComponent<PortalObject>().MirrorPortalController = b;
						b.BaseBuildObject.GetComponent<PortalObject>().MirrorPortalController = bb;
						break;
					}
				}
				portals.Add(b.BaseBuildObject.GetComponent<PortalObject>());
			}
			return portals;
		}
	}

    public List<PortObject> Ports {
        get {
            List<PortObject> result = new List<PortObject>();
            foreach (BuildObjectController b in PortControllers) {
                result.Add(b.BaseBuildObject.GetComponent<PortObject>());
            }
            return result;
        }
    }

    private List<TorchObject> Torchs {
        get {
            List<TorchObject> result = new List<TorchObject>();
            foreach (BuildObjectController b in TorchControllers) {
                result.Add(b.BaseBuildObject.GetComponent<TorchObject>());
            }
            return result;
        }
    }

    public event Action<List<Resource>, bool> OnResourcesAdded = delegate { };
    public event Action<List<Resource>, BaseInObject> OnResourcesTaked = delegate { };
    public event Action<List<Resource>> OnResourcesChanged = delegate { };
	public event Action<bool> OnLevelWasEnd = delegate { };
    public event Action<Node> OnObjectDestroyedEvent = delegate { };
    public event Action<string> OnBonusCameAvailable = delegate { };
    public event Action<string> OnBonusCameActive = delegate { };
    public event Action<int> OnScoreChanged = delegate { };

    public List<List<Node>> riverPaths = new List<List<Node>>();

    private bool inited = false;
	public bool AnyTorchExist = false;

	private float resourceCooldownBonusDown;
	private float speedCooldownBonusDown;
	private float workerCooldownBonusDown;
	private float workCooldownBonusDown;

	private float resourceDurationBonusDown;
	private float speedDurationBonusDown;
	private float workerDurationBonusDown;
	private float workDurationBonusDown;

    private bool resourceBonusLastAvailable;
    private bool speedBonusLastAvailable;
    private bool workerBonusLastAvailable;
    private bool workBonusLastAvailable;

	[SerializeField] private float resourceBonusCooldown;
	[SerializeField] private float speedBonusCooldown;
	[SerializeField] private float workerBonusCooldown;
	[SerializeField] private float workBonusCooldown;

	[SerializeField] private float resourceBonusDuration;
	[SerializeField] private float speedBonusDuration;
	[SerializeField] private float workerBonusDuration;
	[SerializeField] private float workBonusDuration;

    private float ResourceBonusCooldown {
        get {
            return resourceBonusCooldown
                 * (GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.LevelBonusCooldown) ? 0.9F : 1F);
        }
    }

    private float SpeedBonusCooldown {
        get {
            return speedBonusCooldown
                 * (GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.LevelBonusCooldown) ? 0.9F : 1F);
        }
    }

    private float WorkerBonusCooldown {
        get {
            return workerBonusCooldown
                 * (GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.LevelBonusCooldown) ? 0.9F : 1F);
        }
    }

    private float WorkBonusCooldown {
        get {
            return workBonusCooldown
                 * (GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.LevelBonusCooldown) ? 0.9F : 1F);
        }
    }

    private float ResourceBonusDuration {
        get {
            return resourceBonusDuration
                * (GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.LevelBonusDuration) ? 1.1F : 1F);
        }
    }

    private float SpeedBonusDuration {
        get {
            return speedBonusDuration
                * (GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.LevelBonusDuration) ? 1.1F : 1F);
        }
    }

    private float WorkerBonusDuration {
        get {
            return workerBonusDuration
                * (GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.LevelBonusDuration) ? 1.1F : 1F);
        }
    }

    private float WorkBonusDuration {
        get {
            return workBonusDuration
                * (GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.LevelBonusDuration) ? 1.1F : 1F);
        }
    }

    [HideInInspector] public bool ResourceBonusNeed;
	[HideInInspector] public bool SpeedBonusNeed;
	[HideInInspector] public bool WorkerBonusNeed;
	[HideInInspector] public bool WorkBonusNeed;

	[HideInInspector] public bool ResourceBonusActive;
	[HideInInspector] public bool SpeedBonusActive;
	[HideInInspector] public bool WorkerBonusActive;
	[HideInInspector] public bool WorkBonusActive;

    [SerializeField] private GameObject resourceBonusObject;
    [SerializeField] private GameObject speedBonusObject;
    [SerializeField] private GameObject workerBonusObject;
    [SerializeField] private GameObject workBonusObject;

    [SerializeField] RectTransform scoreRectTransform;
    [SerializeField] RectTransform timeBarPoint;

    private GameObject parentObjects;
    public GameObject ParentObjects {
        get {
            if (!parentObjects) {
                parentObjects = new GameObject("Objects");
                parentObjects.transform.position = Vector3.zero;
            }
            return parentObjects;
        }
    }

	public CampObject CampObject {
		get {
			if (mainTentController != null) {
				return (mainTentController.BaseBuildObject as CampObject);
			}
			return null;
		}
	}

    public bool IsInit {
        get {
            return inited;
        }
    }

    public bool GameStarted {
        get; private set;
    }

    public bool PanelStartClosed {
        get; private set;
    }

    public bool GameIsEnd {
        get; private set;
    }

    private int StartCountGems;

	public void DoInit(Level level, int levelNumber) {
        LevelNumber = levelNumber;

		LevelTime = level.TotalTime;
        LevelStar1 = level.Time1;
        LevelStar2 = level.Time2;
        LevelStar3 = level.Time3;

		resourceCooldownBonusDown = 0F;
		speedCooldownBonusDown = 0F;
		workerCooldownBonusDown = 0F;
		workCooldownBonusDown = 0F;

		ResourceBonusNeed = level.ResourceBonus;
		SpeedBonusNeed = level.SpeedBonus;
		WorkerBonusNeed = level.WorkerBonus;
		WorkBonusNeed = level.WorkBonus;

		ObjectLayer objectLayer = level.LayerContainer.Layers[0] as ObjectLayer;
		PathLayer pathLayer = level.LayerContainer.Layers[1] as PathLayer;

		#region graph parse

		Graph = new Graph();
		Dictionary<string, Node> existNodes = new Dictionary<string, Node>();

		foreach (PathPoint p in pathLayer.PathGraph.Points) {
			existNodes.Add(p.ID, new Node(p.Position));
			PointNodes.Add(p.ID, existNodes[p.ID]);
		}

		foreach (PathJoint j in pathLayer.PathGraph.Joints) {
			Graph.AddNode(existNodes[j.Point1ID], existNodes[j.Point2ID]);
		}

		#endregion

		foreach (string s in objectLayer.LayerObjectContainer.Objects.Keys) {
			#region graph parse

			Node nodeObject = new Node(objectLayer.LayerObjectContainer.Objects[s].Position);
			nodeObject.IsFree = false;

			foreach (string p in objectLayer.LayerObjectContainer.Objects[s].PointIDs) {
				Graph.AddNode(nodeObject, existNodes[p]);
			}

			#endregion

			if (objectLayer.LayerObjectContainer.Objects[s].Name.ToLower().Contains("jason")) {
				StartCoroutine(DelaySpawnJason(objectLayer.LayerObjectContainer.Objects[s]));
				continue;
			} else if (objectLayer.LayerObjectContainer.Objects[s].Name.ToLower().Contains("medea")) {
				StartCoroutine(DelaySpawnMedea(objectLayer.LayerObjectContainer.Objects[s]));
				continue;
			} else if (objectLayer.LayerObjectContainer.Objects[s].Name.ToLower().Contains("worker")) {
				StartCoroutine(DelaySpawnWorker(objectLayer.LayerObjectContainer.Objects[s]));
				continue;
			} else if (objectLayer.LayerObjectContainer.Objects[s].Name.ToLower().Contains("torch")) {
				AnyTorchExist = true;
			}

            //Debug.Log("TwoWorkers: " + GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.TwoWorkers));

            if (objectLayer.LayerObjectContainer.Objects[s].Name.ToLower().Equals("tent01") &&
                GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.TwoWorkers)) {
                objectLayer.LayerObjectContainer.Objects[s].Name = "tent02";
            }

            BaseInObject obj = SpawnObject(objectLayer.LayerObjectContainer.Objects[s]).GetComponent<BaseInObject>();
			obj.Node = nodeObject;

			if (obj.GetComponent<BuildObject>() != null) {
				GameObject buildObjectController = new GameObject("BuildObjectController, " + s);

                string keyNextUpgrade = string.Format("{0}{1:00}",
                                                      objectLayer.LayerObjectContainer.Objects[s].Name.Substring(0, objectLayer.LayerObjectContainer.Objects[s].Name.Length - 2),
                                                      obj.GetComponent<BuildObject>().Upgrade + 1);

                ResourceManager.I.GetObjectPrefab(keyNextUpgrade); //cache

				BuildObjectController buildController = buildObjectController.AddComponent<BuildObjectController>();
				buildController.Flip = objectLayer.LayerObjectContainer.Objects[s].Flip;
				buildController.BaseBuildObject = obj.GetComponent<BuildObject>();
                obj.GetComponent<BuildObject>().BuildObjectController = buildController;

				if (obj.GetComponent<ProductionBuildObject>() != null) {
                    if (objectLayer.LayerObjectContainer.Objects[s].JoinObjectsIDs.Count.Equals(0))
						throw new Exception("Для добывающих зданий требуется joined drop placement!!!");
                    buildController.DropPointID = objectLayer.LayerObjectContainer.Objects[s].JoinObjectsIDs[0];
				}

				if (obj.GetComponent<PortalObject>() != null) {
					PortalControllers.Add(buildController);
				}

                if (obj.GetComponent<PortObject>() != null) {
                    PortControllers.Add(buildController);
                    //set out/in node for port
                    buildController.CustomData = GetPointNodes(objectLayer.LayerObjectContainer.Objects[s].SpecialPointsID);
                }

                if (obj.GetComponent<TorchObject>() != null) {
                    TorchControllers.Add(buildController);
                }

				if (objectLayer.LayerObjectContainer.Objects[s].Name.ToLower().Equals("tent01") ||
					objectLayer.LayerObjectContainer.Objects[s].Name.ToLower().Equals("tent02") ||
					objectLayer.LayerObjectContainer.Objects[s].Name.ToLower().Equals("tent03")) {
					mainTentController = buildController;
				}
			} else if (obj.GetComponent<SwitchObject>() != null) {
				obj.GetComponent<SwitchObject>().BridgeID = objectLayer.LayerObjectContainer.Objects[s].JoinObjectsIDs[0];
            } else if (obj.GetComponent<KeyObject>() != null) {
                obj.GetComponent<KeyObject>().GateID = objectLayer.LayerObjectContainer.Objects[s].JoinObjectsIDs[0];
            }

			Objects.Add(nodeObject, obj);
			ObjectsOnID.Add(s, obj);
		}

		foreach (GameObject f in fogObjects) {
			f.SetActive(AnyTorchExist);
		}


        if (LevelNumber == 40) {
            UserData.I.SetGems(23);
        }

        if (LevelNumber == 50) {
            UserData.I.SetGems(13);
        }

        StartCountGems = UserData.I.GetGems();

        AddResources(new List<Resource>() {
            new Resource (Resource.Types.Food, level.StartFood),
            new Resource (Resource.Types.Stone, level.StartStone),
            new Resource (Resource.Types.Wood, level.StartWood),
            new Resource (Resource.Types.Gold, level.StartGold),
            new Resource(Resource.Types.Gem, StartCountGems)
		});

        LevelTaskController.I.OnTaskComplete += OnTaskComplete;

		inited = true;
		GameGUIManager.I.CalculateStarPositions(level);

        CalculateRiverPaths();

        OnTimeChanged.Invoke(0f);
	}

    private void Update () {
        if (Input.GetMouseButtonDown(0) && !GameStarted && GameInited && PanelStartClosed) {
            StartLevel();
        }
    }

    private void CalculateRiverPaths()
    {
        foreach (PortObject portFrom in Ports) {
            foreach (PortObject portTo in Ports) {
                if (portFrom == portTo) continue;

                List<List<Node>> paths = AstarAlgorithm.CalculateAllPaths(portFrom.Node, portTo.Node);

                foreach (List<Node> path in paths) {
                    ClearPortsInPath(path);
                }

                paths.RemoveAll(x => PathContainsObjects(x));
                riverPaths.AddRange(paths);
            }
        }
    }

    public bool OnRiver(Node node)
    {
        foreach (List<Node> n in riverPaths) {
            if (n.Contains(node)) return true;
        }
        return false;
    }

    private void ClearPortsInPath(List<Node> path)
    {
        path.RemoveAll(x => IsPort(x));
    }

    private bool PathContainsObjects(List<Node> path)
    {
        return path.Exists(x => NodeContainsObjects(x));
    }

    private bool NodeContainsObjects(Node node)
    {
        if (IsObject(node)) return true;
        return node.IncidentNodes.Where(x => !IsPort(x) && IsObject(x)).Count() > 0;
    }

    public void AddTimeScore() {
        int score = (int)(Mathf.Abs(LevelTime - PassedTime) * 1);
        GameGUIManager.I.ShowEndPopupScores(timeBarPoint.position, scoreRectTransform.position, score, () => { Scores += score; });
    }

    public void OnPanelStartOkClicked(){
        PanelStartClosed = true;
        GameGUIManager.I.ShowStartTipText();
        LevelTaskPingController.I.StartPing();
        GameGUIManager.I.ShowRetractablePanels();
    }

    private void StartLevel () {
#if _ANALYTICS

        switch (LevelNumber) {
            case 5:
            case 15:
            case 25:
            case 35:
            case 45:
            case 55:
                AnalyticsEvent.Custom("level_start", new Dictionary<string, object> { { "level_number", LevelNumber } });
                break;
        }
#endif

        GameGUIManager.I.HideStartTipText();
        LevelTaskPingController.I.PingEnable = true;
        TutorialManager.I.LoadTutorial(LevelNumber);
        StartCoroutine(TimeGhanger());
        StartCoroutine(BonusTimer());
        GameStarted = true;
    }

    private void OnTaskComplete(Task task) {
        AudioWrapper.I.PlaySfx("complete_task");

		if (LevelTaskController.I.AllTaskDone()) {
            DoFinishLevel();
		}
	}

    private void DoFinishLevel(int stars = -1) {
        foreach (BaseCharacter bc in GameResourceManager.I.Characters) {
            bc.BeHappy();
        }
        if (GameResourceManager.I.Jason != null) GameResourceManager.I.Jason.BeHappy();
        if (GameResourceManager.I.Medea != null) GameResourceManager.I.Medea.BeHappy();

        CampObject.ReleaseWorkers();

        UserData.I.SetGems(GameResourceManager.I.GetCountResource(Resource.Types.Gem, GameResourceManager.I.Resources));

        if (stars == -1) {
            stars = 0;
            if (PassedTime <= LevelStar3) stars = 3;
            else if (PassedTime <= LevelStar2) stars = 2;
            else if (PassedTime <= LevelStar1) stars = 1;
        }

        if (stars == 3) {
            if (!UserData.I.GetLevel3StarsStatus(LevelNumber)) {
                AwardHandler.I.UpdateAward("award_stars", 1);
                UserData.I.SaveLevel3StarsStatus(LevelNumber, true);
            }
        }

        GameBonusesProgressManager.I.AddStars(stars, LevelNumber);

        OnLevelWasEnd.Invoke(true);

        GameIsEnd = true;
#if _ANALYTICS

        switch (LevelNumber) {
            case 5:
            case 15:
            case 25:
            case 35:
            case 45:
            case 55:
                AnalyticsEvent.Custom("level_complete", new Dictionary<string, object> {
                    { "level_number", LevelNumber },
                    { "stars_count", stars }
                });
                break;
        }
#endif
    }

    public void DoPassLevel(int stars) {
        LevelTaskController.I.OnTaskComplete -= OnTaskComplete;

        LevelTaskController.I.DoAllTasks();

        switch (stars) {
            case 3: time = LevelStar3 - 1; break;
            case 2: time = LevelStar2 - 1; break;
            case 1: time = LevelStar1 - 1; break;
            case 0: time = LevelStar1 + 1; break;
        }

        OnTimeChanged.Invoke(time);

        DoFinishLevel(stars);
    }

    public void FillBonuses() {
        resourceCooldownBonusDown = ResourceBonusCooldown;
        speedCooldownBonusDown = SpeedBonusCooldown;
        workerCooldownBonusDown = WorkerBonusCooldown;
        workCooldownBonusDown = WorkBonusCooldown;
    }

	public Node GetPointNode(string pointID) {
		if (PointNodes.ContainsKey(pointID))
			return PointNodes[pointID];
		return null;
	}

    public List<Node> GetPointNodes(List<string> pointIDs)
    {
        List<Node> result = new List<Node>();
        foreach (string id in pointIDs) {
            if (PointNodes.ContainsKey(id))
                result.Add(PointNodes[id]);
        }
        return result;
    }

    public void AddResources(List<Resource> resource, bool action = false) {
        int food = 0, wood = 0, stone = 0, gold = 0, gems = 0;
        foreach (Resource res in resource) {
            switch (res.Type) {
                case Resource.Types.Food:
                    food += res.Count;
                    break;
                case Resource.Types.Wood:
                    wood += res.Count;
                    break;
                case Resource.Types.Stone:
                    stone += res.Count;
                    break;
                case Resource.Types.Gold:
                    gold += res.Count;
                    break;
                case Resource.Types.Gem:
                    gems += res.Count;
                    break;
            }
        }

        AwardHandler.I.UpdateAward("award_food", food);
        AwardHandler.I.UpdateAward("award_wood", wood);
        AwardHandler.I.UpdateAward("award_stone", stone);
        AwardHandler.I.UpdateAward("award_gold", gold);

		GameResourceManager.I.AddResources(resource);
        OnResourcesAdded.Invoke(resource, action);
        GameManager.I.DoResourcesChanged(action);
	}

    public void TakeResources(List<Resource> resource, BaseInObject baseObject) {
		GameResourceManager.I.TakeResources(resource);
        OnResourcesTaked.Invoke(GameResourceManager.I.ChangeSign(resource), baseObject);
        DoResourcesChanged(true);
	}

    public void DoResourcesChanged(bool action = false) {
        int food = 0, wood = 0, stone = 0, gold = 0, gems = 0;
        foreach (Resource res in GameResourceManager.I.Resources) {
            switch (res.Type) {
                case Resource.Types.Food:
                    food += res.Count;
                    break;
                case Resource.Types.Wood:
                    wood += res.Count;
                    break;
                case Resource.Types.Stone:
                    stone += res.Count;
                    break;
                case Resource.Types.Gold:
                    gold += res.Count;
                    break;
                case Resource.Types.Gem:
                    gems += res.Count;
                    break;
            }
        }

        LevelTaskController.I.UpdateTaskSet("act_take_food", food);
        LevelTaskController.I.UpdateTaskSet("act_take_wood", wood);
        LevelTaskController.I.UpdateTaskSet("act_take_stone", stone);
        LevelTaskController.I.UpdateTaskSet("act_take_gold", gold);

        OnResourcesChanged.Invoke(GameResourceManager.I.Resources);
	}

	public bool RequiredResourcesExist(List<Resource> resources) {
		return GameResourceManager.I.RequiredResourcesExist(resources);
	}

	public bool RequiredResourcesExistMaxCharacters(List<Resource> resources) {
		return GameResourceManager.I.RequiredResourcesExistMaxCharacters(resources);
	}

    public bool RequiredCharactersExits(List<Resource> resources) {
        return GameResourceManager.I.RequiredCharactersExits(resources);
    }

    public void AddObject(BaseInObject obj) {
        if (Objects.ContainsKey(obj.Node)) Objects[obj.Node] = obj;
        else Objects.Add(obj.Node, obj);
    }

	public List<BaseInObject> GetObjects(List<Node> nodes) {
		List<BaseInObject> objects = new List<BaseInObject>();
		foreach (Node n in nodes) {
			if (Objects.ContainsKey(n)) {
				objects.Add(Objects[n]);
			}
		}
		return objects; 
	}

	public BaseInObject GetObject(string objectID) {
		if (ObjectsOnID.ContainsKey(objectID)) {
			return ObjectsOnID[objectID];
		}
		return null;
	}

    public bool IsPort(Node node)
    {
        if (Objects.ContainsKey(node)) {
            return Objects[node] is PortObject;
        }
        return false;
    }

    public bool IsObject (Node node) {
        return Objects.ContainsKey(node);
    }

    public BaseInObject GetObject(Node node) {
        return Objects[node];
    }

    public List<BaseInObject> GetObjectsOnActionKey (string actionKey) {
        return Objects.Values.Where(x => x.KeyAction.Equals(actionKey)).ToList();
    }

    public List<BaseInObject> GetObjectsOnStartWithActionKey(string startWithActionKey) {
        return Objects.Values.Where(x => x.KeyAction.StartsWith(startWithActionKey, StringComparison.CurrentCultureIgnoreCase)).ToList();
    }

    public List<BaseInObject> GetObjectsWithResource (Resource.Types resourceType) {
        return Objects.Values.Where(x => (x != null) && (
                                    ((x is NormalResourceObject) && ((x as NormalResourceObject).Resources.Exists(v => v.Type.Equals(resourceType))) && (((x is BushObject) ? (x as BushObject).IsRiped : true))) 

                                    ||

                                    ((x is ProductionBuildObject) && ((x as ProductionBuildObject).DropResources.Exists(v => v.Type.Equals(resourceType))) && ((x as ProductionBuildObject).Upgrade > 0))

                                    ||

                                    ((x is MarketPlaceObject) && ((x as MarketPlaceObject).Resources.Exists(v => v.Type.Equals(resourceType)))))
                                   ).ToList();
    }

    public BaseInObject GetNearestObject(Vector3 position, string namePart = "") {
        return Objects.Values.Where(x => (x != null) && (x.gameObject.name.ToLower().Contains(namePart.ToLower()))).OrderBy(x => Vector3.Distance(position, x.transform.position)).FirstOrDefault();
    }

    public BaseInObject GetObjectOnNamePart(string namePart) {
        return Objects.Values.Where(x => (x != null) && (x.gameObject.name.ToLower().Contains(namePart.ToLower()))).FirstOrDefault();
    }

    public List<BaseInObject> GetNearestObjects(Vector3 position, string namePart = "") {
        return Objects.Values.Where(x => (x != null) && (x.gameObject.name.ToLower().Contains(namePart.ToLower()))).OrderBy(x => Vector3.Distance(position, x.transform.position)).ToList();
    }

	public void StopNoPathPulsationAll() {
		foreach (BaseInObject obj in Objects.Values) {
			if (obj != null)
				obj.StopNoPathPulsate(); 
		}
	}

	public GameObject SpawnObject(LayerObject obj) {
        GameObject g = Instantiate(ResourceManager.I.GetObjectPrefab(obj.Name));
        g.transform.parent = ParentObjects.transform;
		g.transform.position = obj.Position;
		g.transform.localScale = new Vector3(obj.Flip ? -1 : 1, 1, 1);
		g.GetComponent<BaseInObject>().OnClick += InputUserHandler.OnClickObject;
		g.GetComponent<BaseInObject>().OnDestroyed += OnObjectDestroyed;
		return g;
	}

	public GameObject SpawnObject(string fullKey, BaseInObject baseObject) {
		GameObject g = Instantiate(ResourceManager.I.GetObjectPrefab(fullKey));
        g.transform.parent = ParentObjects.transform;
		g.transform.position = baseObject.transform.position;
		g.transform.rotation = baseObject.transform.rotation;
		g.GetComponent<BaseInObject>().Node = baseObject.Node;
		g.transform.localScale = new Vector3(baseObject.LayerObject.Flip ? -1 : 1, 1, 1);
		g.GetComponent<BaseInObject>().OnClick += InputUserHandler.OnClickObject;
		g.GetComponent<BaseInObject>().OnDestroyed += OnObjectDestroyed;
		return g;
	}

	public GameObject SpawnObject(string fullKey, Node node) {
		GameObject g = Instantiate(ResourceManager.I.GetObjectPrefab(fullKey));
		g.GetComponent<BaseInObject>().Node = node;
        g.transform.parent = ParentObjects.transform;
		g.transform.position = node.Position;
		g.GetComponent<BaseInObject>().OnClick += InputUserHandler.OnClickObject;
		g.GetComponent<BaseInObject>().OnDestroyed += OnObjectDestroyed;
		return g;
	}

    public GameObject SpawnObjectItem(Resource.Types type, int upgrade, Vector3 position, Node from, Node node) {
        GameObject g = Instantiate(ResourceManager.I.GetObjectPrefab(string.Format("{0}{1:00}", type.ToString(), upgrade)));

        if(node == null) {
            node = new Node(position);

            PointNodes.Add(Guid.NewGuid().ToString(), node);
            Graph.AddNode(from, node);
        }

        node.IsFree = false;

        g.GetComponent<BaseInObject>().Node = node;
        g.transform.parent = ParentObjects.transform;
        g.transform.position = position;
        g.GetComponent<BaseInObject>().OnClick += InputUserHandler.OnClickObject;
        g.GetComponent<BaseInObject>().OnDestroyed += OnObjectDestroyed;

        AddObject(g.GetComponent<BaseInObject>());

        return g;
    }

    public void AddScores(Vector3 pos, int additionalScores) {
        Scores += additionalScores;
        if (additionalScores > 0) {
            GameGUIManager.I.ShowPopupScores(pos, additionalScores);
        }
    }

    public void OnObjectDestroyed(Node obj) {
        OnObjectDestroyedEvent.Invoke(obj);
	}

	public void ActivateObjectsInPosition(Vector3 pos, float radius) {
		foreach (BaseInObject obj in Objects.Values) {
			if (obj == null || obj.gameObject == null)
				continue;

			if (Vector3.Distance(new Vector3(obj.transform.position.x, obj.transform.position.y, 0F), new Vector3(pos.x, pos.y, 0f)) <= radius) {
				obj.Available = true;
                obj.OnBecomeAvailableFromTorch();
			}
		}
	}

	public bool TorchExistInPosition(Vector3 pos) {
        foreach (TorchObject obj in Torchs) {
            if (obj.Upgrade.Equals(1) && Vector3.Distance(new Vector3(obj.transform.position.x, obj.transform.position.y, 0F), new Vector3(pos.x, pos.y, 0f)) <= 500F) {
				return true;
			}
		}
		return false;
	}

    public void AddFillBonusPrecent(float precent) {
        if (ResourceBonusNeed) {
            resourceCooldownBonusDown = Mathf.Clamp(resourceCooldownBonusDown + (ResourceBonusCooldown * precent / 100F), 0F, ResourceBonusCooldown);
            GameGUIManager.I.ResourceBar.PlayPuffEffect();
        }
        if (SpeedBonusNeed) {
            speedCooldownBonusDown = Mathf.Clamp(speedCooldownBonusDown + (SpeedBonusCooldown * precent / 100F), 0F, SpeedBonusCooldown);
            GameGUIManager.I.SpeedBar.PlayPuffEffect();
        }
        if (WorkerBonusNeed) {
                workerCooldownBonusDown = Mathf.Clamp(workerCooldownBonusDown + (WorkerBonusCooldown * precent / 100F), 0F, WorkerBonusCooldown);
            GameGUIManager.I.WorkerBar.PlayPuffEffect();
        }
        if (WorkBonusNeed) {
            workCooldownBonusDown = Mathf.Clamp(workCooldownBonusDown + (WorkBonusCooldown * precent / 100F), 0F, WorkBonusCooldown);
            GameGUIManager.I.WorkBar.PlayPuffEffect();
        }
    }

	public void TryActiveResourceBonus() {
		if (resourceCooldownBonusDown >= ResourceBonusCooldown && !ResourceBonusActive) {
			resourceDurationBonusDown = ResourceBonusDuration;
			resourceCooldownBonusDown = 0f;
			ResourceBonusActive = true;

            GameGUIManager.I.ResourceBar.StopAnimation();

            OnBonusCameActive.Invoke("Resource");
            AudioWrapper.I.PlaySfx("bonus_used");
            resourceBonusLastAvailable = false;

            Vector3 point = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(resourceBonusObject.GetComponent<RectTransform>(),
                                                                    resourceBonusObject.GetComponent<RectTransform>().position,
                                                                    Camera.main, out point);
            AddScores(point, (int)(ResourceBonusCooldown * 10));

            speedCooldownBonusDown = Mathf.Clamp(speedCooldownBonusDown - SpeedBonusCooldown * 0.5F, 0F, SpeedBonusCooldown);
            workerCooldownBonusDown = Mathf.Clamp(workerCooldownBonusDown - WorkerBonusCooldown * 0.5F, 0F, WorkerBonusCooldown);
            workCooldownBonusDown = Mathf.Clamp(workCooldownBonusDown - WorkBonusCooldown * 0.5F, 0F, WorkBonusCooldown);
		}
	}

	public void TryActiveSpeedBonus() {
		if (speedCooldownBonusDown >= SpeedBonusCooldown && !SpeedBonusActive) {
			speedDurationBonusDown = SpeedBonusDuration;
			speedCooldownBonusDown = 0f;
			SpeedBonusActive = true;

            GameGUIManager.I.SpeedBar.StopAnimation();

            OnBonusCameActive.Invoke("Speed");
            AudioWrapper.I.PlaySfx("bonus_used");
            speedBonusLastAvailable = false;

            Vector3 point = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(speedBonusObject.GetComponent<RectTransform>(),
                                                                    speedBonusObject.GetComponent<RectTransform>().position,
                                                                    Camera.main, out point);
            AddScores(point, (int)(SpeedBonusCooldown * 10));

            resourceCooldownBonusDown = Mathf.Clamp(resourceCooldownBonusDown - ResourceBonusCooldown * 0.5F, 0F, ResourceBonusCooldown);
            workerCooldownBonusDown = Mathf.Clamp(workerCooldownBonusDown - WorkerBonusCooldown * 0.5F, 0F, WorkerBonusCooldown);
            workCooldownBonusDown = Mathf.Clamp(workCooldownBonusDown - WorkBonusCooldown * 0.5F, 0F, WorkBonusCooldown);
		}
	}

	public void TryActiveWorkerBonus() {
		if (workerCooldownBonusDown >= WorkerBonusCooldown && !WorkerBonusActive) {
			workerDurationBonusDown = WorkerBonusDuration;
			workerCooldownBonusDown = 0f;
			WorkerBonusActive = true;

            GameGUIManager.I.WorkerBar.StopAnimation();

            GameResourceManager.I.AdditionalCharacters++;
            GameManager.I.CampObject.UpdateWorkerIcons();

            OnBonusCameActive.Invoke("Worker");
            AudioWrapper.I.PlaySfx("bonus_used");
            workerBonusLastAvailable = false;

            Vector3 point = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(workerBonusObject.GetComponent<RectTransform>(),
                                                                    workerBonusObject.GetComponent<RectTransform>().position,
                                                                    Camera.main, out point);
            AddScores(point, (int)(WorkerBonusCooldown * 10));

            resourceCooldownBonusDown = Mathf.Clamp(resourceCooldownBonusDown - ResourceBonusCooldown * 0.5F, 0F, ResourceBonusCooldown);
            speedCooldownBonusDown = Mathf.Clamp(speedCooldownBonusDown - SpeedBonusCooldown * 0.5F, 0F, SpeedBonusCooldown);
            workCooldownBonusDown = Mathf.Clamp(workCooldownBonusDown - WorkBonusCooldown * 0.5F, 0F, WorkBonusCooldown);
		}
	}

	public void TryActiveWorkBonus() {
		if (workCooldownBonusDown >= WorkBonusCooldown && !WorkBonusActive) {
			workDurationBonusDown = WorkBonusDuration;
			workCooldownBonusDown = 0f;
			WorkBonusActive = true;

            GameGUIManager.I.WorkBar.StopAnimation();

            OnBonusCameActive.Invoke("Work");
            AudioWrapper.I.PlaySfx("bonus_used");
            workBonusLastAvailable = false;

            Vector3 point = Vector3.zero;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(workBonusObject.GetComponent<RectTransform>(),
                                                                    workBonusObject.GetComponent<RectTransform>().position,
                                                                    Camera.main, out point);
            AddScores(point, (int)(WorkBonusCooldown * 10));

            resourceCooldownBonusDown = Mathf.Clamp(resourceCooldownBonusDown - ResourceBonusCooldown * 0.5F, 0F, ResourceBonusCooldown);
            speedCooldownBonusDown = Mathf.Clamp(speedCooldownBonusDown - SpeedBonusCooldown * 0.5F, 0F, SpeedBonusCooldown);
            workerCooldownBonusDown = Mathf.Clamp(workerCooldownBonusDown - WorkerBonusCooldown * 0.5F, 0F, WorkerBonusCooldown);
		}
	}

	private IEnumerator BonusTimer() {
        while (!GameIsEnd) {

			if (ResourceBonusNeed && !ResourceBonusActive) {
				resourceCooldownBonusDown = Mathf.Clamp(resourceCooldownBonusDown + Time.deltaTime, 0F, ResourceBonusCooldown);
                GameGUIManager.I.UpdateResourceBar(resourceCooldownBonusDown, ResourceBonusCooldown, true);

                if (resourceCooldownBonusDown >= ResourceBonusCooldown) {
                    if (!resourceBonusLastAvailable) {
                        GameGUIManager.I.ResourceBar.Ping();
                        AudioWrapper.I.PlaySfx("bonus_ready");
                    }

                    resourceBonusLastAvailable = true;
                    OnBonusCameAvailable.Invoke("Resource");
                } else resourceBonusLastAvailable = false;
			}
			if (SpeedBonusNeed && !SpeedBonusActive) {
				speedCooldownBonusDown = Mathf.Clamp(speedCooldownBonusDown + Time.deltaTime, 0F, SpeedBonusCooldown);
                GameGUIManager.I.UpdateSpeedBar(speedCooldownBonusDown, SpeedBonusCooldown, true);

                if (speedCooldownBonusDown >= SpeedBonusCooldown) {
                    if (!speedBonusLastAvailable) {
                        GameGUIManager.I.SpeedBar.Ping();
                        AudioWrapper.I.PlaySfx("bonus_ready");
                    }

                    speedBonusLastAvailable = true;
                    OnBonusCameAvailable.Invoke("Speed");
                } else speedBonusLastAvailable = false;
			}
			if (WorkerBonusNeed && !WorkerBonusActive) {
				workerCooldownBonusDown = Mathf.Clamp(workerCooldownBonusDown + Time.deltaTime, 0F, WorkerBonusCooldown);
                GameGUIManager.I.UpdateWorkerBar(workerCooldownBonusDown, WorkerBonusCooldown, true);

                if (workerCooldownBonusDown >= WorkerBonusCooldown) {
                    if (!workerBonusLastAvailable) {
                        GameGUIManager.I.WorkerBar.Ping();
                        AudioWrapper.I.PlaySfx("bonus_ready");
                    }

                    workerBonusLastAvailable = true;
                    OnBonusCameAvailable.Invoke("Worker");
                } else workerBonusLastAvailable = false;
			}
			if (WorkBonusNeed && !WorkBonusActive) {
				workCooldownBonusDown = Mathf.Clamp(workCooldownBonusDown + Time.deltaTime, 0F, WorkBonusCooldown);
                GameGUIManager.I.UpdateWorkBar(workCooldownBonusDown, WorkBonusCooldown, true);

                if (workCooldownBonusDown >= WorkBonusCooldown) {
                    if (!workBonusLastAvailable) {
                        GameGUIManager.I.WorkBar.Ping();
                        AudioWrapper.I.PlaySfx("bonus_ready");
                    }

                    workBonusLastAvailable = true;
                    OnBonusCameAvailable.Invoke("Work");
                } else workBonusLastAvailable = false;
			}

			if (ResourceBonusActive) {
				resourceDurationBonusDown = Mathf.Clamp(resourceDurationBonusDown - Time.deltaTime, 0F, ResourceBonusDuration);
                GameGUIManager.I.UpdateResourceBar(resourceDurationBonusDown, ResourceBonusDuration, false);

				if (resourceDurationBonusDown <= 0F) {
					ResourceBonusActive = false;
				}
			}
			if (SpeedBonusActive) {
				speedDurationBonusDown = Mathf.Clamp(speedDurationBonusDown - Time.deltaTime, 0F, SpeedBonusDuration);
                GameGUIManager.I.UpdateSpeedBar(speedDurationBonusDown, SpeedBonusDuration, false);

				if (speedDurationBonusDown <= 0F) {
					SpeedBonusActive = false;
				}
			}
			if (WorkerBonusActive) {
				workerDurationBonusDown = Mathf.Clamp(workerDurationBonusDown - Time.deltaTime, 0F, WorkerBonusDuration);
                GameGUIManager.I.UpdateWorkerBar(workerDurationBonusDown, WorkerBonusDuration, false);

				if (workerDurationBonusDown <= 0F) {
                    GameResourceManager.I.AdditionalCharacters--;
                    GameManager.I.CampObject.UpdateWorkerIcons();
					WorkerBonusActive = false;
				}
			}
			if (WorkBonusActive) {
				workDurationBonusDown = Mathf.Clamp(workDurationBonusDown - Time.deltaTime, 0F, WorkBonusDuration);
                GameGUIManager.I.UpdateWorkBar(workDurationBonusDown, WorkBonusDuration, false);

				if (workDurationBonusDown <= 0F) {
					WorkBonusActive = false;
				}
			}

			yield return null;
		}
	}

	private IEnumerator TimeGhanger() {
        while (!GameIsEnd) {
            if (UserData.I.GameMode == 0 || UserData.I.GameMode == 1) {
                time += Time.deltaTime;
                OnTimeChanged.Invoke(time);
            }
            OnTimeChanged.Invoke(time);
			yield return null;
		}
	}

	private IEnumerator DelaySpawnJason(LayerObject obj) {
		while (!inited) {
			yield return null;
		}
		CharacterHomeObject home = Instantiate(ResourceManager.I.GetObjectPrefab("characterHomeObject")).GetComponent<CharacterHomeObject>();
        foreach (string specialPoint in obj.SpecialPointsID) {
            if(GetPointNode(specialPoint) != null) {
                home.Node = GetPointNode(specialPoint);
                break;
            }
        }
        home.transform.parent = ParentObjects.transform;
		home.transform.position = home.Node.Position;
        CampObject.SpawnJason(home);
	}

	private IEnumerator DelaySpawnMedea(LayerObject obj) {
		while (!inited) {
			yield return null;
		}
		CharacterHomeObject home = Instantiate(ResourceManager.I.GetObjectPrefab("characterHomeObject")).GetComponent<CharacterHomeObject>();
        foreach (string specialPoint in obj.SpecialPointsID) {
            if (GetPointNode(specialPoint) != null) {
                home.Node = GetPointNode(specialPoint);
                break;
            }
        }
        home.transform.parent = ParentObjects.transform;
		home.transform.position = home.Node.Position;
        CampObject.SpawnMedea(home);
	}

	private IEnumerator DelaySpawnWorker(LayerObject obj) {
		while (!inited) {
			yield return null;
		}
		CharacterHomeObject home = Instantiate(ResourceManager.I.GetObjectPrefab("characterHomeObject")).GetComponent<CharacterHomeObject>();
		home.Node = GetPointNode(obj.SpecialPointsID[0]);
        home.transform.parent = ParentObjects.transform;
		home.transform.position = home.Node.Position;
        CampObject.SpawnWorker(home);
	}
}
