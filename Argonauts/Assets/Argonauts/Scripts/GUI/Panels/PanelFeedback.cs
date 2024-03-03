using HGL;
using TMPro;
using UnityEngine;

public class PanelFeedback : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;

    public void OnButtonSendClicked()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            string emailTo = "bussinestour.report@gmail.com";
            string subject = "Argonauts Agency: Glove of Midas - Feedback";
            string body = inputField.text;
            
            EmailUtility.Send(emailTo, subject, body);
        }
        HGL_WindowManager.I.CloseWindow(null, null, nameof(PanelFeedback), false);
    }

    public void OnButtonCloseClicked()
    {
        HGL_WindowManager.I.CloseWindow(null, null, nameof(PanelFeedback), false);
    }
}
