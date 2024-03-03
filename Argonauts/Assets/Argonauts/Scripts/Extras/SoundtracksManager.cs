using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundtracksManager : MonoBehaviour {
    [SerializeField] private UITweener[] tweeners;

	public Toggle toggleTrack01;
	public Toggle toggleTrack02;
	public Toggle toggleTrack03;
	public Toggle toggleTrack04;
	public Toggle toggleTrack05;

    [SerializeField] private AudioClip[] clips;
    private AudioClip currAudioClip = null;

    private IEnumerator Start() {
        FadeManager.I.ForceFadeIn();
        yield return new WaitForSeconds(0.1F);
        yield return FadeManager.I.FadeOut(2F);

        foreach (UITweener tw in tweeners) {
            tw.PlayForward();
        }
    }

    private void StopCurrentAudioClip() {
		if (currAudioClip != null) {
			AudioManager.Instance.StopSound (currAudioClip);
			currAudioClip = null;
		}
	}

	private IEnumerator IE_PlayNewAudioTrack(AudioClip clip) {
		yield return new WaitForSeconds (0.1f);
		currAudioClip = clip;
		AudioManager.Instance.PlaySound (currAudioClip, AudioManager.TypesAudioSource.Music, true);
	}

	private void PlayNewAudioTrack(int index) {
		StopAllCoroutines ();
		StartCoroutine(IE_PlayNewAudioTrack(clips[index]));
	}

	public void OnToggle_Track01() {
		StopCurrentAudioClip ();

        UpdateToggles();

		if (toggleTrack01.isOn) {
			PlayNewAudioTrack (0);
        }
	}

	public void OnToggle_Track02() {
		StopCurrentAudioClip ();

        UpdateToggles();

		if (toggleTrack02.isOn) {
			PlayNewAudioTrack (1);
		}
	}

	public void OnToggle_Track03() {
		StopCurrentAudioClip ();

        UpdateToggles();

		if (toggleTrack03.isOn) {
			PlayNewAudioTrack (2);
		}
	}

	public void OnToggle_Track04() {
		StopCurrentAudioClip ();

        UpdateToggles();

		if (toggleTrack04.isOn) {
			PlayNewAudioTrack (3);
		}
	}

	public void OnToggle_Track05() {
		StopCurrentAudioClip ();

        UpdateToggles();

		if (toggleTrack05.isOn) {
			PlayNewAudioTrack (4);
		}
	}

    private void UpdateToggles() {
        toggleTrack01.targetGraphic.enabled = !toggleTrack01.isOn;
        toggleTrack02.targetGraphic.enabled = !toggleTrack02.isOn;
        toggleTrack03.targetGraphic.enabled = !toggleTrack03.isOn;
        toggleTrack04.targetGraphic.enabled = !toggleTrack04.isOn;
        toggleTrack05.targetGraphic.enabled = !toggleTrack05.isOn;
    }

	public void OnBtn_Back() {
		StopCurrentAudioClip ();
        TransitionManager.I.LoadMenu ();
	}
}
