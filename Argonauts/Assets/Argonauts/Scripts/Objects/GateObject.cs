using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GateObject : BaseInObject {
	[SerializeField] Sprite openedSprite;
    [SerializeField] private ParticleSystem effectOpen;

    public override event Action<Node> OnDestroyed = delegate { };

    public void DoOpen (Vector3 pos, GameObject keyPrefab) {
        StartCoroutine(IEDoOpen(pos, keyPrefab));
	}

    private IEnumerator IEDoOpen(Vector3 pos, GameObject keyPrefab) {
        GameObject k = Instantiate(keyPrefab, pos, Quaternion.identity);
        yield return MoveToPoint(k.transform, pos, transform.position - Vector3.up * 0.1f, 500f);
        ParticleSystem keyEffect = k.transform.GetChild(0).GetComponent<ParticleSystem>();
        keyEffect.transform.SetParent(null);
        keyEffect.Stop();
        Destroy(k);
        

        if (effectOpen != null)
            effectOpen.Play();

        Node.IsFree = true;
        GetComponent<SpriteRenderer>().sprite = openedSprite;
        AudioWrapper.I.PlaySfx("gate_opened");

        OnDestroyed.Invoke(Node);

        yield return new WaitForSeconds(2f);
        Destroy(keyEffect);
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

        while (true) {
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
    }
}
