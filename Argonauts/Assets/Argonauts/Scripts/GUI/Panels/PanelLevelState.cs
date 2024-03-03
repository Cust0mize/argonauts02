using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelLevelState : MonoBehaviour {

    [SerializeField] private RectTransform taskPanel;
    [SerializeField] private float durationShrinkPanel = 1.5f;

    [SerializeField] private TextMeshProUGUI[] tasksText;
    [SerializeField] private Image timeStar1;
    [SerializeField] private Image timeStar2;
    [SerializeField] private Image timeStar3;
    [SerializeField] private Image timeBar;

    [SerializeField] private Image crossoutLine;
    [SerializeField] private GameObject crossoutParticles;
    [SerializeField] Vector2 crossoutParticleBounds;
    [SerializeField] private float durationDoneEffect = 1.5f;

    [SerializeField] private float panelTaskSpeed;
    [SerializeField] private TextMeshProUGUI levelText;

    private List<Task> strikeouts = new List<Task>();

    private bool isShrink = false;

    private int countEffectDoneStarted;
    private bool isDoneTaskEffect {
        get {
            return countEffectDoneStarted > 0;
        }
    }

    private IEnumerator shirnkCoroutine;

    public void InitStars (Level level) {
        float positionFactor1 = 1F - (level.Time1 / level.TotalTime);
        float positionFactor2 = 1F - (level.Time2 / level.TotalTime);
        float positionFactor3 = 1F - (level.Time3 / level.TotalTime);
        float pos1 = positionFactor1 * timeBar.rectTransform.rect.width;
        float pos2 = positionFactor2 * timeBar.rectTransform.rect.width;
        float pos3 = positionFactor3 * timeBar.rectTransform.rect.width;

        timeStar1.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos1, 0.0f);
        timeStar2.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos2, 0.0f);
        timeStar3.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos3, 0.0f);
    }

    public void UpdateTimeBar (float curentTime, float levelTime) {
        timeBar.fillAmount = 1 - curentTime / levelTime;
    }

    public void TaskUpdated () {
        string text = string.Empty;
        string trueKey = string.Empty;

        Task targetTask;

        int i = 0;

        foreach (Task t in LevelTaskController.I.Tasks) {
            text = string.Empty;
            trueKey = string.Empty;

            targetTask = LevelTaskController.I.TargetTasks.Find(tt => tt.Key.Equals(t.Key));

            StringBuilder sb = new StringBuilder();
            string[] keyParts = t.Key.Split('_');
            for (int s = 1; s < keyParts.Length; s++) {
                sb.Append("_");
                sb.Append(keyParts[s]);
            }

            switch (t.Key.Substring(0, t.Key.Length - 2)) {
                case "act_take_food": trueKey = string.Format("task{0}", sb.ToString().Substring(0, sb.Length - 2)); break;
                case "act_take_wood": trueKey = string.Format("task{0}", sb.ToString().Substring(0, sb.Length - 2)); break;
                case "act_take_stone": trueKey = string.Format("task{0}", sb.ToString().Substring(0, sb.Length - 2)); break;
                case "act_take_gold": trueKey = string.Format("task{0}", sb.ToString().Substring(0, sb.Length - 2)); break;
                case "act_take_tooth": trueKey = string.Format("task{0}", sb.ToString().Substring(0, sb.Length - 2)); break;
                default: trueKey = string.Format("task{0}", sb); break;
            }

            text += string.Format(LocalizationManager.GetLocalizedString(trueKey) + " <color=#011960FF>({0}/{1})</color>", t.Value, targetTask.Value);

            tasksText[i].text = text;

            if (t.Value >= targetTask.Value) {
                if (!strikeouts.Contains(t)) {
                    strikeouts.Add(t);
                    StartDoneTaskEffect(tasksText[i]);
                }
            }

            i++;
        }
    }

    private void Awake () {
        StartCoroutine(IEStartTaskPanelSizeChange());
    }

    private void Start () {
        int numberLevel = TransitionManager.I.TargetLevel;
        levelText.GetComponent<TextMeshProUGUI>().text = string.Format(LocalizationManager.GetLocalizedString("gui_lbl_level_number"), numberLevel);
    }

    private void StartDoneTaskEffect (TextMeshProUGUI task) {
        StartCoroutine(IEStartDoneTaskEffect(task));
    }

    private IEnumerator IEStartTaskPanelSizeChange () {
        while (!LevelTaskController.I.TasksInited) yield return null;

        foreach (TextMeshProUGUI t in tasksText) {
            t.gameObject.SetActive(false);
        }

        for (int i = 0; i < LevelTaskController.I.Tasks.Count;i++) {
            tasksText[i].gameObject.SetActive(true);
        }

        float height = 30F + LevelTaskController.I.Tasks.Count * 50F + ((3 - LevelTaskController.I.Tasks.Count) * 10F);
        taskPanel.sizeDelta = new Vector2(taskPanel.sizeDelta.x, height);
    }

    private IEnumerator IEStartDoneTaskEffect (TextMeshProUGUI task) {
        while (countEffectDoneStarted > 0) {
            yield return null;
        }
        while (isShrink) {
            yield return null;
        }

        countEffectDoneStarted++;

        float durationEffect = 1.5f;

        StartCoroutine(IEStartCrossoutTask(task, durationEffect));
        StartCoroutine(IEStartCrossoutParticlesTask(task, durationEffect));

        yield return new WaitForSeconds(durationEffect);

        task.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.05f);

        countEffectDoneStarted--;

        if (shirnkCoroutine != null) {
            yield break;
        }

        shirnkCoroutine = IEStartShrinkTaskPanel(0.6f);
        yield return shirnkCoroutine;
        shirnkCoroutine = null;
    }

    private IEnumerator IEStartCrossoutTask (TextMeshProUGUI task, float duration) {
        crossoutLine.gameObject.SetActive(true);
        crossoutLine.fillAmount = 0;
        crossoutLine.GetComponent<RectTransform>().anchoredPosition = new Vector2(task.GetComponent<RectTransform>().anchoredPosition.x, task.GetComponent<RectTransform>().anchoredPosition.y);

        float timeDown = 0f;
        while (timeDown < duration) {
            crossoutLine.fillAmount = timeDown / duration;
            timeDown += Time.deltaTime;
            yield return null;
        }
        crossoutLine.gameObject.SetActive(false);
    }

    private IEnumerator IEStartCrossoutParticlesTask (TextMeshProUGUI task, float duration) {
        crossoutParticles.GetComponent<ParticleSystem>().Play();

        crossoutParticles.transform.position = new Vector3(crossoutParticles.transform.position.x, task.transform.position.y, 0.0f);
        crossoutParticles.transform.localPosition = new Vector3(crossoutParticleBounds.x, crossoutParticles.transform.localPosition.y, 0.0f);

        float timeDown = 0f;
        while (timeDown < duration) {
            crossoutParticles.transform.localPosition = Vector3.Lerp(new Vector3(crossoutParticleBounds.x, crossoutParticles.transform.localPosition.y, crossoutParticles.transform.localPosition.z),
                                                                     new Vector3(crossoutParticleBounds.y, crossoutParticles.transform.localPosition.y, crossoutParticles.transform.localPosition.z),
                                                                     timeDown / duration);
            task.color = new Color(task.color.r, task.color.g, task.color.b, 1f - timeDown / duration);
            timeDown += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);
        crossoutParticles.GetComponent<ParticleSystem>().Stop();
    }

    private IEnumerator IEStartShrinkTaskPanel (float duration) {
        while (isDoneTaskEffect) {
            yield return null;
        }
        isShrink = true;

        int countTask = 0;

        foreach (TextMeshProUGUI t in tasksText) {
            if (t.gameObject.activeSelf)
                countTask++;
        }

        float startPanelHeight = taskPanel.sizeDelta.y;
        float panelSize = 30F + countTask * 50F + ((3 - countTask) * 10F);

        float timeDown = 0f;
        while (timeDown < duration) {
            taskPanel.sizeDelta = Vector2.Lerp(new Vector2(taskPanel.sizeDelta.x, startPanelHeight), new Vector2(taskPanel.sizeDelta.x, panelSize), timeDown / duration);
            timeDown += Time.deltaTime;
            yield return null;
        }

        isShrink = false;
    }
}
