using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoPathController : MonoBehaviour {
    public IEnumerator NoPathWorker(List<GameObject> path, float growSpeed, float shrinkSpeed) {
        if (path.Count.Equals(0)) yield break;

        float currSize = 0.8F;
        while (currSize < 1F) {
            for (int i = 0; i < path.Count; i++) {
                path[i].transform.localScale += Vector3.one * growSpeed * Time.deltaTime;
            }
            currSize = path[0].transform.localScale.x;
            yield return null;
        }

        while (currSize > 0F) {
            for (int i = 0; i < path.Count; i++) {
                path[i].transform.localScale -= Vector3.one * shrinkSpeed * Time.deltaTime;
            }
            currSize = path[0].transform.localScale.x;
            yield return null;
        }
    }

    public IEnumerator DestroyPath(List<GameObject> path, float delay) {
        yield return new WaitForSeconds(delay);

        foreach (GameObject p in path) {
            Destroy(p);
        }

        path.Clear();
    }
}
