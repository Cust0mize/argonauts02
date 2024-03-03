using UnityEngine;
using UnityEngine.Networking;

public static class EmailUtility
{
    public static void Send(string email, string subject, string body)
    {
        subject = EscapeUrl(subject);
        body = EscapeUrl(body);
        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }
    
    private static string EscapeUrl (string url)
    {
        return UnityWebRequest.EscapeURL(url).Replace("+","%20");
    }
}
