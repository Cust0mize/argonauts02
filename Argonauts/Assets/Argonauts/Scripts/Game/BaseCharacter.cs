using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class BaseCharacter : MonoBehaviour {
    public enum Stages {
        Ready,
        InWay,
        Working,
        Returning,
        Free,
        FreeReady
    }

    public enum ActionStages {
        Mine,
        Build,
        Dig,
        Cut,
        Attack
    }

    public enum Types {
        Worker,
        Jaison,
        Medea
    }

    public bool IsFake = false;

    private Animator animator;
    private Vector3 lastPosition;
    private Vector2 dir;
    private float dirX = 0;
    private float dirY = 0;
    private Image resourceIcon;

    public Types Type;

    [SerializeField]
    private Stages stage;
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private GameObject resourceIconPrefab;
    [SerializeField]
    private Vector3 offsetResourceIcon;
    [SerializeField]
    private GameObject triggerPointEffect;
    [SerializeField]
    private GameObject shadow;
    [SerializeField]
    private GameObject boatEffect;

    private bool boatEquiped;
    public bool BoatEquiped {
        get {
            return boatEquiped;
        }
        set {
            boatEquiped = value;
            if (Animator.HasBool("Boat")) Animator.SetBool("Boat", boatEquiped);
            shadow.gameObject.SetActive(!value);
            boatEffect.gameObject.SetActive(value);
        }
    }

    public float Speed {
        get {
            return (GameManager.I.SpeedBonusActive ? speed * 2 : speed) * (GameBonusesManager.I.BonuseIsActive(GameBonus.GameBonusTypes.CharacterMoveSpeed) ? 1.1F : 1F);
        }
    }

    protected virtual Image ResourceIcon {
        get {
            if (resourceIcon == null) {
                resourceIcon = Instantiate(resourceIconPrefab, GameManager.I.Canvas2D.transform).GetComponent<Image>();
                resourceIcon.transform.position = transform.position + offsetResourceIcon;
                resourceIcon.gameObject.SetActive(false);
            }
            return resourceIcon;
        }
    }

    public virtual Stages Stage {
        get {
            return stage;
        }
        set {
            if (value == Stages.InWay || value == Stages.Returning) {
                if (Animator.HasBool("Run")) Animator.SetBool("Run", true);
                if (Animator.HasBool("Dig")) Animator.SetBool("Dig", false);
                if (Animator.HasBool("Cut")) Animator.SetBool("Cut", false);
                if (Animator.HasBool("Mine")) Animator.SetBool("Mine", false);
                if (Animator.HasBool("Build")) Animator.SetBool("Build", false);
                if (Animator.HasBool("Attack")) Animator.SetBool("Attack", false);
            } else if (value == Stages.Working) {
                if (Animator.HasBool("Run")) Animator.SetBool("Run", false);

                if (Target != null) {
                    if (Target.ActionStage == ActionStages.Build) {
                        if (Animator.HasBool("Dig")) Animator.SetBool("Dig", false);
                        if (Animator.HasBool("Cut")) Animator.SetBool("Cut", false);
                        if (Animator.HasBool("Mine")) Animator.SetBool("Mine", false);
                        if (Animator.HasBool("Build")) Animator.SetBool("Build", true);
                        if (Animator.HasBool("Attack")) Animator.SetBool("Attack", false);
                    } else if (Target.ActionStage == ActionStages.Mine) {
                        if (Animator.HasBool("Dig")) Animator.SetBool("Dig", false);
                        if (Animator.HasBool("Cut")) Animator.SetBool("Cut", false);
                        if (Animator.HasBool("Mine")) Animator.SetBool("Mine", true);
                        if (Animator.HasBool("Build")) Animator.SetBool("Build", false);
                        if (Animator.HasBool("Attack")) Animator.SetBool("Attack", false);
                    } else if (Target.ActionStage == ActionStages.Cut) {
                        if (Animator.HasBool("Dig")) Animator.SetBool("Dig", false);
                        if (Animator.HasBool("Cut")) Animator.SetBool("Cut", true);
                        if (Animator.HasBool("Mine")) Animator.SetBool("Mine", false);
                        if (Animator.HasBool("Build")) Animator.SetBool("Build", false);
                        if (Animator.HasBool("Attack")) Animator.SetBool("Attack", false);
                    } else if (Target.ActionStage == ActionStages.Dig) {
                        if (Animator.HasBool("Dig")) Animator.SetBool("Dig", true);
                        if (Animator.HasBool("Cut")) Animator.SetBool("Cut", false);
                        if (Animator.HasBool("Mine")) Animator.SetBool("Mine", false);
                        if (Animator.HasBool("Build")) Animator.SetBool("Build", false);
                        if (Animator.HasBool("Attack")) Animator.SetBool("Attack", false);
                    } else if (Target.ActionStage == ActionStages.Attack) {
                        if (Animator.HasBool("Dig")) Animator.SetBool("Dig", false);
                        if (Animator.HasBool("Cut")) Animator.SetBool("Cut", false);
                        if (Animator.HasBool("Mine")) Animator.SetBool("Mine", false);
                        if (Animator.HasBool("Build")) Animator.SetBool("Build", false);
                        if (Animator.HasBool("Attack")) Animator.SetBool("Attack", true);

                        if (Type == Types.Worker) {
                            if (Animator.HasBool("Mine")) Animator.SetBool("Mine", true);
                        }
                    }
                }
            } else {
                if (Animator.HasBool("Run")) Animator.SetBool("Run", false);
                if (Animator.HasBool("Dig")) Animator.SetBool("Dig", false);
                if (Animator.HasBool("Cut")) Animator.SetBool("Cut", false);
                if (Animator.HasBool("Mine")) Animator.SetBool("Mine", false);
                if (Animator.HasBool("Build")) Animator.SetBool("Build", false);
                if (Animator.HasBool("Attack")) Animator.SetBool("Attack", false);
            }

            stage = value;
            OnStageChandeg.Invoke(this);

            GameManager.I.CampObject.UpdateWorkerIcons();
        }
    }

    private GameObject homeWorkPoint;
    public GameObject HomeWorkPoint {
        get {
            if(homeWorkPoint == null) {
                homeWorkPoint = Home == null ? GameManager.I.CampObject.GetFirstFreePoint(transform.position, false) : Home.GetFirstFreePoint(transform.position, false);
            }
            return homeWorkPoint;
        }
    }

    public Animator Animator {
        get {
            if (animator == null) {
                animator = GetComponent<Animator>();
            }
            return animator;
        }
    }

    public List<Resource> Backpack;

    private List<Node> path;
    public BaseInObject Target;
    public BaseInObject Home;
    public Node LastPassedNode;

    private GameObject currentWorkPoint;
    private Action onReachTeleport;
    private BaseInObject cachedObject;

    public BaseInObject CachedObject {
        get {
            return cachedObject;
        }
    }

    public event Action<BaseCharacter> OnStageChandeg = delegate { };

    private const float ERROR_CLOSE_WALK = 1F;
    private const float ERROR_WALK = 10F;
    private const float DURATION_WAIT = 2F;

    private readonly Vector3 leftScale = new Vector3(-1, 1, 1);
    private readonly Vector3 rightScale = new Vector3(1, 1, 1);

    private readonly string[] jaisonAttacks = new string[3] { "jason_attack01", "jason_attack02", "jason_attack03" };

    private void Start() {
        lastPosition = transform.position;
    }

    private void LateUpdate() {
        if (Stage == Stages.InWay || Stage == Stages.Returning) {
            if (resourceIcon != null)
                resourceIcon.transform.position = transform.position + offsetResourceIcon;

        }
    }

    private void Update() {
        if (Stage == Stages.InWay || Stage == Stages.Returning) {
            dir.x = transform.position.x - lastPosition.x;
            dir.y = transform.position.y - lastPosition.y;
            dir.Normalize();

            dirX = 0;
            dirY = 0;

            if (dir.x > 0.25f) {
                dirX = 1;
            }
            if (dir.x < -0.25f) {
                dirX = -1;
            }

            if (dir.y > 0.25f) {
                dirY = 1;
            }
            if (dir.y < -0.25f) {
                dirY = -1;
            }

            transform.localScale = dir.x > 0 ? rightScale : leftScale;

            Animator.SetFloat("SpeedX", dirX);
            Animator.SetFloat("SpeedY", dirY);

            lastPosition = transform.position;
        }
    }

    public void MoveToObject(BaseInObject target, List<Node> path) {
        StopAllCoroutines();

        this.path = path;

        Target = target;
        Stage = Stages.InWay;
        LevelTaskPingController.I.DisablePin(Target);
        if (Target is BuildObject)
            LevelTaskPingController.I.DisablePin((Target as BuildObject).BuildObjectController);

        AudioWrapper.I.PlaySfx("worker_start_walk");

        target.SetCheckmarkActivity(true);

        StartCoroutine(ReachCoroutine(OnWorkReach, path));
    }

    public void MoveToHome(BaseInObject target, List<Node> path) {
        StopAllCoroutines();

        this.path = path;

        Target = target;
        Stage = Stages.Returning;

        StartCoroutine(ReachCoroutine(OnHomeReach, path));
    }

    private void OnWorkReach() {
        currentWorkPoint = Target.GetFirstFreePoint(transform.position, !(Target is CampObject));
        StartCoroutine(WorkingCoroutine());
    }

    private void OnHomeReach() {
        if(Target == null) { // check if me upgrade tent and link became null
            Target = GameManager.I.CampObject;
        }

        StartCoroutine(WalkCloseToHome());
    }

    private IEnumerator WorkingCoroutine() {
        while (Vector2.Distance(transform.position, currentWorkPoint.transform.position) > ERROR_CLOSE_WALK) {
            transform.position = Vector2.MoveTowards(transform.position, currentWorkPoint.transform.position, Speed * Time.deltaTime);
            yield return null;
        }

        Stage = Stages.Working;

        dir.x = Target.transform.position.x - transform.position.x;
        dir.y = Target.transform.position.y - transform.position.y;
        dir.Normalize();

        dirX = 0;
        dirY = 0;

        if (dir.x > 0.25f) {
            dirX = 1;
        }
        if (dir.x < -0.25f) {
            dirX = -1;
        }

        if (dir.y > 0.25f) {
            dirY = 1;
        }
        if (dir.y < -0.25f) {
            dirY = -1;
        }

        transform.localScale = dir.x > 0 ? rightScale : leftScale;

        Animator.SetFloat("SpeedX", dirX);
        Animator.SetFloat("SpeedY", dirY);

        Target.SetCheckmarkActivity(false);

        cachedObject = Target;

        Vector3 cachePosition = Target.transform.position;

        StartCoroutine(OnStartBaseAnimationEvent());

        if (Target is MarketPlaceObject) {
            gameObject.Hide();
        }

        if (Type == Types.Worker) {
            if (Target.ActionStage == ActionStages.Dig) {
                AudioWrapper.I.PlaySfx("worker_dig");
            }
        }

        yield return Target.Use(OnGetResources);

        if (Target is MarketPlaceObject) {
            gameObject.Show();
        }

        OnEndBaseAnimationEvent(cachePosition);

        OverseerController.I.RemoveMeFromOrder(Target);

        Target.FreePoint(currentWorkPoint);

        if (Backpack.Count.Equals(0)) {
            Stage = Stages.FreeReady;
            GameManager.I.CampObject.UpdateWorkerIcons();
            yield return new WaitForSeconds(DURATION_WAIT);
        }

        List<Node> path = OverseerController.I.GetPathForCharacter(LastPassedNode != null ? LastPassedNode : CachedObject.Node, Home.Node);
        MoveToHome(Home == null ? GameManager.I.CampObject : Home, path);
    }

    private IEnumerator WalkCloseToHome() {
        while (Vector2.Distance(transform.position, HomeWorkPoint.transform.position) > ERROR_CLOSE_WALK) {
            transform.position = Vector2.MoveTowards(transform.position, HomeWorkPoint.transform.position, Speed * Time.deltaTime);
            yield return null;
        }

        List<Resource> backpack = new List<Resource>(Backpack);
        foreach (Resource r in backpack) {
            if (GameManager.I.ResourceBonusActive)
                r.Count += 1;
        }

        GameManager.I.AddResources(backpack, true);
        Destroy(resourceIcon);
        Stage = Stages.Free;

        OverseerController.I.OnArriveHome(cachedObject);
    }

    private void OnGetResources(List<Resource> resources) {
        Backpack = resources;
        if (resources.Count > 0) {
            ResourceIcon.sprite = ResourceManager.I.GetResourceSprite(string.Format("{0}{1:00}", resources[0].Type.ToString(), Mathf.Clamp(resources[0].Count, 1, 3)));
            resourceIcon.gameObject.SetActive(true);
        }
    }

    private IEnumerator ReachCoroutine(Action callback, List<Node> path)
    {
        int step = 0;

        PortalObject teleportedTo = null;
        PortObject supposedPort = null;
        bool waitForPortIn = false;
        bool waitForPortOut = false;

        if (supposedPort == null && !BoatEquiped) {
            if (step + 1 < path.Count) supposedPort = GameManager.I.Ports.FirstOrDefault(x => x != null && (x.Node == path[step] || x.Node == path[step + 1]));
            else supposedPort = GameManager.I.Ports.FirstOrDefault(x => x != null && (x.Node == path[step]));

            if (supposedPort != null) {
                path.Insert(step, supposedPort.GetFirstFreePointAsNode(transform.position, false));
                waitForPortIn = true;
            }
        }

        if (GameManager.I.Portals.Exists(x => x != null && x.Node == path[step] && x != teleportedTo)) {
            PortalObject teleportedFrom = GameManager.I.Portals.Where(x => x != null && x.Node == path[step] && x != teleportedTo).FirstOrDefault();
            teleportedFrom.Transfer(this);
            teleportedTo = teleportedFrom.MirrorPortalController.BaseBuildObject.GetComponent<PortalObject>();
        }

        LastPassedNode = path[step];

        step++;

        while (step < path.Count) {
            if (Vector2.Distance(transform.position, path[step].Position) < ERROR_WALK) {
                LastPassedNode = path[step];
                if (GameManager.I.Portals.Exists(x => x != null && x.Node == path[step] && x != teleportedTo && x.Node != Target.Node)) {
                    PortalObject teleportedFrom = GameManager.I.Portals.Where(x => x != null && x.Node == path[step] && x != teleportedTo).First();
                    teleportedFrom.Transfer(this);

                    teleportedTo = teleportedFrom.MirrorPortalController.BaseBuildObject.GetComponent<PortalObject>();

                    step++;
                    continue;
                }

                if (waitForPortIn) {
                    path.Remove(supposedPort.Node);

                    Node nextNode = path[step + 1];
                    if (step + 2 < path.Count) nextNode = path[step + 2];
                    Vector2 nextPoint = nextNode.Position;

                    supposedPort.EquipBoat(this, nextPoint, nextNode);
                    supposedPort = null;
                    waitForPortIn = false;
                    step++;
                    continue;
                }

                if (waitForPortOut) {
                    path.Remove(supposedPort.Node);

                    Node nextNode = path[step + 1];
                    if (step + 2 < path.Count) nextNode = path[step + 2];

                    supposedPort.UnequipBoat(this, path[step + 1].Position, nextNode);
                    supposedPort = null;
                    waitForPortOut = false;
                    step++;
                    continue;
                }

                if (supposedPort == null && step + 1 < path.Count) {
                    supposedPort = GameManager.I.Ports.FirstOrDefault(x => x != null && (x.Node == path[step + 1]) && x.Node != Target.Node);
                    if (supposedPort != null) {
                        if (BoatEquiped) {
                            path.Insert(step + 1, supposedPort.GetNearOutPoint(path[step].Position));
                            waitForPortOut = true;
                        } else {
                            waitForPortIn = true;
                        }
                    }
                }

                if (step + 1 < path.Count)
                    step++;
                else break;
            }

            transform.position = Vector2.MoveTowards(transform.position, path[step].Position, Speed * Time.deltaTime);
            yield return null;
        }

        if (callback != null) {
            callback.Invoke();
        }
    }

    #region Animation events

    private void OnBaseOneShotAnimationEvent(ActionStages needStage) {
        if (cachedObject is BuildObject) {
            if (triggerPointEffect != null && (cachedObject as BaseInObject).ActionStage == needStage) {
                (cachedObject as BaseInObject).PlayOneShotUseEffect(triggerPointEffect.transform.position, transform.position);
            }
        } else {
            if (triggerPointEffect != null && cachedObject is BaseResourceObject && (cachedObject as BaseResourceObject).ActionStage == needStage) {
                bool needCheckOrder = needStage == ActionStages.Cut || needStage == ActionStages.Build ? false : true;
                (cachedObject as BaseResourceObject).PlayOneShotUseEffect(triggerPointEffect.transform.position, transform.position, needCheckOrder);
            }
        }
    }

    public void OnCutAnimationEvent() {
        if(cachedObject != null) {
            AudioWrapper.I.PlaySfx("worker_cut");
            OnBaseOneShotAnimationEvent((cachedObject).ActionStage);
        }
    }

    public void OnMineAnimationEvent() {
        if (cachedObject != null) {
            AudioWrapper.I.PlaySfx("worker_mine");
            OnBaseOneShotAnimationEvent((cachedObject).ActionStage);
        }
    }

    public void OnBuildAnimationEvent() {
        if (cachedObject != null) {
            AudioWrapper.I.PlaySfx("worker_build");
            OnBaseOneShotAnimationEvent((cachedObject).ActionStage);
        }
    }

    public void OnAttackAnimatonEvent() {
        if (cachedObject != null) {
            if (Type == Types.Medea) {
                AudioWrapper.I.PlaySfx("medea_attack");
            } else if (Type == Types.Jaison) {
                AudioWrapper.I.PlaySfx(jaisonAttacks[UnityEngine.Random.Range(0, jaisonAttacks.Length)]);
            }

            OnBaseOneShotAnimationEvent((cachedObject).ActionStage);
        }
    }

    public IEnumerator OnStartBaseAnimationEvent() {
        yield return new WaitForSeconds(0.1f);
        if (cachedObject != null) {
            if (cachedObject is BaseResourceObject) {
                switch ((cachedObject as BaseResourceObject).ActionStage) {
                    case ActionStages.Dig:
                        OnStartDigAnimationEvent();
                        break;
                }
            }
        }
    }

    public void OnEndBaseAnimationEvent(Vector3 position) {
        if (cachedObject != null) {
            if (cachedObject is BaseResourceObject) {
                switch ((cachedObject as BaseResourceObject).ActionStage) {
                    case ActionStages.Dig:
                        OnEndDigAnimationEvent();
                        break;
                }

                //todo: maybe need check if anyway resource count great one
                if ((cachedObject as BaseResourceObject).Resources.Count > 0) {
                    (cachedObject as BaseResourceObject).PlayPickupEffect(position);
                }
            }
        }
    }

    public void OnStartDigAnimationEvent() {
        if (cachedObject is BaseResourceObject && (cachedObject as BaseResourceObject).ActionStage == ActionStages.Dig) {
            (cachedObject as BaseResourceObject).PlayUseEffect(triggerPointEffect.transform.position, transform.position, false);
    	}
    }

    public void OnEndDigAnimationEvent() {
        if (cachedObject is BaseResourceObject && (cachedObject as BaseResourceObject).ActionStage == ActionStages.Dig) {
            (cachedObject as BaseResourceObject).StopUseEffect();
    	}
    }

    #endregion

    #region Happy Region

    public void BeHappy()
    {
        if (BoatEquiped) {
            StartCoroutine(WaitBoatOffAndHappy());
            return;
        }

        StopAllCoroutines();

        Stage = Stages.Ready;

        Animator.SetBool("Happy", true);
    }

    private IEnumerator WaitBoatOffAndHappy()
    {
        while (BoatEquiped) {
            yield return null;
        }
        BeHappy();
    }

    public void Release() {
        StartCoroutine(IERealese());
    }

    private IEnumerator IERealese() {
        GameObject p = GameManager.I.CampObject.GetHappyPoint();

        float progressMove = 0f;

        Stage = Stages.InWay;
        while (Vector2.Distance(transform.position, p.transform.position) > .5f) {
            transform.position = Vector2.MoveTowards(transform.position, p.transform.position, progressMove);
            progressMove += Time.deltaTime * 30f;
            yield return null;
        }

        BeHappy();
    }

    #endregion
}
