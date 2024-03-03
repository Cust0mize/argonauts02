using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesDropPoint : BaseInObject {
	public List<Resource> TargetResources;

    public bool IsEmpty {
        get{
            return resourceObject == null;
        }
    }

    private BaseResourceObject resourceObject;
    private Node resouceObjectNode;

    public override bool CanBeUsed () {
        return false;
    }

    protected override void Awake () {
        Node.IsFree = true;
    }

    public void Drop (List<Resource> resources, Vector3 productionObject) {
        AudioWrapper.I.PlaySfx("item_appear");

        resourceObject = GameManager.I.SpawnObjectItem(resources[0].Type, resources[0].Count, transform.position - Vector3.up * 0.1f, Node, resouceObjectNode).GetComponent<BaseResourceObject>();
        resourceObject.transform.position = productionObject;

        if (resouceObjectNode == null) resouceObjectNode = resourceObject.Node;

        StartCoroutine(IEDrop(productionObject, resourceObject.transform));
	}

    private IEnumerator IEDrop(Vector3 productionObject, Transform target) {
        target.GetComponent<NormalResourceObject>().SetActiveTrace(true);
        yield return MoveToPoint(target, productionObject, transform.position - Vector3.up * 0.1f, 250f);
        target.GetComponent<NormalResourceObject>().SetActiveTrace(false);
        yield return new WaitForSeconds(0.5f);
        target.GetComponent<NormalResourceObject>().SetActiveTrace(false);
    }

    private IEnumerator MoveToPoint(Transform target, Vector3 from, Vector3 to, float speed) {
        if (to.Equals(from)) {
            yield break;
        }

        Vector3 fly_direction = to - from;
        float distance = fly_direction.magnitude;
        fly_direction.Normalize();
        Vector3 curr_direction = fly_direction;

        Vector3 crossDir = Vector3.Cross((to - from).normalized, Vector3.forward); //dir
        Vector3 crossDirScale = crossDir * (Vector3.Distance(from, to) * 0.5F); //dir * scale
        if (from.x <= to.x) crossDirScale = -crossDirScale; //reverse if need
        Vector3 middle_pnt = crossDirScale + Vector3.Lerp(from, to, 0.5F); //end middle point

        float curr_distance = 0;
        Vector3 currValue = Vector3.zero;
        Vector3 currPos = Vector3.zero;

        Vector3 startScale = new Vector3(0.3f, 0.3f, 0.3f);

        while (true) {
            target.transform.localScale = Vector3.Lerp(startScale, Vector3.one, curr_distance / distance);

            currValue = fly_direction * speed * Time.deltaTime;
            curr_distance += currValue.magnitude;
            currPos = Utils.GetBezier2Point(from, middle_pnt, to, curr_distance / distance);

            curr_direction = currPos - target.position;
            curr_direction.Normalize();

            target.position = currPos;

            bool isX = false;
            bool isY = false;

            if ((curr_direction.x > 0 && currPos.x >= to.x) ||
                (curr_direction.x < 0 && currPos.x <= to.x))
                isX = true;

            if ((curr_direction.y > 0 && currPos.y >= to.y) ||
                (curr_direction.y < 0 && currPos.y <= to.y))
                isY = true;

            if (Vector3.Distance(currPos, to) < 3F) {
                break;
            }

            if (isX && isY)
                break;

            yield return 0;
        }

        target.transform.position = to;
        target.transform.localScale = Vector3.one;
    }
}
