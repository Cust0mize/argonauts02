using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OverseerController : LocalSingletonBehaviour<OverseerController> {
    private const int MAX_QUEUE = 4;

    [SerializeField] float offsetNopePaths;
    [SerializeField] GameObject nopePathPrefab;

    List<GameObject> nopePaths = new List<GameObject>();
    [SerializeField] HashSet<BaseInObject> orderObjects = new HashSet<BaseInObject>();
    HashSet<BaseInObject> handlingObjects = new HashSet<BaseInObject>();
    List<BaseInObject> flags = new List<BaseInObject>();

    public event Action<BaseInObject> OnPathBlockedEvent = delegate { };
    public event Action<List<Resource>, BaseInObject> OnNotEnoughtResources = delegate { };
    public event Action<BaseInObject> OnObjectAddedInOrder = delegate { };
    public event Action<BaseInObject> OnArriveHomeEvent = delegate { };

    private NoPathController noPathController;

    private IEnumerator Start() {
        noPathController = transform.GetChild(0).GetComponent<NoPathController>();

        StartCoroutine(WorkerQueue());
        yield return 0;
    }

    private IEnumerator WorkerQueue() {
        int countWorkers = 0;
        bool jaisonSended = false;
        bool medeaSended = false;

        CoroutineWithData cd;
        List<BaseCharacter> flagCharacter;
        List<Node> path;

        while (true) {
            yield return null;
#region Pre-jaison
            for (int f = 0; f < flags.Count; f++) {
                lock (flags[f]) {
                    if (flags[f].RequiredResources.Exists(x => x.Type == Resource.Types.Jaison) && !flags[f].RequiredResources.Exists(x => x.Type == Resource.Types.Medea) && !flags[f].RequiredResources.Exists(x => x.Type == Resource.Types.Worker)) {
                        handlingObjects.Add(flags[f]);

                        if (!flags[f].EnoughCharacters()) {
                            handlingObjects.Remove(flags[f]);
                            continue;
                        }

                        if ((flags[f].Node.IsFree && !(flags[f] is ResourcesDropPoint))) { //REMOVED QUEUE FLAG
                            handlingObjects.Remove(flags[f]);
                            flags.RemoveAt(f);
                            UpdateQueueFlags();
                            continue;
                        }

                        cd = new CoroutineWithData(this, GameManager.I.CampObject.GetFreeCharacters(0, flags[f].Node, flags[f].RequiredResources));
                        yield return cd.coroutine;
                        flagCharacter = cd.result as List<BaseCharacter>;

                        if (flagCharacter == null) {
                            handlingObjects.Remove(flags[f]);
                            continue;
                        }
                        if (flags.Count == 0) {
                            foreach (BaseCharacter b in flagCharacter) {
                                b.Stage = BaseCharacter.Stages.Free;
                            }
                            handlingObjects.Remove(flags[f]);
                            continue;
                        }

                        flagCharacter.RemoveAll(x => x == null || x.gameObject == null);
                        if (flagCharacter.Count.Equals(0)) {
                            handlingObjects.Remove(flags[f]);
                        }

                        List<BaseCharacter> sendedCharacters = new List<BaseCharacter>();

                        for (int i = 0; i < flagCharacter.Count; i++) {
                            path = null;

                            if (flagCharacter[i].Stage == BaseCharacter.Stages.Ready) {
                                path = GetPathForCharacter(flagCharacter[i].Home.Node, flags[f].Node);
                                flagCharacter[i].MoveToObject(flags[f], path);

                                sendedCharacters.Add(flagCharacter[i]);
                            } else if (flagCharacter[i].Stage == BaseCharacter.Stages.Free) {
                                path = GetPathForCharacter(flagCharacter[i].Home.Node, flags[f].Node);
                                flagCharacter[i].MoveToObject(flags[f], path);

                                sendedCharacters.Add(flagCharacter[i]);
                            } else if (flagCharacter[i].Stage == BaseCharacter.Stages.FreeReady) {
                                Node n = flagCharacter[i].CachedObject.Node;
                                if (!(flagCharacter[i].CachedObject is CampObject) && flagCharacter[i].LastPassedNode != null) n = flagCharacter[i].LastPassedNode;
                                path = GetPathForCharacter(n, flags[f].Node);

                                if (path == null) {
                                    path = GetPathForCharacter(flagCharacter[i].CachedObject.Node, flags[0].Node);
                                }

                                flagCharacter[i].MoveToObject(flags[f], path);

                                sendedCharacters.Add(flagCharacter[i]);
                            } else if (flagCharacter[i].Stage == BaseCharacter.Stages.Returning && flagCharacter[i].Backpack.Count.Equals(0)) {
                                Node n = flagCharacter[i].CachedObject.Node;
                                if (!(flagCharacter[i].CachedObject is CampObject) && flagCharacter[i].LastPassedNode != null) n = flagCharacter[i].LastPassedNode;
                                path = GetPathForCharacter(n, flags[f].Node);

                                if (path == null) {
                                    path = GetPathForCharacter(flagCharacter[i].CachedObject.Node, flags[0].Node);
                                }

                                flagCharacter[i].MoveToObject(flags[f], path);

                                sendedCharacters.Add(flagCharacter[i]);
                            }

                            if (flagCharacter.Count > 1) {
                                yield return new WaitForSeconds(0.2F);
                            }
                        }

                        yield return new WaitForSeconds(0.1F);

                        if (flags.Count > 0) {
                            handlingObjects.Remove(flags[f]);
                            flags[f].SendedCharacters.AddRange(sendedCharacters);

                            if (flags[f].SendedCharacters.Count >= flags[f].CountStages) { //REMOVED QUEUE FLAG
                                AddMeInOrder(flags[f]);
                                flags.RemoveAt(f);
                                UpdateQueueFlags();
                            }
                        }
                    }
                }
            }
#endregion

#region Pre-medea queue 
            for (int f = 0; f < flags.Count; f++) {
                lock (flags[f]) {
                    if (flags[f].RequiredResources.Exists(x => x.Type == Resource.Types.Medea) && !flags[f].RequiredResources.Exists(x => x.Type == Resource.Types.Jaison) && !flags[f].RequiredResources.Exists(x => x.Type == Resource.Types.Worker)) {
                        handlingObjects.Add(flags[f]);

                        if (!flags[f].EnoughCharacters()) {
                            handlingObjects.Remove(flags[f]);
                            continue;
                        }

                        if ((flags[f].Node.IsFree && !(flags[f] is ResourcesDropPoint))) { //REMOVED QUEUE FLAG
                            handlingObjects.Remove(flags[f]);
                            flags.RemoveAt(f);
                            UpdateQueueFlags();
                            continue;
                        }

                        cd = new CoroutineWithData(this, GameManager.I.CampObject.GetFreeCharacters(0, flags[f].Node, flags[f].RequiredResources));
                        yield return cd.coroutine;
                        flagCharacter = cd.result as List<BaseCharacter>;

                        if (flagCharacter == null) {
                            handlingObjects.Remove(flags[f]);
                            continue;
                        }
                        if (flags.Count == 0) {
                            foreach (BaseCharacter b in flagCharacter) {
                                b.Stage = BaseCharacter.Stages.Free;
                            }
                            handlingObjects.Remove(flags[f]);
                            continue;
                        }

                        flagCharacter.RemoveAll(x => x == null || x.gameObject == null);
                        if (flagCharacter.Count.Equals(0)) {
                            handlingObjects.Remove(flags[f]);
                        }

                        List<BaseCharacter> sendedCharacters = new List<BaseCharacter>();

                        for (int i = 0; i < flagCharacter.Count; i++) {
                            path = null;

                            if (flagCharacter[i].Stage == BaseCharacter.Stages.Ready) {
                                path = GetPathForCharacter(flagCharacter[i].Home.Node, flags[f].Node);
                                flagCharacter[i].MoveToObject(flags[f], path);

                                sendedCharacters.Add(flagCharacter[i]);
                            } else if (flagCharacter[i].Stage == BaseCharacter.Stages.Free) {
                                path = GetPathForCharacter(flagCharacter[i].Home.Node, flags[f].Node);
                                flagCharacter[i].MoveToObject(flags[f], path);

                                sendedCharacters.Add(flagCharacter[i]);
                            } else if (flagCharacter[i].Stage == BaseCharacter.Stages.FreeReady) {
                                Node n = flagCharacter[i].CachedObject.Node;
                                if (!(flagCharacter[i].CachedObject is CampObject) && flagCharacter[i].LastPassedNode != null) n = flagCharacter[i].LastPassedNode;
                                path = GetPathForCharacter(n, flags[f].Node);

                                if (path == null) {
                                    path = GetPathForCharacter(flagCharacter[i].CachedObject.Node, flags[0].Node);
                                }

                                flagCharacter[i].MoveToObject(flags[f], path);

                                sendedCharacters.Add(flagCharacter[i]);
                            } else if (flagCharacter[i].Stage == BaseCharacter.Stages.Returning && flagCharacter[i].Backpack.Count.Equals(0)) {
                                Node n = flagCharacter[i].CachedObject.Node;
                                if (!(flagCharacter[i].CachedObject is CampObject) && flagCharacter[i].LastPassedNode != null) n = flagCharacter[i].LastPassedNode;
                                path = GetPathForCharacter(n, flags[f].Node);

                                if (path == null) {
                                    path = GetPathForCharacter(flagCharacter[i].CachedObject.Node, flags[0].Node);
                                }

                                flagCharacter[i].MoveToObject(flags[f], path);

                                sendedCharacters.Add(flagCharacter[i]);
                            }

                            if (flagCharacter.Count > 1) {
                                yield return new WaitForSeconds(0.2F);
                            }
                        }

                        yield return new WaitForSeconds(0.1F);

                        if (flags.Count > 0) {
                            handlingObjects.Remove(flags[f]);

                            flags[f].SendedCharacters.AddRange(sendedCharacters);

                            if (flags[f].SendedCharacters.Count >= flags[f].CountStages) { //REMOVED QUEUE FLAG
                                AddMeInOrder(flags[f]);
                                flags.RemoveAt(f);
                                UpdateQueueFlags();
                            }
                        }
                    }
                }
            }
            #endregion

            #region Main queue
            if (flags.Count > 0) {
                lock (flags[0]) {
                    handlingObjects.Add(flags[0]);

                    if (!flags[0].EnoughCharacters()) {
                        handlingObjects.Remove(flags[0]);
                        continue;
                    }

                    if ((flags[0].Node.IsFree && !(flags[0] is ResourcesDropPoint))) { //REMOVED QUEUE FLAG
                        handlingObjects.Remove(flags[0]);
                        flags.RemoveAt(0);
                        UpdateQueueFlags();
                        continue;
                    }

                    countWorkers = 0;
                    jaisonSended = false;
                    medeaSended = false;

                    if (flags[0].RequiredResources.Exists(x => x.Type == Resource.Types.Worker))
                        countWorkers = GameResourceManager.I.GetCountResource(Resource.Types.Worker, flags[0].RequiredResources) - flags[0].SendedCharacters.Where(x => x.Type == BaseCharacter.Types.Worker).Count();
                    if (flags[0].RequiredResources.Exists(x => x.Type == Resource.Types.Jaison))
                        jaisonSended = flags[0].SendedCharacters.Exists(x => x.Type == BaseCharacter.Types.Jaison);
                    if (flags[0].RequiredResources.Exists(x => x.Type == Resource.Types.Medea))
                        medeaSended = flags[0].SendedCharacters.Exists(x => x.Type == BaseCharacter.Types.Medea);

                    cd = new CoroutineWithData(this, GameManager.I.CampObject.GetFreeCharacters(countWorkers, flags[0].Node, flags[0].RequiredResources, jaisonSended, medeaSended));
                    yield return cd.coroutine;
                    flagCharacter = cd.result as List<BaseCharacter>;

                    if (flagCharacter == null) {
                        handlingObjects.Remove(flags[0]);
                        continue;
                    }
                    if (flags.Count == 0) {
                        foreach (BaseCharacter b in flagCharacter) {
                            b.Stage = BaseCharacter.Stages.Free;
                        }
                        handlingObjects.Remove(flags[0]);
                        continue;
                    }

                    flagCharacter.RemoveAll(x => x == null || x.gameObject == null);
                    if (flagCharacter.Count.Equals(0)) {
                        handlingObjects.Remove(flags[0]);
                    }

                    List<BaseCharacter> sendedCharacters = new List<BaseCharacter>();

                    for (int i = 0; i < flagCharacter.Count; i++) {
                        path = null;

                        if (flagCharacter[i].Stage == BaseCharacter.Stages.Ready) {
                            path = GetPathForCharacter(flagCharacter[i].Home.Node, flags[0].Node);
                            flagCharacter[i].MoveToObject(flags[0], path);

                            sendedCharacters.Add(flagCharacter[i]);
                        } else if (flagCharacter[i].Stage == BaseCharacter.Stages.Free) {
                            path = GetPathForCharacter(flagCharacter[i].Home.Node, flags[0].Node);
                            flagCharacter[i].MoveToObject(flags[0], path);

                            sendedCharacters.Add(flagCharacter[i]);
                        } else if (flagCharacter[i].Stage == BaseCharacter.Stages.FreeReady) {
                            Node n = flagCharacter[i].CachedObject.Node;
                            if (!(flagCharacter[i].CachedObject is CampObject) && flagCharacter[i].LastPassedNode != null) n = flagCharacter[i].LastPassedNode;
                            path = GetPathForCharacter(n, flags[0].Node);

                            if (path == null) {
                                path = GetPathForCharacter(flagCharacter[i].CachedObject.Node, flags[0].Node);
                            }

                            flagCharacter[i].MoveToObject(flags[0], path);

                            sendedCharacters.Add(flagCharacter[i]);
                        } else if (flagCharacter[i].Stage == BaseCharacter.Stages.Returning && flagCharacter[i].Backpack.Count.Equals(0)) {
                            Node n = flagCharacter[i].CachedObject.Node;
                            if (!(flagCharacter[i].CachedObject is CampObject) && flagCharacter[i].LastPassedNode != null) n = flagCharacter[i].LastPassedNode;
                            path = GetPathForCharacter(n, flags[0].Node);

                            if (path == null) {
                                path = GetPathForCharacter(flagCharacter[i].CachedObject.Node, flags[0].Node);
                            }

                            flagCharacter[i].MoveToObject(flags[0], path);

                            sendedCharacters.Add(flagCharacter[i]);
                        }

                        if (flagCharacter.Count > 1) {
                            yield return new WaitForSeconds(0.2F);
                        }
                    }

                    yield return new WaitForSeconds(0.1F);

                    if (flags.Count > 0) {
                        handlingObjects.Remove(flags[0]);

                        if (flags[0].MultiplyWorkers) {
                            if (flagCharacter.Count > 0) {
                                AddMeInOrder(flags[0]);
                                flags.RemoveAt(0);
                                UpdateQueueFlags();
                            }
                        } else {
                            flags[0].SendedCharacters.AddRange(sendedCharacters);

                            if (flags[0].SendedCharacters.Count >= flags[0].CountStages) { //REMOVED QUEUE FLAG
                                AddMeInOrder(flags[0]);
                                flags.RemoveAt(0);
                                UpdateQueueFlags();
                            }
                        }
                    }
                }
            }

#endregion
        }
    }

    private void UpdateQueueFlags() {
        for (int i = 0; i < flags.Count; i++) {
            flags[i].UpdateFlag(i + 1);
        }
    }

    public void OnArriveHome(BaseInObject obj) {
        OnArriveHomeEvent.Invoke(obj);
    }

    public void AddMeInOrder(BaseInObject obj) {
        orderObjects.Add(obj);
        if (obj is BaseResourceObject) (obj as BaseResourceObject).StopPing();
        if (obj is BuildObject) (obj as BuildObject).StopPing();
        if (obj is EnemyObject) (obj as EnemyObject).StopPing();
        OnObjectAddedInOrder.Invoke(obj);
    }

    public void RemoveMeFromOrder(BaseInObject obj) {
        orderObjects.Remove(obj);
    }

    public bool ObjectInOrder(BaseInObject obj) {
        return flags.Contains(obj) || orderObjects.Contains(obj);
    }

    #region Blocked path logic 

    public void OnPathBlocked(Node a, Node b, BaseInObject obj) {
        AudioWrapper.I.PlaySfx("no_enough_resources");

        List<Node> path = AstarAlgorithm.CalculatePathWithoutBlock(GameManager.I.Graph, a, b);

        GameManager.I.StopNoPathPulsationAll();

        foreach (GameObject p in nopePaths) {
            Destroy(p);
        }
        nopePaths.Clear();

        if (path != null) {
            BuildBlockedPath(path);
        }

        OnPathBlockedEvent.Invoke(obj);
    }

    public void BuildBlockedPath(List<Node> path) {
        int step = 0;
        Vector3 lastPos = path[step].Position;
        Vector3 pos = path[step].Position;

        if (path.Count > 0 && path[0] == GameManager.I.CampObject.Node) {
            pos = Vector3.MoveTowards(pos, path[step + 1].Position, 50F);
            lastPos = pos;
        }

        bool nowPathFree = true;

        PortalObject teleportedTo = null;

        while (step + 1 < path.Count - 1) {
            pos = Vector3.MoveTowards(pos, path[step + 1].Position, 1);

            if (Vector3.Distance(pos, lastPos) >= offsetNopePaths) {
                GameObject n = Instantiate(nopePathPrefab, pos, Quaternion.identity);
                n.transform.localScale = Vector3.one * 0.8F;
                n.transform.SetParent(GameManager.I.Canvas2D.transform, true);
                n.transform.position = pos;

                n.GetComponent<Image>().color = nowPathFree ? Color.white : new Color(1.0f, 0.15f, 0.15f);

                lastPos = pos;
                nopePaths.Add(n);
            }

            if (Vector2.Distance(pos, path[step + 1].Position) < 1) {
                step++;

                if (GameManager.I.Portals.Exists(x => x != null && x.Node == path[step] && x != teleportedTo)) {
                    teleportedTo = GameManager.I.Portals.Where(x => x != null && x.Node == path[step] && x != teleportedTo).First().MirrorPortalController.BaseBuildObject.GetComponent<PortalObject>();
                    pos = path[step + 1].Position;
                }

                if (GameManager.I.IsObject(path[step]) && !path[step].IsFree && nowPathFree) {
                    nowPathFree = false;
                }
            }
        }

        List<BaseInObject> objs = GameManager.I.GetObjects(path);

        foreach (BaseInObject obj in objs) {
            if (obj != null) {
                if (obj.Node != path[path.Count - 1] && obj != GameManager.I.CampObject && obj.gameObject.activeSelf && !obj.Node.IsFree)
                    obj.StartNoPathPulsate(2, 3.5f);
            }
        }

        noPathController.StopAllCoroutines();
        noPathController.StartCoroutine(noPathController.NoPathWorker(nopePaths, 1f, 0.7f));
        noPathController.StartCoroutine(noPathController.DestroyPath(nopePaths, 3f));
    }

#endregion

    public void OnClickObject(BaseInObject obj) {
        lock (obj) {
            if (flags.Contains(obj))
                Debug.Log(handlingObjects.Contains(obj));
            if (flags.Contains(obj) && !orderObjects.Contains(obj) && !handlingObjects.Contains(obj)) {
                GameManager.I.AddResources(obj.RequiredResources);
                if (obj is BaseResourceObject) (obj as BaseResourceObject).CheckShouldPing();
                obj.OrderCounts--;

                flags.Remove(obj);
                UpdateQueueFlags();

                obj.SetFlagActivity(false, 0);
                obj.SetCheckmarkActivity(false);
                return;
            }
            if (flags.Count >= MAX_QUEUE)
                return;

            if ((!obj.MultiplyWorkers && (orderObjects.Contains(obj)) || handlingObjects.Contains(obj)) || (obj.Node.IsFree && !(obj is ResourcesDropPoint)))
                return;
            if ((obj.MultiplyWorkers && obj.OrderCounts >= obj.MaxMultiplyWorkers))
                return;
            if (!obj.Available)
                return;
            if (!obj.Clickable)
                return;
            if (!obj.CanBeUsed())
                return;
            if (!obj.FreePointExist())
                return;

            if (obj.RequiredResources.Exists(x => x.Type == Resource.Types.Jaison)) {
                if (!PathExist(GameResourceManager.I.Jason.Home.Node, obj.Node)) {
                    OnPathBlocked(GameResourceManager.I.Jason.Home.Node, obj.Node, obj);
                    return;
                }
            }
            if (obj.RequiredResources.Exists(x => x.Type == Resource.Types.Medea)) {
                if (!PathExist(GameResourceManager.I.Medea.Home.Node, obj.Node)) {
                    OnPathBlocked(GameResourceManager.I.Medea.Home.Node, obj.Node, obj);
                    return;
                }
            }
            if (obj.RequiredResources.Exists(x => x.Type == Resource.Types.Worker)) {
                if (!PathExist(GameManager.I.CampObject.Node, obj.Node)) {
                    OnPathBlocked(GameManager.I.CampObject.Node, obj.Node, obj);
                    return;
                }
            }

            if (!obj.EnoughResourcesWithCharacters()) {
                AudioWrapper.I.PlaySfx("no_enough_resources");

                OnNotEnoughtResources.Invoke(obj.RequiredResources, obj);
            } else {
                GameManager.I.TakeResources(obj.RequiredResources, obj);

                obj.OrderCounts++;

                flags.Add(obj);
                flags[flags.Count - 1].SetFlagActivity(true, flags.Count);

                GameGUIManager.I.HideActionInfo();

                if (obj is BaseResourceObject) (obj as BaseResourceObject).StopPing();
            }
        }
    }

    #region Pathfidning 

    public bool PathExist(Node a, Node b) {
        return AstarAlgorithm.CalculatePath(GameManager.I.Graph, a, b) != null;
    }

    public List<Node> GetPathForCharacter(Node start, Node goal) {
        List<Node> path = null;
        path = AstarAlgorithm.CalculatePath(GameManager.I.Graph, start, goal);

        if (path != null) {
            if (path.Count > 1) {
                path.Remove(goal);
            }
            return path;
        }

        return null;
    }

    public List<Node> GetPathWithoutBlock(Node a, Node b, List<Node> visited = null) {
        List<Node> path = null;
        path = AstarAlgorithm.CalculatePathWithoutBlock(GameManager.I.Graph, a, b, visited);
        if (path == null) path = new List<Node>();
        return path;
    }

#endregion
}
