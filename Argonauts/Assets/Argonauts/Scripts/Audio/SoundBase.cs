using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class SoundBase : MonoBehaviour {
    [Tooltip("Name.")]
    [SerializeField]
    protected string m_name;

    [Tooltip("Group name.")]
    [SerializeField]
    protected string m_groupName;

    [Tooltip("Loop.")]
    [SerializeField]
    protected bool m_doLoop;

    [Tooltip("Volume.")]
    [SerializeField]
    [Range(0, 1f)]
    protected float m_volume = 1f;

    [Tooltip("Audio Source.")]
    [SerializeField]
    protected AudioSource m_audioSource;

    public virtual string Name {
        get { return m_name; }
        set { m_name = value; }
    }

    public virtual float Volume {
        get { return m_volume; }
        set {
            m_volume = Mathf.Clamp01(value);
            m_audioSource.volume = m_volume;
        }
    }

    public virtual string GroupName {
        get { return m_groupName; }
        set { m_groupName = value; }
    }

    public virtual AudioSource AudioSource {
        get { return m_audioSource; }
        set { m_audioSource = value; }
    }

    public virtual bool DoLoop {
        get { return m_doLoop; }
        set { m_doLoop = value; }
    }

    public virtual bool IsPlaying {
        get { return m_audioSource.isPlaying; }
    }

    public UnityEvent OnFinishStep = new UnityEvent();

    public UnityEvent OnFinishAll = new UnityEvent();

    public virtual bool Play() {
        StopCoroutine("SoundFadeOut");

        this.AudioSource.volume = Volume;

        this.AudioSource.Play();
        StopCoroutine("CallFinishEvents");
        StartCoroutine("CallFinishEvents");

        return true;
    }

    public virtual bool Stop(float time = 0) {
        if (!IsPlaying) {
            return false;
        }

        if (time > 0) {
            StartCoroutine(SoundFadeOut(time));
            return true;
        }

        StopCoroutine("CallFinishEvents");
        this.AudioSource.Stop();
        OnFinishAll.Invoke();

        return true;
    }

    public virtual bool Pause() {
        if (AudioListener.pause) {
            return false;
        }

        StopCoroutine("CallFinishEvents");
        this.AudioSource.Pause();

        return true;
    }

    public virtual bool Restart() {
        this.AudioSource.Stop();

        return Play();
    }

    protected virtual bool Awake() {
        if (m_audioSource == null) {
            Debug.LogError("Audio Source.");
            return false;
        }

        this.AudioSource.loop = false;
        this.AudioSource.playOnAwake = false;
        this.AudioSource.volume = Volume;
        return true;
    }

    protected virtual IEnumerator SoundFadeOut(float timeDelay) {
        float l_stopTime = Time.time + timeDelay;
        float l_volumeDelayAspect = Volume / timeDelay;

        while (Time.time < l_stopTime && this.AudioSource.isPlaying) {
            this.AudioSource.volume = l_volumeDelayAspect * (l_stopTime - Time.time);
            yield return null;
        }

        Stop();

        this.AudioSource.volume = Volume;
    }

    protected virtual IEnumerator CallFinishEvents() {
        while (this.AudioSource.isPlaying || AudioListener.pause) {
            yield return null;
        }

        OnFinishStep.Invoke();

        if (DoLoop) {
            Play();
            yield break;
        }

        StopCoroutine("CallFinishEvents");
        OnFinishAll.Invoke();
    }
}