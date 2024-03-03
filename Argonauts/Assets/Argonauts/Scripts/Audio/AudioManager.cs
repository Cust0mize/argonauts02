using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

[ComVisible(false)]
public class AudioManager : GlobalSingletonBehaviour<AudioManager> {
    public enum TypesAudioSource { Music, Sfx }

    private List<Data> m_sounds = new List<Data>();

    protected float m_volume = 1f;
    protected float m_volumeSounds = 1f;
    protected float m_volumeMusic = 1f;

    public virtual float Volume {
        get { return m_volume; }
        set {
            m_volume = Mathf.Clamp01(value);
            AudioListener.volume = m_volume;
        }
    }

    public virtual float VolumeSounds {
        get { return m_volumeSounds; }
        set {
            m_volumeSounds = Mathf.Clamp01(value);
            UpdateVolume();
        }
    }

    public virtual float VolumeMusic {
        get { return m_volumeMusic; }
        set {
            m_volumeMusic = Mathf.Clamp01(value);
            UpdateVolume();
        }
    }

    public bool IsPaused {
        get { return AudioListener.pause; }
    }

    public void DoInit(float volumeMusic, float volumeSounds, bool mute) {
        Volume = mute ? 0f : 1f;
        VolumeMusic = volumeMusic;
        VolumeSounds = volumeSounds;
    }

    public void Play() {
        AudioListener.pause = false;
    }

    public void Pause() {
        AudioListener.pause = true;
    }

    void OnVolumeSoundsChanged(float value) {
        VolumeSounds = value;
    }

    void OnVolumeMusicChanged(float value) {
        VolumeMusic = value;
    }

    void OnMuteAudioChanged(bool value) {
        AudioListener.volume = value ? 0F : 1F;
    }

    public bool PlaySound(string path, TypesAudioSource type = TypesAudioSource.Sfx, bool doLoop = false) {
        if (m_sounds.Any(data => data.path == path && !data.source.isPlaying)) {
            var l_soundData = m_sounds.First(data => data.path == path && !data.source.isPlaying);

            l_soundData.source.loop = doLoop;
            l_soundData.source.Play();

            return true;
        }

        var l_clip = Resources.Load(path) as AudioClip;

        if (l_clip == null) {
            Debug.LogWarning(string.Format("Звук '{0}' не найден.", path));
            return false;
        }

        bool l_additional;

        AudioSource l_audioSource = GetComponents<AudioSource>().FirstOrDefault(source => source.clip == null);
        if (l_audioSource == null) {
            l_audioSource = gameObject.AddComponent<AudioSource>();
            l_additional = true;
        } else {
            l_additional = false;
        }

        l_audioSource.clip = l_clip;
        l_audioSource.volume = type == TypesAudioSource.Music ? VolumeMusic : VolumeSounds;
        l_audioSource.loop = doLoop;
        Data l_data = new Data(path, l_audioSource, l_additional, type);
        m_sounds.Add(l_data);
        l_audioSource.Play();
        if (l_additional && !doLoop) {
            StartCoroutine(WaitUnilFinished(l_data));
        }

        return true;
    }

    public bool PlaySound(AudioClip clip, TypesAudioSource type = TypesAudioSource.Sfx, bool doLoop = false) {
        if (clip == null) {
            Debug.LogWarning("Sound not found.");
            return false;
        }

        if (m_sounds.Any(data => data.source.clip == clip && !data.source.isPlaying)) {
            Data l_soundData = m_sounds.First(data => data.source.clip == clip && !data.source.isPlaying);

            l_soundData.source.loop = doLoop;
            l_soundData.source.Play();

            return true;
        }

        bool l_additional;
        AudioSource l_audioSource = GetComponents<AudioSource>().FirstOrDefault(source => source.clip == null);
        if (l_audioSource == null) {
            l_audioSource = gameObject.AddComponent<AudioSource>();
            l_additional = true;
        } else {
            l_additional = false;
        }

        l_audioSource.clip = clip;
        l_audioSource.volume = type == TypesAudioSource.Music ? VolumeMusic : VolumeSounds;
        l_audioSource.loop = doLoop;
        Data l_data = new Data("", l_audioSource, l_additional, type);
        m_sounds.Add(l_data);
        l_audioSource.Play();
        if (l_additional && !doLoop) {
            StartCoroutine(WaitUnilFinished(l_data));
        }

        return true;
    }

