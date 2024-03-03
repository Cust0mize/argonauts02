using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Threading.Tasks;

public static class StreamingAssetsLoader
{
    public static async Task<string> GetText(string path) {
        string fullPath = string.Format("{0}/{1}", Application.streamingAssetsPath, path);
#if !UNITY_ANDROID || UNITY_EDITOR
        fullPath = "file://" + fullPath;
#endif

        UnityWebRequest request = UnityWebRequest.Get(fullPath);
        await request.SendWebRequest();

        while (!request.isDone) {
            await System.Threading.Tasks.Task.Yield();
        }

        if (request.isNetworkError) {
            //Debug.Log(fullPath + ", error: " + request.error);
        }
        else {
            //Debug.Log(fullPath + ", loaded text file.");
        }

        return request.downloadHandler.text;
    }

    public static async Task<Texture2D> GetTexture2D(string path) {
        string fullPath = string.Format("{0}/{1}", Application.streamingAssetsPath, path);
#if !UNITY_ANDROID || UNITY_EDITOR
        fullPath = "file://" + fullPath;
#endif

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(fullPath);
        await request.SendWebRequest();

        while (!request.isDone) {
            await System.Threading.Tasks.Task.Yield();
        }

        if (request.isNetworkError) {
            Debug.Log(fullPath + ", error: " + request.error);
        }
        else {
            Debug.Log(fullPath + ", loaded texture file.");
        }

        return DownloadHandlerTexture.GetContent(request);
    }

    public static async Task<Sprite> GetSprite(string path) {
        Texture2D texture2d = await GetTexture2D(path);
        if (texture2d != null) {
            return Sprite.Create(texture2d, new Rect(0, 0, texture2d.width, texture2d.height), new Vector2(0.5F, 0.5F), 1F);
        }
        return null;
    }
}
