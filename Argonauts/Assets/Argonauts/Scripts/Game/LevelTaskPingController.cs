using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LevelTaskPingController : LocalSingletonBehaviour<LevelTaskPingController> {
    [SerializeField] private GameObject pinPrefab;
    [SerializeField] private float pingDuration;
    [SerializeField] private float pingInterval;
    [SerializeField] private float fadeInOutDuration;

    public bool PingEnable = false;

    private Dictionary<BaseInObject, GameObject> usedPins = new Dictionary<BaseInObject, GameObject>();
    private Dictionary<BuildObjectController, BuildObjectPin> buildObjectsUsedPins = new Dictionary<BuildObjectController, BuildObjectPin>();

    private readonly List<string> exceptionBuildTaskKeys = new List<string>(){
        "act_build_farm01",
        "act_build_farm02",
        "act_build_farm03",
        "act_build_sawmill01",
        "act_build_sawmill02",
        "act_build_sawmill03",
        "act_build_quarry01",
        "act_build_quarry02",
        "act_build_quarry03",
        "act_build_goldmine01",
        "act_build_goldmine02",
        "act_build_goldmine03",
        "act_build_tent01",
        "act_build_tent02",
        "act_build_portalbig01",
        "act_build_portalbig02"
    };

    public void DoInit (Level level) {
        if (level.Task1 != null) {
            InitPins(level.Task1.Key);
        }
        if (level.Task2 != null) {
            InitPins(level.Task2.Key);
        }
        if (level.Task3 != null) {
            InitPins(level.Task3.Key);
        }

        OverseerController.I.OnNotEnoughtResources += OnNotEnoughtResources;
    }

    private void OnNotEnoughtResources(List<Resource> resource, BaseInObject obj) {
        foreach (Resource r in resource) {
            if (!GameResourceManager.I.RequiredResourceExistMaxCharacters(r)) {
                if (r.Type == Resource.Types.Worker) {
                    GameManager.I.CampObject.PingNoResource();
                    break;
                }

                List<BaseInObject> objects = GameManager.I.GetObjectsWithResource(r.Type);

                foreach (BaseInObject o in objects) {
                    o.PingNoResource();
                }
                break;
            }
        }
    }

    private void InitPins (string taskKey) {
        if (string.IsNullOrEmpty(taskKey)) return;

        string actionKey = GetActionKey(taskKey);

        List<BaseInObject> objects = new List<BaseInObject>();

        switch (actionKey) {
            case "act_take_food": break;
            case "act_take_wood": break;
            case "act_take_stone": break;
            case "act_take_gold": break;
            default:
                string k = GetActionKey(taskKey);

                if (exceptionBuildTaskKeys.Contains(k)) {
                    int needUpgrade = int.Parse(k.Substring(k.Length - 1, 1));
                    if (k.Contains("tent")) needUpgrade++;
                    objects.AddRange(GameManager.I.GetObjectsOnStartWithActionKey(k.Remove(k.Length - 2, 2)).Where(x => (x as BuildObject).Upgrade < needUpgrade).ToList());
                } else {
                    objects.AddRange(GameManager.I.GetObjectsOnActionKey(k));
                }
                break;
        }

        foreach (BaseInObject obj in objects) {
            GameObject pin = Instantiate(pinPrefab, GameManager.I.Canvas2D.transform);
            pin.transform.position = obj.transform.position;
            pin.transform.position += (Vector3)obj.OffsetPanelsTop;
            pin.GetComponent<CanvasGroup>().alpha = 0f;

            if (obj is BuildObject) {
                int needUpgrade = 1;
                if (obj is ProductionBuildObject || obj is CampObject || obj is PortalBigObject) {
                    needUpgrade = int.Parse(taskKey.Substring(taskKey.Length - 1, 1));

                    if (obj is CampObject) needUpgrade++;
                }
                buildObjectsUsedPins.Add(obj.GetComponent<BuildObject>().BuildObjectController, new BuildObjectPin(pin, needUpgrade));
            } else {
                usedPins.Add(obj, pin);
            }
        }
    }

    private string GetActionKey (string taskKey) {
        string[] k = taskKey.Split('_');
        StringBuilder sb = new StringBuilder();
        sb.Append("act");
        for (int i = 1; i < k.Length; i++) {
            sb.Append("_");
            sb.Append(k[i]);
        }

        return sb.ToString();
    }

    public void StartPing () {
        StopAllCoroutines();
        StartCoroutine(PingWorker());
    }

    public void DisablePin(BaseInObject obj) {
        if (usedPins.ContainsKey(obj)) {
            Destroy(usedPins[obj]);
            usedPins.Remove(obj);
        }
    }

    public void DisablePin(BuildObjectController obj) {
        if (buildObjectsUsedPins.ContainsKey(obj)) {
            if (obj.BaseBuildObject.Upgrade + 1 >= buildObjectsUsedPins[obj].NeedUpgrade) {
                Destroy(buildObjectsUsedPins[obj].Pin);
                buildObjectsUsedPins.Remove(obj);
            }
        }
    }

    private IEnumerator PingWorker () {
        yield return PingIteration();

        while (true) {
            yield return new WaitForSeconds(pingInterval);

            foreach(BuildObjectController obj in buildObjectsUsedPins.Keys) {
                buildObjectsUsedPins[obj].Pin.transform.position = obj.BaseBuildObject.transform.position;
                buildObjectsUsedPins[obj].Pin.transform.position += (Vector3)obj.BaseBuildObject.OffsetPanelsTop;
            }

            yield return PingIteration();
        }
    }

    private IEnumerator PingIteration () {
        float fadePingDown = 0f;
        while (fadePingDown < fadeInOutDuration) {
            //simple objects
            foreach (BaseInObject obj in usedPins.Keys) {
                if (obj == null || obj.gameObject == null || obj.Node.IsFree) {
                    usedPins[obj].GetComponent<CanvasGroup>().alpha = 0f;
                    continue;
                }
                usedPins[obj].GetComponent<CanvasGroup>().alpha = fadePingDown / fadeInOutDuration;
            }
            //builds
            foreach (BuildObjectController obj in buildObjectsUsedPins.Keys) {
                if (obj == null || obj.gameObject == null || obj.BaseBuildObject == null || obj.BaseBuildObject.gameObject == null || obj.BaseBuildObject.Node.IsFree) {
                    buildObjectsUsedPins[obj].Pin.GetComponent<CanvasGroup>().alpha = 0f;
                    continue;
                }
                if (obj.BaseBuildObject is ProductionBuildObject || obj.BaseBuildObject is CampObject) {
                    if (obj.BaseBuildObject.Upgrade >= buildObjectsUsedPins[obj].NeedUpgrade) {
                        buildObjectsUsedPins[obj].Pin.GetComponent<CanvasGroup>().alpha = 0f;
                        continue;
                    }
                }

                buildObjectsUsedPins[obj].Pin.GetComponent<CanvasGroup>().alpha = fadePingDown / fadeInOutDuration;
            }
            fadePingDown += Time.deltaTime;
            yield return null;
        }

        //simple objects
        foreach (BaseInObject obj in usedPins.Keys) {
            if (obj == null || obj.gameObject == null || obj.Node.IsFree) {
                usedPins[obj].GetComponent<CanvasGroup>().alpha = 0f;
                continue;
            }
            usedPins[obj].GetComponent<CanvasGroup>().alpha = 1f;
        }
        //builds
        foreach (BuildObjectController obj in buildObjectsUsedPins.Keys) {
            if (obj == null || obj.gameObject == null || obj.BaseBuildObject == null || obj.BaseBuildObject.gameObject == null || obj.BaseBuildObject.Node.IsFree) {
                buildObjectsUsedPins[obj].Pin.GetComponent<CanvasGroup>().alpha = 0f;
                continue;
            }
            if (obj.BaseBuildObject is ProductionBuildObject || obj.BaseBuildObject is CampObject) {
                if (obj.BaseBuildObject.Upgrade >= buildObjectsUsedPins[obj].NeedUpgrade) {
                    buildObjectsUsedPins[obj].Pin.GetComponent<CanvasGroup>().alpha = 0f;
                    continue;
                }
            }
            buildObjectsUsedPins[obj].Pin.GetComponent<CanvasGroup>().alpha = 1f;
        }

        while (!PingEnable) yield return null;

        yield return new WaitForSeconds(pingDuration);

        while (fadePingDown > 0) {
            //simple objects
            foreach (BaseInObject obj in usedPins.Keys) {
                if (obj == null || obj.gameObject == null || obj.Node.IsFree) {
                    usedPins[obj].GetComponent<CanvasGroup>().alpha = 0f;
                    continue;
                }
                usedPins[obj].GetComponent<CanvasGroup>().alpha = fadePingDown / fadeInOutDuration;
            }
            //builds
            foreach (BuildObjectController obj in buildObjectsUsedPins.Keys) {
                if (obj == null || obj.gameObject == null || obj.BaseBuildObject == null || obj.BaseBuildObject.gameObject == null || obj.BaseBuildObject.Node.IsFree) {
                    buildObjectsUsedPins[obj].Pin.GetComponent<CanvasGroup>().alpha = 0f;
                    continue;
                }
                if (obj.BaseBuildObject is ProductionBuildObject || obj.BaseBuildObject is CampObject) {
                    if (obj.BaseBuildObject.Upgrade >= buildObjectsUsedPins[obj].NeedUpgrade) {
                        buildObjectsUsedPins[obj].Pin.GetComponent<CanvasGroup>().alpha = 0f;
                        continue;
                    }
                }
                buildObjectsUsedPins[obj].Pin.GetComponent<CanvasGroup>().alpha = fadePingDown / fadeInOutDuration;
            }
            fadePingDown -= Time.deltaTime;
            yield return null;
        }

        foreach (BaseInObject obj in usedPins.Keys) {
            usedPins[obj].GetComponent<CanvasGroup>().alpha = 0f;
        }
        foreach (BuildObjectController obj in buildObjectsUsedPins.Keys) {
            buildObjectsUsedPins[obj].Pin.GetComponent<CanvasGroup>().alpha = 0f;
        }
    }

    public class BuildObjectPin {
        public GameObject Pin;
        public int NeedUpgrade = 1;

        public BuildObjectPin(GameObject pin, int needUpgrade) {
            Pin = pin;
            NeedUpgrade = needUpgrade;
        }
    }
}
