using System.Collections;
using UnityEngine;

public class Scaller : MonoBehaviour {
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private int factor = 1;
    [SerializeField] private float duration = 1f;

    public void Play() {
        StopAllCoroutines();
        StartCoroutine(IEPlay());
    }

    private IEnumerator IEPlay() {
        float down = 0f;
        while (down < duration) {
            Vector3 s = Vector3.one * curve.Evaluate((down / duration));
            transform.localScale = new Vector3(s.x * factor, s.y, s.z);
            down += Time.deltaTime;
            yield return null;
        }
    }
}
