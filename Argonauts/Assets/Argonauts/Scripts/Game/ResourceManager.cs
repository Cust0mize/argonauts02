using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : GlobalSingletonBehaviour<ResourceManager> {
	public void DoInit() {
		Resources.UnloadUnusedAssets();
	}

	public GameObject GetObjectPrefab(string name) {
        GameObject o = VResources.Load<GameObject>(string.Format("Objects/{0}", name.ToLower()));
		return o;
	}

	public Sprite GetResourceSprite(string name) {
        Sprite s = VResources.Load<Sprite>(string.Format("Resources/{0}", name.ToLower()));
		return s;
	}

	public Material GetOverlayObjectMaterial() {
        return VResources.Load<Material>(string.Format("Materials/ObjectOverlay"));
	}

    public Sprite GetTaskIconSprite(string name) {
        Sprite s = VResources.Load<Sprite>(string.Format("Tasks_Icons/{0}", name.ToLower()));
        return s;
    }

    public GameObject GetTutorial(string name) {
        GameObject o = VResources.Load<GameObject>(string.Format("Tutorials/{0}", name.ToLower()));
        return o;
    }

    public Sprite GetAwardIcon(string name) {
        Sprite s = VResources.Load<Sprite>(string.Format("Awards/{0}", name.ToLower()));
        return s;
    }

    public static AudioClip GetMusic(string name) {
        return Resources.Load<AudioClip>(string.Format("Music/{0}", name));
    }

    public static AudioClip GetSfx(string name) {
        return Resources.Load<AudioClip>(string.Format("Sfx/{0}", name));
    }
}
