﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingChestElement : MonoBehaviour {
    [SerializeField] private float speed;

    public IEnumerator DoFly(Vector2 from, Transform to, Vector2 fromScale, Vector2 toScale)
    {
        if (to.position.Equals(from)) {
            yield break;
        }

        Vector3 fly_direction = (Vector2)to.position - from;
        float distance = fly_direction.magnitude;
        fly_direction.Normalize();
        Vector3 curr_direction = fly_direction;

        Vector3 crossDir = Vector3.Cross(((Vector2)to.position - from).normalized, Vector3.forward); //dir
        Vector3 crossDirScale = crossDir * (Vector3.Distance(from, (Vector2)to.position) * 0.5F); //dir * scale
        if (from.x <= to.position.x) crossDirScale = -crossDirScale; //reverse if need
        Vector3 middle_pnt = crossDirScale + Vector3.Lerp(from, (Vector2)to.position, 0.5F); //end middle point

        float curr_distance = 0;
        Vector3 currValue = Vector3.zero;
        Vector3 currPos = Vector3.zero;

        float maxDistanceError = Vector2.Distance(from, to.position) * 1.25F;

        while (true) {
            fly_direction = (Vector2)to.position - from;
            distance = fly_direction.magnitude;
            fly_direction.Normalize();
            curr_direction = fly_direction;

            currValue = fly_direction * speed * Time.deltaTime;
            curr_distance += currValue.magnitude;
            currPos = Utils.GetBezier2Point(from, middle_pnt, (Vector2)to.position, curr_distance / distance);

            transform.GetComponent<RectTransform>().sizeDelta = Vector3.Lerp(fromScale, toScale, curr_distance / distance);

            curr_direction = currPos - transform.position;
            curr_direction.Normalize();

            transform.position = currPos;

            bool isX = false;
            bool isY = false;

            if ((curr_direction.x > 0 && currPos.x >= to.position.x) ||
                (curr_direction.x < 0 && currPos.x <= to.position.x))
                isX = true;

            if ((curr_direction.y > 0 && currPos.y >= to.position.y) ||
                (curr_direction.y < 0 && currPos.y <= to.position.y))
                isY = true;

            if (isX && isY)
                break;

            if (Vector3.Distance(currPos, (Vector2)to.position) < 25F) {
                transform.position = (Vector2)to.position;
                break;
            }

            if (Vector3.Distance(currPos, (Vector2)to.position) >= maxDistanceError) {
                transform.position = (Vector2)to.position;
                break;
            }

            yield return 0;
        }

        transform.position = (Vector2)to.position;
    }
}
