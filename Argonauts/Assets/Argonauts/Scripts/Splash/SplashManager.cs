using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using WyrmTale;
using UnityEngine.Networking;

//--------------------------------------------------------
// utils classes
public class Splash {
	public string pic { get; set; }

	public int time { get; set; }

	public int clickable { get; set; }
}

public class SplashesList {
	public List<Splash> splashes { get; set; }

	public void Init (string initStr) {
		JSON json = new JSON ();
		json.serialized = initStr;

		JSON[] jsonArray = json.ToArray<JSON> ("splashes");
		Splash splashItem = null;

		splashes = new List<Splash> ();

		for (int i = 0; i < jsonArray.Length; ++i) {
			splashItem = new Splash ();
			splashItem.pic = jsonArray [i].ToString ("pic");
			splashItem.time = jsonArray [i].ToInt ("time");
			splashItem.clickable = jsonArray [i].ToInt ("clickable");

			splashes.Add (splashItem);
		}
	}
}


//--------------------------------------------------------
// splashes manager
public class SplashManager : MonoBehaviour {

	public RawImage imageRender;

	private SplashesList splashesList = null;
	private int index = 0;
	private string filePath = "";
	private string stringData = "";
	private Color color = Color.white;
	private bool isCanClick = false;

	IEnumerator Start () {
		if (!ProjectSettings.I.SpashEnabled) {
			SceneManager.LoadScene (TransitionManager.ID_SCENE_MENU);
			yield break;
		}

		filePath = System.IO.Path.Combine (Application.streamingAssetsPath, "Splash/splashes.txt");
		stringData = "";

		//if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
		//if (!filePath.Contains ("://"))
		//filePath = "file://" + filePath;

#if !UNITY_ANDROID || UNITY_EDITOR
		filePath = "file://" + filePath;
#endif

		var req = UnityWebRequest.Get(filePath);
		yield return req.SendWebRequest();
		stringData = req.downloadHandler.text;

		//stringData = req.result;
		//} else {
		//	stringData = System.IO.File.ReadAllText (filePath);
		//}

		if (!string.IsNullOrEmpty (stringData)) {
			splashesList = new SplashesList ();
			splashesList.Init (stringData);
		}

		while (!ProjectSettings.I.isInit)
			yield return 0;

		while (!AdditionalSplashManager.I.isFinished)
			yield return 0;

		AdditionalSplashManager.I.gameObject.SetActive (false);

		index = 0;
		StartCoroutine (IE_ShowImage ());
	}


	IEnumerator IE_ShowImage () {
		isCanClick = false;
		color.a = 0.0f;
		imageRender.color = color;

		// exit
		if (splashesList == null || splashesList.splashes.Count <= 0 || index >= splashesList.splashes.Count) {
			SceneManager.LoadScene (TransitionManager.ID_SCENE_MENU);
			yield break;
		}

		// load picture

		filePath = System.IO.Path.Combine (Application.streamingAssetsPath, "Splash/" + splashesList.splashes [index].pic);
		stringData = "";

		if (!filePath.Contains ("://"))
			filePath = "file://" + filePath;

		WWW www = new WWW (filePath);
		yield return www;

		if (string.IsNullOrEmpty (www.error)) {
			imageRender.texture = www.texture;
			color.r = 1.0f;
			color.g = 1.0f;
			color.b = 1.0f;
		} else {
			imageRender.texture = null;
			color.r = 0.0f;
			color.g = 0.0f;
			color.b = 0.0f;
		}

		if (imageRender.texture == null) {
			SceneManager.LoadScene (TransitionManager.ID_SCENE_MENU);
			yield break;
		}

		// show
		isCanClick = true;

		while (color.a < 1.0f) {
			color.a += 1.0f * Time.deltaTime;
			color.a = Mathf.Min (color.a, 1.0f);
			imageRender.color = color;
			yield return 0;
		}

		// stand
		yield return new WaitForSeconds (splashesList.splashes [index].time);

		// hide
		StartCoroutine (IE_HideImage ());
	}


	IEnumerator IE_HideImage () {
		isCanClick = false;

		// hide
		while (color.a > 0.0f) {
			color.a -= 1.0f * Time.deltaTime;
			color.a = Mathf.Max (color.a, 0.0f);
			imageRender.color = color;
			yield return 0;
		}

		++index;
		StartCoroutine (IE_ShowImage ());
	}


	public void OnPointer_Down () {
		if (!AdditionalSplashManager.I.isFinished) {
			AdditionalSplashManager.I.OnPointer_Down ();
		}

		if (index >= splashesList.splashes.Count)
			return;

		if (isCanClick && splashesList.splashes [index].clickable > 0) {
			isCanClick = false;
			StopAllCoroutines ();
			StartCoroutine (IE_HideImage ());
		}
	}
}
