using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExportController : MonoBehaviour {
    [SerializeField] Camera exportCamera;

    public Texture2D CaptureScreen() {
        RenderTexture RT = new RenderTexture(1920, 1080, 0);
        RenderTexture CurRT = RenderTexture.active;
        RenderTexture.active = RT;
        exportCamera.targetTexture = RT;
        exportCamera.Render();
        Texture2D TD = new Texture2D(RT.width, RT.height);
        TD.ReadPixels(new Rect(0, 0, TD.width, TD.height), 0, 0);
        TD.Apply();
        RenderTexture.active = CurRT;
        exportCamera.targetTexture = null;
        return TD;
    }
}