    public bool PlaySound(AudioClip clip, float volume, float pitch, Vector3 position) {
        if (clip == null) {
            Debug.LogWarning("Sound not found.");
            return false;
        }

        if (m_sounds.Any(data => data.source.clip == clip && !data.source.isPlaying)) {
            Data l_soundData = m_sounds.First(data => data.source.clip == clip && !data.source.isPlaying);

            l_soundData.source.loop = false;
            l_soundData.source.Play();

            return true;
        }

        bool l_additional;
        AudioSource l_audioSource = GetComponents<AudioSource>().FirstOrDefault(source => source.clip == null);
        if (l_audioSource == null) {
            l_audioSource = gameObject.AddComponent<AudioSource>();
            l_additional = true;
        } else {
            l_additional = false;
        }

        l_audioSource.clip = clip;
        l_audioSource.volume = VolumeSounds * volume;
        l_audioSource.pitch = pitch;
        l_audioSource.transform.position = position;
        l_audioSource.loop = false;
        Data l_data = new Data("", l_audioSource, l_additional, TypesAudioSource.Sfx);
        m_sounds.Add(l_data);
        l_audioSource.Play();
        if (l_additional) {
            StartCoroutine(WaitUnilFinished(l_data));
        }

        return true;
    }

    public bool StopSound(string path, float time = 0) {
        foreach (Data data in m_sounds) {
            if (data.path != path || !data.source.isPlaying) {
                continue;
            }

            if (time > 0) {
                StartCoroutine(SoundFadeOut(data, time));
                continue;
            }

            data.source.Stop();
        }

        return true;
    }

    public bool StopSound(AudioClip clip, float time = 0) {
        if (clip == null) {
            Debug.LogWarning("Sound not found.");
            return false;
        }

        foreach (Data data in m_sounds) {
            if (data.source.clip != clip || !data.source.isPlaying) {
                continue;
            }

            if (time > 0) {
                StartCoroutine(SoundFadeOut(data, time));
                continue;
            }

            data.source.Stop();
        }

        return true;
    }

    public bool IsPlaying(string path) {
        return m_sounds.Any(data => data.path == path && data.source.isPlaying);
    }

    public bool IsPlaying(AudioClip clip) {
        return m_sounds.Any(data => data.source.clip == clip && data.source.isPlaying);
    }

    private IEnumerator SoundFadeOut(Data data, float timeDelay) {
        AudioSource l_audioSource = data.source;
        float l_stopTime;
        if (data.additional && !l_audioSource.loop && l_audioSource.time + timeDelay > l_audioSource.clip.length) {
            timeDelay = l_audioSource.clip.length - l_audioSource.time - Time.deltaTime;
            l_stopTime = Time.time + timeDelay;
        } else {
            l_stopTime = Time.time + timeDelay;
        }
        float l_volumeDelayAspect = data.type == TypesAudioSource.Music ? VolumeMusic : VolumeSounds / timeDelay;

        while (!Mathf.Approximately(l_audioSource.volume, 0f)) {
            l_audioSource.volume = l_volumeDelayAspect * (l_stopTime - Time.time);
            yield return null;
        }

        l_audioSource.Stop();

        l_audioSource.volume = data.type == TypesAudioSource.Music ? VolumeMusic : VolumeSounds;
    }

    private IEnumerator WaitUnilFinished(Data data) {
        while (!HasFinishedPlaying(data.source)) {
            yield return null;
        }
        Remove(data);
    }

    private bool HasFinishedPlaying(AudioSource source) {
        if (source.clip == null) {
            return true;
        }

        return (!source.isPlaying && source.time == 0);
    }

    private void Remove(Data audioData) {
        m_sounds.Remove(audioData);
        Destroy(audioData.source);
    }

    private void UpdateVolume() {
        int countSounds = m_sounds.Count;
        for (int i = 0; i < countSounds; i++) {
            if (m_sounds[i] != null) {
                m_sounds[i].UpdateVolume();
            }
        }
    }

    private class Data {
        public string path = string.Empty;

        public AudioSource source = null;

        public bool additional = false;

        public TypesAudioSource type = TypesAudioSource.Sfx;

        public Data(string newPath, AudioSource newsource, bool newFlag, TypesAudioSource type) {
            path = newPath;
            source = newsource;
            additional = newFlag;
            this.type = type;
        }

        public void UpdateVolume() {
            if (source != null) {
                source.volume = type == TypesAudioSource.Music ? Instance.VolumeMusic : Instance.VolumeSounds;
            }
        }
    }
}