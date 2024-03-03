using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarsController : MonoBehaviour {
	[SerializeField] private Image time;
	[SerializeField] public Image[] stars;
	[SerializeField] private GameObject rightEdge;
	[SerializeField] private float speed;
	[SerializeField] private ParticleSystem miniExplosion;
	private bool particleLifetime;

	private float time1;
	private float time2;
	private float time3;

    private bool star1Effect;
    private bool star2Effect;
    private bool star3Effect;

    private float time1Sfx;
    private float time2Sfx;
    private float time3Sfx;

    private bool star1Sfx;
    private bool star2Sfx;
    private bool star3Sfx;

	private void Start () {
		StartCoroutine (StarEffect (stars, rightEdge, speed));
	}

	public void InitTime (Level level) {
        time1 = 1F - (level.Time1 / level.TotalTime);
        time2 = 1F - (level.Time2 / level.TotalTime);
        time3 = 1F - (level.Time3 / level.TotalTime);

        time1Sfx = 1F - ((level.Time1 - 10) / level.TotalTime);
        time2Sfx = 1F - ((level.Time2 - 10) / level.TotalTime);
        time3Sfx = 1F - ((level.Time3 - 10) / level.TotalTime);
	}

	private IEnumerator EffectMiniExplosion (Image star) {
        miniExplosion.transform.position = new Vector3(star.transform.position.x, star.transform.position.y, 0.0f);
        miniExplosion.Play();
        yield return new WaitForSeconds(miniExplosion.main.startLifetime.constant);
	}

	private IEnumerator StarEffect (Image[] stars, GameObject rightEdge, float speed) {
		while (true) {
            if (time.fillAmount < time3Sfx && !star3Sfx) {
                star3Sfx = true;
                AudioWrapper.I.PlaySfx("clock");
            }
            if (time.fillAmount < time2Sfx && !star2Sfx) {
                star2Sfx = true;
                AudioWrapper.I.PlaySfx("clock");
            }
            if (time.fillAmount < time1Sfx && !star1Sfx) {
                star1Sfx = true;
                AudioWrapper.I.PlaySfx("clock");
            }

            if (time.fillAmount < time3 && !star3Effect) {
                star3Effect = true;

                yield return EffectMiniExplosion(stars[0]);
                yield return stars[0].GetComponent<StarItem>().MoveToPoint(stars[0].transform.position, rightEdge.transform.position, speed);
                Destroy(stars[0]);
            }
            if (time.fillAmount < time2 && !star2Effect) {
                star2Effect = true;

                yield return EffectMiniExplosion(stars[1]);
                yield return stars[1].GetComponent<StarItem>().MoveToPoint(stars[1].transform.position, rightEdge.transform.position, speed);
                Destroy(stars[1]);
            }
            if (time.fillAmount < time1&& !star1Effect) {
                star1Effect = true;

                yield return EffectMiniExplosion(stars[2]);
                yield return stars[2].GetComponent<StarItem>().MoveToPoint(stars[2].transform.position, rightEdge.transform.position, speed);
                Destroy(stars[2]);
            }
			yield return null;
		}
	}
}
