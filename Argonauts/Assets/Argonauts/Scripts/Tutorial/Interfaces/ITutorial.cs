using UnityEngine;

public interface ITutorial {
    bool IsOn { get; }
    void Launch();
    void StartObserveEvent(string nameEvent);
    void StopObserveEvent(string nameEvent);
    void StopAllObserveEvents();
    void StopTutorial();
    GameObject MoveArrow(Vector3 position, ArrowSide side, float distance = 20F, float moveDistance = 30F);
}
