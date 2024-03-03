using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupScoreElementGUI : MonoBehaviour {
    [SerializeField] TextMeshProUGUI countText;
    [SerializeField] ParticleSystem trail;
    [SerializeField] Color color = Color.yellow;

    private const float amplitude = 1f;
    private const float frequency = 3f;
    private const float offset = 0.9f;

    public void Init(int score) {
        countText.text = score.ToString();
        countText.transform.GetComponent<TextMeshProUGUI>().color = color;
    }

    public void DoPopUp(float lifetime, float speed) {
        StopAllCoroutines();
        StartCoroutine(IEDoPopUp(lifetime, speed));
    }

    public void DoPopUp(Vector3 from, Vector3 to, float speed, Action callback) {
        StartCoroutine(MoveToPoint(from, to, speed, () => { DestroyImmediate(gameObject); callback.Invoke(); }));
    }

    private IEnumerator IEDoPopUp(float lifetime, float speed) {
        transform.position += Vector3.up * 1f;

        float startLifetime = lifetime;

        while (lifetime > 0) {
            transform.position += Vector3.up * Time.deltaTime * speed;
            transform.localScale = Vector3.one * GetSinusoid((startLifetime - lifetime) / (startLifetime * 2F));
            lifetime -= Time.deltaTime;
            yield return null;
        }
        DestroyImmediate(gameObject);
    }

    private float GetSinusoid(float t) {
        return amplitude * Mathf.Sin(frequency * t + offset);
    }

    private IEnumerator MoveToPoint(Vector3 from, Vector3 to, float speed, Action callback) {
        if (to.Equals(from)) {
            yield break;
        }

        trail.gameObject.SetActive(true);
        trail.Stop();
        trail.Play();

        Vector3 fly_direction = to - from;
        float distance = fly_direction.magnitude;
        fly_direction.Normalize();
        Vector3 curr_direction = fly_direction;

        Vector3 crossDir = Vector3.Cross((to - from).normalized, Vector3.forward); //dir
        Vector3 crossDirScale = crossDir * (Vector3.Distance(from, to) * 0.5F); //dir * scale
        if (from.x >= to.x) crossDirScale = -crossDirScale; //reverse if need
        Vector3 middle_pnt = crossDirScale + Vector3.Lerp(from, to, 0.5F); //end middle point

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

        trail.Stop();
        callback.Invoke();
    }
}
