using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;


//--------------------------------------------------------
// splashes manager
public class AdditionalSplashManager : LocalSingletonBehaviour<AdditionalSplashManager> {

	public RawImage imageRender;
	public bool isFinished = false;

	private SplashesList splashesList = null;
	private int index = 0;
	private string filePath = "";
	private Color color = Color.white;
	private bool isCanClick = false;

	IEnumerator Start () {

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			isFinished = true;
			yield break;
		}

		if (!ProjectSettings.I.isMPC_build && !ProjectSettings.I.isGT_build && !ProjectSettings.I.isFGD_build)
		{
			isFinished = true;
			yield break;
		}

		string filePath = "";

		// folder ../fsdata
		if (ProjectSettings.I.isMPC_build)
		{
			splashesList = new SplashesList();
			splashesList.splashes = new List<Splash>();

			// splash2.jpg
			filePath = "file://" + Application.dataPath + "/../fsdata/" + "splash2.jpg";
			if (Application.platform == RuntimePlatform.OSXPlayer)
				filePath = "file://" + Application.dataPath + "/../../fsdata/" + "splash2.jpg";

			WWW www = new WWW(filePath);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && www.texture != null)
			{
				Splash splash = new Splash();
				splash.clickable = 1;
				splash.time = 4;
				splash.pic = filePath;
				splashesList.splashes.Add(splash);
			}

			// splash1.jpg
			filePath = "file://" + Application.dataPath + "/../fsdata/" + "splash1.jpg";
			if (Application.platform == RuntimePlatform.OSXPlayer)
				filePath = "file://" + Application.dataPath + "/../../fsdata/" + "splash1.jpg";
			
			www = new WWW(filePath);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && www.texture != null)
			{
				Splash splash = new Splash();
				splash.clickable = 1;
				splash.time = 4;
				splash.pic = filePath;
				splashesList.splashes.Add(splash);
			}
		}

		// folder ../logo (FGP)
		if (ProjectSettings.I.isFGP_build)
		{
			splashesList = new SplashesList();
			splashesList.splashes = new List<Splash>();

			filePath = "file://" + Application.dataPath + "/../logo/" + "FreeGamePick.jpg";
			if (Application.platform == RuntimePlatform.OSXPlayer)
				filePath = "file://" + Application.dataPath + "/../../logo/" + "FreeGamePick.jpg";

			WWW www = new WWW(filePath);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && www.texture != null)
			{
				Splash splash = new Splash();
				splash.clickable = 1;
				splash.time = 4;
				splash.pic = filePath;
				splashesList.splashes.Add(splash);
			}
		}

		// folder ../logo (MRG)
		if (ProjectSettings.I.isMRG_build)
		{
			splashesList = new SplashesList();
			splashesList.splashes = new List<Splash>();

			filePath = "file://" + Application.dataPath + "/../logo/" + "mrg_logo.jpg";
			if (Application.platform == RuntimePlatform.OSXPlayer)
				filePath = "file://" + Application.dataPath + "/../../logo/" + "mrg_logo.jpg";

			WWW www = new WWW(filePath);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && www.texture != null)
			{
				Splash splash = new Splash();
				splash.clickable = 1;
				splash.time = 4;
				splash.pic = filePath;
				splashesList.splashes.Add(splash);
			}
		}

		// folder ../logo (GT)
		if (ProjectSettings.I.isGT_build)
		{
			splashesList = new SplashesList();
			splashesList.splashes = new List<Splash>();

			filePath = "file://" + Application.dataPath + "/../logo/" + "gt1024.jpg";
			if (Application.platform == RuntimePlatform.OSXPlayer)
				filePath = "file://" + Application.dataPath + "/../../logo/" + "gt1024.jpg";
			
			WWW www = new WWW(filePath);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && www.texture != null)
			{
				Splash splash = new Splash();
				splash.clickable = 1;
				splash.time = 4;
				splash.pic = filePath;
				splashesList.splashes.Add(splash);
			}
		}

		// folder ../logo (FGD)
		if (ProjectSettings.I.isFGD_build)
		{
			splashesList = new SplashesList();
			splashesList.splashes = new List<Splash>();
			
			// splash2.jpg
			filePath = "file://" + Application.dataPath + "/../logo/" + "fgd1024.jpg";
			if (Application.platform == RuntimePlatform.OSXPlayer)
				filePath = "file://" + Application.dataPath + "/../../logo/" + "fgd1024.jpg";
			
			WWW www = new WWW(filePath);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && www.texture != null)
			{
				Splash splash = new Splash();
				splash.clickable = 1;
				splash.time = 4;
				splash.pic = filePath;
				splashesList.splashes.Add(splash);
			}
		}

		while(!ProjectSettings.I.isInit)
			yield return 0;

		if (splashesList == null || splashesList.splashes.Count == 0)
		{
			isFinished = true;
			yield break;
		}

		index = 0;
		StartCoroutine(IE_ShowImage());
	}


	IEnumerator IE_ShowImage()
	{
		isCanClick = false;
		color.a = 0.0f;
		imageRender.color = color;

		// exit
		if (splashesList == null || splashesList.splashes.Count <= 0 || index >= splashesList.splashes.Count )
		{
			isFinished = true;
			yield break;
		}

		// load picture
		filePath = splashesList.splashes[index].pic;

		WWW www = new WWW(filePath);
		yield return www;

		if (string.IsNullOrEmpty(www.error)) {
			imageRender.texture = www.texture;
			color.r = 1.0f;
			color.g = 1.0f;
			color.b = 1.0f;
		}
		else {
			imageRender.texture = null;
			color.r = 0.0f;
			color.g = 0.0f;
			color.b = 0.0f;
		}

		if (imageRender.texture == null)
		{
			isFinished = true;
			yield break;
		}

		// show
		isCanClick = true;

		while(color.a < 1.0f)
		{
			color.a += 1.0f * Time.deltaTime;
			color.a = Mathf.Min(color.a, 1.0f);
			imageRender.color = color;
			yield return 0;
		}

		// stand
		yield return new WaitForSeconds(splashesList.splashes[index].time);

		// hide
		StartCoroutine(IE_HideImage());
	}


	IEnumerator IE_HideImage()
	{
		isCanClick = false;

		// hide
		while(color.a > 0.0f)
		{
			color.a -= 1.0f * Time.deltaTime;
			color.a = Mathf.Max(color.a, 0.0f);
			imageRender.color = color;
			yield return 0;
		}

		++index;
		StartCoroutine(IE_ShowImage());
	}


	public void OnPointer_Down()
	{
		if (index >= splashesList.splashes.Count)
			return;

		if (isCanClick && splashesList.splashes[index].clickable > 0)
		{
			isCanClick = false;
			StopAllCoroutines();
			StartCoroutine(IE_HideImage());
		}
	}
}
