using System.Collections;
using System.Collections.Generic;
using System.Text;
using HGL;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelStart : MonoBehaviour {
    [SerializeField] TextMeshProUGUI levelText;

    [SerializeField] TaskBlock taskBlock1;
    [SerializeField] TaskBlock taskBlock2;
    [SerializeField] TaskBlock taskBlock3;

    private void OnEnable() {
        GameManager.I.IsPause = true;
    }

    private void OnDisable() {
        GameManager.I.IsPause = false;
    }

    public void Init(int levelNumber, Task task1, Task task2, Task task3) {
        levelText.text = string.Format(LocalizationManager.GetLocalizedString("gui_lbl_level_number"), levelNumber);

        ApplyTask(taskBlock1, task1);
        ApplyTask(taskBlock2, task2);
        ApplyTask(taskBlock3, task3);
    }

    private void ApplyTask(TaskBlock taskUI, Task task) {
        if(task == null || string.IsNullOrEmpty(task.Key)) {
            taskUI.gameObject.SetActive(false);
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.Append("task");
        string[] keyParts = task.Key.Split('_');
        for (int i = 1; i < keyParts.Length; i++) {
            sb.Append("_");
            sb.Append(keyParts[i]);
        }

        Task targetTask = LevelTaskController.I.TargetTasks.Find(tt => tt.Key.Equals(task.Key));

        taskUI.SetText(string.Format(LocalizationManager.GetLocalizedString(sb.ToString()) +" <color=#011960FF>({0})</color>", targetTask.Value));
        taskUI.SetIcon(ResourceManager.I.GetTaskIconSprite(sb.ToString()));
    }

    public void Ok(){
        HGL_WindowManager.I.CloseWindow(null, GameManager.I.OnPanelStartOkClicked, "PanelStart", false, false);
    }
}
