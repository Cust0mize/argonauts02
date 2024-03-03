using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarItem : MonoBehaviour {
    public IEnumerator MoveToPoint(Vector3 from, Vector3 to, float speed) {
        AudioWrapper.I.PlaySfx("star_disappear");

        if (to.Equals(from)) {
            yield break;
        }

        Vector3 fly_direction = to - from;
        float distance = fly_direction.magnitude;
        fly_direction.Normalize();
        Vector3 curr_direction = fly_direction;

        Vector3 middle_pnt = from + fly_direction * (distance / 2);
        middle_pnt.x = from.x + 0.0001f;
        middle_pnt.z = to.z + 0.0001f;

        float curr_distance = 0;
        Vector3 currValue = Vector3.zero;
        Vector3 currPos = Vector3.zero;

        while (true) {
            currValue = fly_direction * speed * Time.deltaTime;
            curr_distance += currValue.magnitude;
            currPos = Utils.GetBezier2Point(from, middle_pnt, to, curr_distance / distance);

            curr_direction = currPos - transform.position;
            curr_direction.Normalize();

            transform.position = currPos;

            bool isX = false;
            bool isZ = false;

            if ((curr_direction.x > 0 && currPos.x >= to.x) ||
                (curr_direction.x < 0 && currPos.x <= to.x))
                isX = true;

            if ((curr_direction.z > 0 && currPos.z >= to.z) ||
                (curr_direction.z < 0 && currPos.z <= to.z))
                isZ = true;

            if (isX && isZ)
                break;

            yield return 0;
        }

        transform.position = to;
    }
}
