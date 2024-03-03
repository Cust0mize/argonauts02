using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioWrapper : GlobalSingletonBehaviour<AudioWrapper> {
	private readonly List<string> gameMusic = new List<string> {
        "music_game_01",
        "music_game_02",
        "music_game_03",
        "music_game_04",
        "music_game_05"
	};

    private readonly List<string> listMusic = new List<string>() {
        "music_game_01",
        "music_game_02",
        "music_game_03",
        "music_game_04",
        "music_game_05",
        "music_comics",
        "music_main_menu",
        "level_finish"
    };

    private List<string> blockedMusic = new List<string>();

    private IEnumerator ieGameMusic;

    private void Start() {
        HandleScene(TransitionManager.I.GetIDCurrentScene());
    }

    private void OnLevelWasLoaded(int id) {
        HandleScene(TransitionManager.I.GetIDCurrentScene());
    }

    private void HandleScene(int id) {
        if (id.Equals(TransitionManager.ID_SCENE_MENU)) {
            PlaySimpleMusic("music_main_menu");
        } else if (id.Equals(TransitionManager.ID_SCENE_MAP)) {
            PlaySimpleMusic("music_main_menu");
        } else if (id.Equals(TransitionManager.ID_SCENE_MAP_EXTRA)) {
            PlaySimpleMusic("music_main_menu");
        } else if (id.Equals(TransitionManager.ID_SCENE_CONCEPTS)) {
            PlaySimpleMusic("music_main_menu");
        } else if (id.Equals(TransitionManager.ID_SCENE_LOADER)) {
            //do nothing
        } else if (id.Equals(TransitionManager.ID_SCENE_TROPHIES)) {
            PlaySimpleMusic("music_main_menu");
        } else if (id.Equals(TransitionManager.ID_SCENE_GAME)) {
            PlayMusicPlaylist(gameMusic);
        } else if (id.Equals(TransitionManager.ID_SCENE_COMICS1)) {
            PlaySimpleMusic("music_comics");
        } else if (id.Equals(TransitionManager.ID_SCENE_COMICS2)) {
            PlaySimpleMusic("music_comics");
        } else if (id.Equals(TransitionManager.ID_SCENE_COMICS3)) {
            PlaySimpleMusic("music_comics");
        } else if (id.Equals(TransitionManager.ID_SCENE_COMICS4)) {
            PlaySimpleMusic("music_comics");
        } else if (id.Equals(TransitionManager.ID_SCENE_COMICS5)) {
            PlaySimpleMusic("music_comics");
        } else if (id.Equals(TransitionManager.ID_SCENE_COMICS6)) {
            PlaySimpleMusic("music_comics");
        } else if (id.Equals(TransitionManager.ID_SCENE_COMICS7)) {
            PlaySimpleMusic("music_comics");
        } else {
            StopOtherMusic();
        }
    }

    public void StopOtherMusic (string exception = "", float time = 0.1F) {
        if (ieGameMusic != null) StopCoroutine(ieGameMusic);
        ieGameMusic = null;

        StopAllCoroutines();
		foreach (string k in listMusic) {
			if (!k.Equals (exception)) {
                AudioManager.Instance.StopSound (ResourceManager.GetMusic (k), time);
			}
		}
	}

    public void StopOtherMusic(List<string> exceptions, float time = 0.1F) {
        if (ieGameMusic != null) StopCoroutine(ieGameMusic);
        ieGameMusic = null;

        StopAllCoroutines();
        foreach (string k in listMusic) {
            if (!exceptions.Contains(k)) {
                AudioManager.Instance.StopSound(ResourceManager.GetMusic(k), time);
            }
        }
    }

    public void PlaySimpleMusic (string musicName, bool loop = true) {
		StopOtherMusic (musicName);
        if (!AudioManager.Instance.IsPlaying (ResourceManager.GetMusic (musicName))) {
            AudioManager.Instance.PlaySound (ResourceManager.GetMusic (musicName), AudioManager.TypesAudioSource.Music, loop);
		}
	}

    public void PlayMusicPlaylist(List<string> playlist) {
        if (ieGameMusic != null) return;

        StopOtherMusic(playlist);

        ieGameMusic = IEPlayMusicPlaylist();
        StartCoroutine(ieGameMusic);

    }

    public void PlayLevelMusic() {
        PlayMusicPlaylist(gameMusic);
    }

    public void PlaySfx(string sfxName) {
        AudioManager.Instance.PlaySound(ResourceManager.GetSfx(sfxName), AudioManager.TypesAudioSource.Sfx, false);
    }

    public void PlaySfx(string sfxName, float volume, float pitch, Vector3 position) {
        AudioManager.Instance.PlaySound(ResourceManager.GetSfx(sfxName), volume, pitch, position);
    }

    private string GetNextMusic(string lastMusicPlayed) {
        if (blockedMusic.Count >= gameMusic.Count) blockedMusic.Clear();
        return gameMusic.Where(x => !blockedMusic.Exists(y => y.Equals(x)) && !x.Equals(lastMusicPlayed)).Random();
    }

    private IEnumerator IEPlayMusicPlaylist() {
        string lastMusicPlayed = "";
        AudioClip lastClipPlayed = null;

        while (true) {
            lastMusicPlayed = GetNextMusic(lastMusicPlayed);
            blockedMusic.Add(lastMusicPlayed);
            lastClipPlayed = ResourceManager.GetMusic(lastMusicPlayed);

            AudioManager.Instance.PlaySound(lastClipPlayed, AudioManager.TypesAudioSource.Music, false);
            yield return new WaitForSeconds(0.1f);

            while (AudioManager.I.IsPlaying(lastClipPlayed) || !Application.isFocused) yield return null;
        }

        ieGameMusic = null;
    }

    public IEnumerator WaitMusicPlaying(AudioClip clip) {
        while (AudioManager.I.IsPlaying(clip) || !Application.isFocused) yield return null;
    }
}
