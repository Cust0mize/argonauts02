using UnityEngine;
using System.Collections;
using HGL;
using System;

namespace HGL {
	public class HGL_Panel : MonoBehaviour {

		[SerializeField] protected string title;

		public HGL_UGUIWindow windowPanel;

		void Awake () {
			if (string.IsNullOrEmpty (title) && GetComponent<HGL_UGUIWindow> ()) {
				title = GetComponent<HGL_UGUIWindow> ().NameWindow;
			}
		}

		public void OpenPanel (Action callbackStart = null, Action callbackFinish = null) {
			if (windowPanel != null)
				windowPanel.Open (false, false, false, callbackStart, callbackFinish);
		}

		public void ClosePanel (bool invert = false, Action callbackStart = null, Action callbackFinish = null) {
			if (windowPanel != null)
				windowPanel.Close (false, true, callbackStart, callbackFinish);
		}

		public void OnButtonCancelClick () {
			ClosePanel ();
		}

		public void OpenAnotherPanel (string panelTitle) {
			HGL_WindowManager.I.OpenWindow (null, null, panelTitle, false, false);
		}
	}
}
