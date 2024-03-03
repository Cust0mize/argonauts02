using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : GlobalSingletonBehaviour<TransitionManager> {
	public const int ID_SCENE_MENU = 1;
	public const int ID_SCENE_MAP = 2;
	public const int ID_SCENE_GAME = 3;
	public const int ID_SCENE_LOADER = 4;
    public const int ID_SCENE_TROPHIES = 5;

    public const int ID_SCENE_COMICS1 = 6;
    public const int ID_SCENE_COMICS2 = 7;
    public const int ID_SCENE_COMICS3 = 8;
    public const int ID_SCENE_COMICS4 = 9;
    public const int ID_SCENE_COMICS5 = 10;
    public const int ID_SCENE_COMICS6 = 11;
    public const int ID_SCENE_COMICS7 = 12;
    public const int ID_SCENE_COMICS8 = 13;

    public const int ID_SCENE_CONCEPTS = 14;
    public const int ID_SCENE_SOUNDTRACKS = 15;

    public const int ID_SCENE_MAP_EXTRA = 16;

	public int TargetLevel = 1;
	public int TargetScene = 1;
    public int SceneToMapExtraID = ID_SCENE_MENU;

    public bool MustOpenDemoWindow { get; set; }

    public Action ComicsCallback = null;

	public void LoadMap() {
        LoadScene(ID_SCENE_MAP);
	}

    public void LoadMapExtra()
    {
        LoadScene(ID_SCENE_MAP_EXTRA);
    }

    public void LoadLevel(int level, bool comics = true) {

        if (comics) {
            ComicsCallback = () => { LoadLevel(level, false); };

            if (level.Equals(1) && !UserData.I.ComicsIsShown(1)) {
                LoadComics(1);
                return;
            } else if (level.Equals(51) && !UserData.I.ComicsIsShown(7)) {
                LoadComics(7);
                return;
            }
        }

        ComicsCallback = null;
        TargetLevel = level;
        LoadScene(ID_SCENE_GAME);
    }

	public void LoadMenu() {
		LoadScene(ID_SCENE_MENU);
	}

    public void LoadTrophies() {
        LoadScene(ID_SCENE_TROPHIES);
    }

    public void LoadConcepts() {
        LoadScene(ID_SCENE_CONCEPTS);
    }

    public void LoadSoundtracks() {
        LoadScene(ID_SCENE_SOUNDTRACKS);
    }

    public void LoadComics(int number) {
        switch (number) {
            case 1: LoadScene(ID_SCENE_COMICS1); break;
            case 2: LoadScene(ID_SCENE_COMICS2); break;
            case 3: LoadScene(ID_SCENE_COMICS3); break;
            case 4: LoadScene(ID_SCENE_COMICS4); break;
            case 5: LoadScene(ID_SCENE_COMICS5); break;
            case 6: LoadScene(ID_SCENE_COMICS6); break;
            case 7: LoadScene(ProjectSettings.I.isAllLevelsEnabled ? ID_SCENE_COMICS7 : ID_SCENE_MAP_EXTRA); break;
            case 8: LoadScene(ProjectSettings.I.isAllLevelsEnabled ? ID_SCENE_COMICS8 : ID_SCENE_MAP_EXTRA); break;
        }
    }

    public int GetComicsNumber(int levelNumber) {
        switch (levelNumber) {
            case 10: return 2;
            case 20: return 3;
            case 30: return 4;
            case 40: return 5;
            case 50: return 6;
            case 60: return 8;
            default: return 0;
        }
    }

    public bool ComicsExits(int levelNumber) {
        switch (levelNumber) {
            case 10: return true;
            case 20: return true;
            case 30: return true;
            case 40: return true;
            case 50: return true;
            case 60: return true;
            default: return false;
        }
    }

	public void RestartLevel() {
		LoadLevel(TargetLevel);
	}

	public void RestartScene() {
		LoadScene(SceneManager.GetActiveScene().buildIndex);
	}

	public void LoadScene(int id) {
		if (FadeManager.Inited) {
			StopAllCoroutines();
			StartCoroutine(IELoadScene(id));
		} else {
			TargetScene = id;
			SceneManager.LoadScene(ID_SCENE_LOADER, LoadSceneMode.Single);
            AwardHandler.I.SaveAwards();
		}
	}

	public IEnumerator IELoadScene(int id) {
		yield return FadeManager.I.FadeIn(2f);
        AwardHandler.I.SaveAwards();
		TargetScene = id;
		SceneManager.LoadScene(ID_SCENE_LOADER, LoadSceneMode.Single);
	}

	public int GetIDCurrentScene() {
		return SceneManager.GetActiveScene().buildIndex;
	}
}
