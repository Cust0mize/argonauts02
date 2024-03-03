using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CampObject : BuildObject, IOverseer {
    [SerializeField]
    Vector3 startPositionHeads;
	[SerializeField]
	float xOffsetHeads;
	[SerializeField]
	private int countCharacters = 1;

    private GameObject workerHeadContainer;
    [SerializeField]private List<GameObject> workerHeads = new List<GameObject>();

	private int maxCharacters = 0;

    public int MaxCharacters {
        get {
            return countCharacters + GameResourceManager.I.UnblockedCharacters + GameResourceManager.I.AdditionalCharacters;
        }
    }

	public int CountCharacters {
		get {
            return countCharacters - BusyCharacters + GameResourceManager.I.UnblockedCharacters + GameResourceManager.I.AdditionalCharacters;
		}
	}

    private int BusyCharacters {
        get {
            int busyCharacters = 0;
            foreach (BaseCharacter ch in GameResourceManager.I.Characters) {
                if (ch.Stage != BaseCharacter.Stages.FreeReady && ch.Stage != BaseCharacter.Stages.Free && !(ch.Stage == BaseCharacter.Stages.Returning && ch.Backpack.Count.Equals(0))) busyCharacters++;
            }
            return busyCharacters;
        }
    }

	[SerializeField]
	GameObject workerPrefab;
    [SerializeField]
    GameObject fakeWorkerPrefab;
	[SerializeField]
	GameObject jasonPrefab;
	[SerializeField]
	GameObject medeaPrefab;

	[SerializeField]
	GameObject workerHeadPrefab;
    [SerializeField]
    Sprite workerHeadAvailableSprite;
    [SerializeField]
    Sprite workerHeadUnavailableSprite;

    [SerializeField] private GameObject[] happyPoints;
    private List<GameObject> usedHappyPoints = new List<GameObject>();

    public override event Action<int> OnSuccessUpgrade = delegate { };

    protected override void Awake() {
		base.Awake();

		GameManager.I.OnObjectDestroyedEvent += OnObjectDestroyedEvent;

		if (GameResourceManager.I.Jason != null)
			GameResourceManager.I.Jason.OnStageChandeg += OnStageJasonChange;
		if (GameResourceManager.I.Medea != null)
			GameResourceManager.I.Medea.OnStageChandeg += OnStageMedeaChange;

		foreach (BaseCharacter bc in GameResourceManager.I.Characters) {
			bc.OnStageChandeg += OnStageChange;
		}
	}

    public GameObject GetHappyPoint() {
        if (usedHappyPoints.Count - 1 >= happyPoints.Length) {
            usedHappyPoints.Clear();
        }

        usedHappyPoints.Add(happyPoints[usedHappyPoints.Count]);
        return happyPoints[usedHappyPoints.Count - 1];
    }

    public void UpdateWorkerIcons() {
        if(workerHeadContainer==null) {
            workerHeadContainer = new GameObject("WorkerHeadContainer");
            workerHeadContainer.transform.SetParent(GameManager.I.Canvas2D.transform);
            RectTransform rt = workerHeadContainer.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(1f, 50f);
            HorizontalLayoutGroup hlg = workerHeadContainer.AddComponent<HorizontalLayoutGroup>();
            hlg.childAlignment = TextAnchor.MiddleCenter;
            hlg.childControlWidth = false;
            hlg.childControlHeight = false;
            hlg.padding.left = -12;
            hlg.spacing = -30;
        }

        workerHeadContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(MaxCharacters * 34 + 6, 50f);
        workerHeadContainer.transform.position = transform.position + new Vector3(0f, startPositionHeads.y, 0f);

        foreach(Transform t in workerHeadContainer.transform) {
            t.gameObject.SetActive(false);
        }

        GameObject wh = null;
        for (int i = 0; i < MaxCharacters; i++) {
            if (i + 1 > workerHeads.Count) {
                wh = Instantiate(workerHeadPrefab, workerHeadContainer.transform);
                wh.GetComponent<Image>().sprite = i < CountCharacters ? workerHeadAvailableSprite : workerHeadUnavailableSprite;
                workerHeads.Add(wh);
            } else {
                wh = workerHeads[i];
                wh.gameObject.SetActive(true);
                wh.transform.position += Vector3.right * i * xOffsetHeads;
                wh.GetComponent<Image>().sprite = i < CountCharacters ? workerHeadAvailableSprite : workerHeadUnavailableSprite;
            }
        }
    }

    public override void Start() {
		base.Start();

		StartCoroutine(DelayStart());
	}

	IEnumerator DelayStart() {
		while (!GameManager.I.GameInited)
			yield return null;
		yield return new WaitForSeconds(0.3F);
        OnObjectDestroyedEvent(null);
	}

    private void OnObjectDestroyedEvent(Node obj) {
        base.OnObjectDestroyed(obj);

        List<BaseCharacter> unlockedCharacters = new List<BaseCharacter>();

        if (GameResourceManager.Inited && OverseerController.Inited) {
            foreach (BaseCharacter worker in GameResourceManager.I.BlockedCharacters) {
                if (OverseerController.I.PathExist(worker.LastPassedNode, worker.Home.Node)) {
                    List<Node> path = OverseerController.I.GetPathForCharacter(worker.LastPassedNode, worker.Home.Node);
                    worker.MoveToHome(worker.Home, path);
                    unlockedCharacters.Add(worker);
                }
            }

            foreach (BaseCharacter unlockedCharacter in unlockedCharacters) {
                GameResourceManager.I.BlockedCharacters.Remove(unlockedCharacter);
            }
        }
    }

	private void OnArrivalToHomeBlockedWorker(bool success) {
		GameResourceManager.I.UnblockedCharacters++;
        GameManager.I.CampObject.UpdateWorkerIcons();
	}

    public IEnumerator GetFreeCharacters(int countWorkers, Node target, List<Resource> needResources = null, bool jaison = false, bool medea = false) {
        List<BaseCharacter> characters = new List<BaseCharacter>();

        if (needResources != null) {
            if (!jaison && needResources.Exists(x => x.Type == Resource.Types.Jaison)) {
                if (GameResourceManager.I.Jason != null && GameResourceManager.I.Jason.Stage == BaseCharacter.Stages.Free || GameResourceManager.I.Jason.Stage == BaseCharacter.Stages.FreeReady || GameResourceManager.I.Jason.Stage == BaseCharacter.Stages.Ready || (GameResourceManager.I.Jason.Stage == BaseCharacter.Stages.Returning && GameResourceManager.I.Jason.Backpack.Count.Equals(0))) {
                    characters.Add(GameResourceManager.I.Jason);
                }
            }

            if (!medea && needResources.Exists(x => x.Type == Resource.Types.Medea)) {
                if (GameResourceManager.I.Medea != null && GameResourceManager.I.Medea.Stage == BaseCharacter.Stages.Free || GameResourceManager.I.Medea.Stage == BaseCharacter.Stages.FreeReady || GameResourceManager.I.Medea.Stage == BaseCharacter.Stages.Ready || (GameResourceManager.I.Medea.Stage == BaseCharacter.Stages.Returning && GameResourceManager.I.Medea.Backpack.Count.Equals(0))) {
                    characters.Add(GameResourceManager.I.Medea);
                }
            }
        }

        foreach (BaseCharacter w in GameResourceManager.I.Characters) {
            if (w == null || w.gameObject == null) continue;
            if (w.Stage == BaseCharacter.Stages.FreeReady || (w.Stage == BaseCharacter.Stages.Returning && w.Backpack.Count.Equals(0))) {
                if (characters.Count + 1 <= countWorkers)
                    characters.Add(w);
                else break;
            }
        }

        int remainedWorkersChars = countWorkers - characters.Count;

        for (int i = 0; i < remainedWorkersChars; i++) {
            if (GameManager.I.CampObject.CountCharacters < 1) break;

            GameObject worker = Instantiate(workerPrefab);
            worker.GetComponent<BaseCharacter>().OnStageChandeg += OnStageChange;
            worker.transform.position = WorkPoints[0].transform.position;
            worker.GetComponent<BaseCharacter>().Stage = BaseCharacter.Stages.Ready;
            worker.GetComponent<BaseCharacter>().Home = this;
            GameResourceManager.I.Characters.Add(worker.GetComponent<BaseCharacter>());
            GameManager.I.CampObject.UpdateWorkerIcons();

            characters.Add(worker.GetComponent<BaseCharacter>());
        }

        yield return characters;
    }

    protected override void OnUpgrade(int upgrade) {
        LevelTaskController.I.UpdateTask(string.Format("{0}{1:00}", KeyAction.Substring(0, KeyAction.Length - 2), upgrade - 1), 1);
        AwardHandler.I.UpdateAward("award_builder", 1);

        GameManager.I.AddScores(transform.position, upgrade * 50);

        OnSuccessUpgrade.Invoke(upgrade);
        UpdateUpgradeIconState();
    }

    public void ReleaseWorkers() {
        while ((MaxCharacters - GameResourceManager.I.Characters.Count) > 0) {
            GameObject worker = Instantiate(workerPrefab);
            worker.GetComponent<BaseCharacter>().OnStageChandeg += OnStageChange;
            worker.transform.position = WorkPoints[0].transform.position;
            worker.GetComponent<BaseCharacter>().Stage = BaseCharacter.Stages.Ready;
            worker.GetComponent<BaseCharacter>().Home = this;

            worker.GetComponent<BaseCharacter>().Release();

            GameResourceManager.I.Characters.Add(worker.GetComponent<BaseCharacter>());
            GameManager.I.CampObject.UpdateWorkerIcons();
        }
    }

	public void SpawnJason(BaseInObject jasonHome) {
		if (GameResourceManager.I.Jason == null) {
			GameResourceManager.I.Resources.Add(new Resource(Resource.Types.Jaison, 1));

			GameResourceManager.I.Jason = Instantiate(jasonPrefab).GetComponent<BaseCharacter>();
			GameResourceManager.I.Jason.OnStageChandeg += OnStageJasonChange;
			GameResourceManager.I.Jason.transform.position = jasonHome.transform.position;
			GameResourceManager.I.Jason.Stage = BaseCharacter.Stages.Ready;
			GameResourceManager.I.Jason.Home = jasonHome;
		}
	}

	public void SpawnMedea(BaseInObject medeaHome) {
		if (GameResourceManager.I.Medea == null) {
			GameResourceManager.I.Resources.Add(new Resource(Resource.Types.Medea, 1));

			GameResourceManager.I.Medea = Instantiate(medeaPrefab).GetComponent<BaseCharacter>();
			GameResourceManager.I.Medea.OnStageChandeg += OnStageMedeaChange;
			GameResourceManager.I.Medea.transform.position = medeaHome.transform.position;
			GameResourceManager.I.Medea.Stage = BaseCharacter.Stages.Ready;
			GameResourceManager.I.Medea.Home = medeaHome;
		}
	}

    public void SpawnMedea(Vector3 position) {
        if (GameResourceManager.I.Medea == null) {
            GameResourceManager.I.Resources.Add(new Resource(Resource.Types.Medea, 1));

            GameResourceManager.I.Medea = Instantiate(medeaPrefab).GetComponent<BaseCharacter>();
            GameResourceManager.I.Medea.OnStageChandeg += OnStageMedeaChange;
            GameResourceManager.I.Medea.transform.position = position;
            GameResourceManager.I.Medea.Stage = BaseCharacter.Stages.Ready;
        }
    }

	public void SpawnWorker(BaseInObject workerHome) {
		BaseCharacter worker = Instantiate(workerPrefab).GetComponent<BaseCharacter>();
		worker.OnStageChandeg += OnStageChange;
		worker.transform.position = workerHome.transform.position;
		worker.Stage = BaseCharacter.Stages.Ready;
		worker.Home = this;
        worker.LastPassedNode = workerHome.Node;

		GameResourceManager.I.BlockedCharacters.Add(worker);
	}

    public void SpawnFakeWorker(Vector3 position, Node node) {
        BaseCharacter worker = Instantiate(fakeWorkerPrefab).GetComponent<BaseCharacter>();
        worker.OnStageChandeg += OnStageChange;
        worker.transform.position = position;
        worker.Stage = BaseCharacter.Stages.Ready;
        worker.Home = this;
        worker.LastPassedNode = node;

        List<Node> path = OverseerController.I.GetPathForCharacter(worker.LastPassedNode, worker.Home.Node);
        worker.MoveToHome(GameManager.I.CampObject, path);
    }

	void OnStageChange(BaseCharacter worker) {
		switch (worker.Stage) {
			case BaseCharacter.Stages.Free:
                if (!worker.IsFake) {
                    GameResourceManager.I.Characters.Remove(worker);
                    DestroyImmediate(worker.gameObject);
                    GameManager.I.CampObject.UpdateWorkerIcons();
                } else {
                    DestroyImmediate(worker.gameObject);
                }
				break;
		}
	}

	void OnStageJasonChange(BaseCharacter jason) {
		switch (jason.Stage) {
			case BaseCharacter.Stages.Free:
				GameResourceManager.I.SetResources(Resource.Types.Jaison, 1);
				break;
			case BaseCharacter.Stages.FreeReady:
				GameResourceManager.I.SetResources(Resource.Types.Jaison, 1);
				break;
			case BaseCharacter.Stages.Ready:
				GameResourceManager.I.SetResources(Resource.Types.Jaison, 1);
				break;
			default:
				GameResourceManager.I.SetResources(Resource.Types.Jaison, 0);
				break;
		}
	}

	void OnStageMedeaChange(BaseCharacter medea) {
		switch (medea.Stage) {
			case BaseCharacter.Stages.Free:
				GameResourceManager.I.SetResources(Resource.Types.Medea, 1);
				break;
			case BaseCharacter.Stages.FreeReady:
				GameResourceManager.I.SetResources(Resource.Types.Medea, 1);
				break;
			case BaseCharacter.Stages.Ready:
				GameResourceManager.I.SetResources(Resource.Types.Medea, 1);
				break;
			default:
				GameResourceManager.I.SetResources(Resource.Types.Medea, 0);
				break;
		}
	}

    protected override void OnDestroy () {
        base.OnDestroy();

		Unsubscribe();
	}

	private void OnDisable() {
		Unsubscribe();
	}

	private void Unsubscribe() {
        if (workerHeadContainer != null) DestroyObject(workerHeadContainer);

		if (GameManager.Inited)
			GameManager.I.OnObjectDestroyedEvent -= OnObjectDestroyedEvent;

        if(GameResourceManager.Inited){
            foreach (BaseCharacter ch in GameResourceManager.I.Characters) {
                ch.OnStageChandeg -= OnStageChange;
            }
            if (GameResourceManager.I.Jason != null)
                GameResourceManager.I.Jason.OnStageChandeg -= OnStageJasonChange;
            if (GameResourceManager.I.Medea != null)
                GameResourceManager.I.Medea.OnStageChandeg -= OnStageMedeaChange;
        }
	}
}
