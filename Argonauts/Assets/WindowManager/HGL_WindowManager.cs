﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace HGL {
	public class HGL_WindowManager : LocalSingletonBehaviour<HGL_WindowManager> {
		[SerializeField] List<HGL_BaseWindow> Windows = new List<HGL_BaseWindow> ();

		public HGL_BaseWindow GetWindow (string name) {
			int countWindows = Windows.Count;
			for (int i = 0; i < countWindows; i++) {
				if (Windows [i].NameWindow.Equals (name)) {
					return Windows [i];
				}
			}
			return null;
		}

		public void OpenWindow (Action onStartOpen, Action onFinishOpen, string name, bool force, bool modal, bool invertingClip = false) {
			int countWindows = Windows.Count;
			for (int i = 0; i < countWindows; i++) {
				if (Windows [i].NameWindow.Equals (name)) {
					if (CanOpen (Windows [i].State)) {
						Windows [i].gameObject.SetActive (true);

						if (Windows [i].SetAsLastSibling)
							Windows [i].transform.SetAsLastSibling ();
						
						Windows [i].Open (force, modal, invertingClip, onStartOpen, onFinishOpen);
					} 
				}
			}
		}

		public void CloseWindow (Action onStartClose, Action onFinishClose, string name, bool force, bool invertingClip = false) {
			int countWindows = Windows.Count;
			for (int i = 0; i < countWindows; i++) {
				if (Windows [i].NameWindow.Equals (name)) {
					if (CanClose (Windows [i].State)) {
						Windows [i].gameObject.SetActive (true);
						Windows [i].Close (force, invertingClip, onStartClose, onFinishClose);
					}
				}
			}
		}

		public bool IsOpen (string name) {
			int countWindows = Windows.Count;
			for (int i = 0; i < countWindows; i++) {
				if (Windows [i].NameWindow.Equals (name)) {
					return Windows [i].State == HGL_WindowStates.Open ? true : false;
				}
			}
			return false;
		}

		public bool IsOpening (string name) {
			int countWindows = Windows.Count;
			for (int i = 0; i < countWindows; i++) {
				if (Windows [i].NameWindow.Equals (name)) {
					return Windows [i].State == HGL_WindowStates.Opening ? true : false;
				}
			}
			return false;
		}

		public bool IsClosing (string name) {
			int countWindows = Windows.Count;
			for (int i = 0; i < countWindows; i++) {
				if (Windows [i].NameWindow.Equals (name)) {
					return Windows [i].State == HGL_WindowStates.Closing ? true : false;
				}
			}
			return false;
		}

		public bool IsOpenOrOpening (string name) {
			int countWindows = Windows.Count;
			for (int i = 0; i < countWindows; i++) {
				if (Windows [i].NameWindow.Equals (name)) {
					return Windows [i].State == HGL_WindowStates.Open || Windows [i].State == HGL_WindowStates.Opening ? true : false;
				}
			}
			return false;
		}

		public int CountOpenWindows () {
			int countWindows = Windows.Count;
			int result = 0;
			for (int i = 0; i < countWindows; i++) {
				if (Windows [i].State == HGL_WindowStates.Opening || Windows [i].State == HGL_WindowStates.Open) {
					result++;
				}
			}
			return result;
		}

		bool CanOpen (HGL_WindowStates state) {
			return state == HGL_WindowStates.Close || state == HGL_WindowStates.Closing || state == HGL_WindowStates.Opening ? true : false;
		}

		bool CanClose (HGL_WindowStates state) {
			return state == HGL_WindowStates.Open || state == HGL_WindowStates.Opening || state == HGL_WindowStates.Closing ? true : false;
		}
	}
}
