using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Компонент, назначающий имя окна.
/// Работает исключительно на Windows.
/// </summary>
public class WindowsSetTitle : MonoBehaviour {
	#region External

	#if UNITY_STANDALONE_WIN
	[DllImport ("user32.dll", EntryPoint = "SetWindowTextW", CharSet = CharSet.Unicode)]
	public static extern bool SetWindowTextW (IntPtr hwnd, string lpString);

	[DllImport ("user32.dll", EntryPoint = "FindWindow")]
	public static extern IntPtr FindWindow (string className, string windowName);
	#endif

	#endregion

	#region UnityCalls

	public static void SetWindowTitle (string title) {
    #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        IntPtr windowPtr = FindWindow(null, Application.productName);
        SetWindowTextW(windowPtr, title);
    #endif
	}

	IEnumerator Start () {
		//todo: change window title
		yield return null;
	}

	#endregion
}