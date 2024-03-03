using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpriteController : MonoBehaviour {
	public Sprite PrefabCellSprite;

	public Dictionary<string , Sprite> Sprites = new Dictionary<string , Sprite>();

	public Sprite GetSprite (string path) {
		if (!Sprites.ContainsKey(path)) return null;
		return Sprites[path];
	}

	public void AddSprite (string path , Sprite sprite) {
		if (!Sprites.ContainsKey(path)) {
			Sprites.Add(path , sprite);
		}
	}

	public void AddSprite (Tile tile) {
		if (!Sprites.ContainsKey(tile.Path)) {
			Sprites.Add(tile.Path , tile.Sprite);
		}
	}

	public void AddSprites (Dictionary<string , Sprite> sprites) {
		foreach (string s in sprites.Keys) {
			if (Sprites.ContainsKey(s)) {
				Sprites[s] = sprites[s];
				continue;
			}
			Sprites.Add(s , sprites[s]);
		}
	}

	public void AddSprites (string[] paths) {
		StartCoroutine(LoadSprites(paths));
	}

	public void Init (Dictionary<string , Sprite> sprites) {
		Sprites = sprites;
	}

	public IEnumerator LoadSprites (string[] paths) {
		for (int i = 0;i < paths.Length;i++) {
			yield return LoadSprite(paths[i]);
		}
	}

	public IEnumerator LoadSprite (string path , bool autoPivot = true) {
		if (string.IsNullOrEmpty(path)) yield break;
		if (Sprites.ContainsKey(path)) yield break;

		WWW www = new WWW("file://" + System.IO.Path.GetFullPath(path));

		while (!www.isDone) {
			yield return null;
		}

		Integer2 pivotPixels = (ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel).PivotOffset;
		Vector2 pivot = new Vector2(0.5F , 0.5F);

		if (!string.IsNullOrEmpty(www.error)) {
			Debug.Log(www.error);
			yield break;
		} else {
			if (autoPivot) {
				pivot = new Vector2((float)pivotPixels.X / (float)www.texture.width , (float)pivotPixels.Y / (float)www.texture.height);
			}
			OnSpriteLoaded(Sprite.Create(www.texture , new Rect(0 , 0 , www.texture.width , www.texture.height) , pivot , 1) , path);
		}

		yield return 0;
	}

	public IEnumerator LoadSprite (string path , Action<Sprite> callback , Vector2 pivot) {
		if (string.IsNullOrEmpty(path)) yield break;
		if (Sprites.ContainsKey(path)) {
			if (callback != null) {
				callback(Sprites[path]);
			}
			yield break;
		}

		WWW www = new WWW("file://" + path);

		while (!www.isDone) {
			yield return null;
		}

		Integer2 pivotPixels = (ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel).PivotOffset;

		if (!string.IsNullOrEmpty(www.error)) {
			Debug.Log(www.error);
			yield break;
		} else {
			if (callback != null) {
				callback(Sprite.Create(www.texture , new Rect(0 , 0 , www.texture.width , www.texture.height) , pivot , 1));
			}
		}

		yield return 0;
	}

	public IEnumerator LoadSprite (string path , Action<Sprite> callback , bool autoPivot = false) {
		if (string.IsNullOrEmpty(path)) yield break;
		if (Sprites.ContainsKey(path)) {
			if (callback != null) {
				callback(Sprites[path]);
			}
			yield break;
		}

		WWW www = new WWW("file://" + System.IO.Path.GetFullPath(path));

		while (!www.isDone) {
			yield return null;
		}

		Integer2 pivotPixels = (ModuleContainer.I.ViewsController.GridSettingsView.GetModel() as GridSettingsModel).PivotOffset;
		Vector2 pivot = new Vector2(0.5F , 0.5F);

		if (!string.IsNullOrEmpty(www.error)) {
			Debug.Log(www.error);
			yield break;
		} else {
			if (autoPivot) {
				pivot = new Vector2((float)pivotPixels.X / (float)www.texture.width , (float)pivotPixels.Y / (float)www.texture.height);
			}
			if (callback != null) {
				callback(Sprite.Create(www.texture , new Rect(0 , 0 , www.texture.width , www.texture.height) , pivot , 1));
			}
		}

		yield return 0;
	}

	void OnSpriteLoaded (Sprite s , string path) {
		if (Sprites.ContainsKey(path)) {
			Sprites[path] = s;
			return;
		}
		Sprites.Add(path , s);
	}
}
