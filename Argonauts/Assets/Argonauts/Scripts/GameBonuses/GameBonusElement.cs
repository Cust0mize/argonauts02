using HGL;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameBonusElement : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Sprite characterMoveSpeed;
    [SerializeField] private Sprite characterWorkSpeed;
    [SerializeField] private Sprite buildsCost;
    [SerializeField] private Sprite twoWorkers;
    [SerializeField] private Sprite market;
    [SerializeField] private Sprite enemyCost;
    [SerializeField] private Sprite levelBonusCooldown;
    [SerializeField] private Sprite levelBonusDuration;

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI timeValueText;

    [SerializeField] private Color greenColor;
    [SerializeField] private Color orangeColor;
    [SerializeField] private Color redColor;

    [SerializeField] private ParticleSystem timeoutEffect;

    private GameBonus gameBonus;

    public GameBonus GameBonus {
        get {
            return gameBonus;
        }
    }

    public void Init(GameBonus gameBonus)
    {
        StopAllCoroutines();
        this.gameBonus = gameBonus;
        iconImage.sprite = GetIconSprite();
        StartCoroutine(BonusUIWorker());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator BonusUIWorker()
    {
        while(!gameBonus.BonusOver()) {
            int totalSeconds = GameBonus.DURATION - gameBonus.PassedTime();

            timeValueText.text = GetTimeFormat();
            if (totalSeconds >= 360F) {
                timeValueText.color = greenColor;
            } else if (totalSeconds >= 180 && totalSeconds < 360F) {
                timeValueText.color = orangeColor;
            } else if (totalSeconds < 180F) {
                timeValueText.color = redColor;
            }

            yield return new WaitForSecondsRealtime(1F);
        }
    }

    private Sprite GetIconSprite()
    {
        switch(gameBonus.BonusType) {
            case GameBonus.GameBonusTypes.CharacterMoveSpeed: return characterMoveSpeed;
            case GameBonus.GameBonusTypes.CharacterWorkSpeed: return characterWorkSpeed;
            case GameBonus.GameBonusTypes.BuildsCost: return buildsCost;
            case GameBonus.GameBonusTypes.TwoWorkers: return twoWorkers;
            case GameBonus.GameBonusTypes.Market: return market;
            case GameBonus.GameBonusTypes.EnemyCost: return enemyCost;
            case GameBonus.GameBonusTypes.LevelBonusCooldown: return levelBonusCooldown;
            case GameBonus.GameBonusTypes.LevelBonusDuration: return levelBonusDuration;
            default: return null;
        }
    }

    private string GetTimeFormat()
    {
        int totalSeconds = GameBonus.DURATION - gameBonus.PassedTime();
        int seconds = totalSeconds % 60;
        int minutes = totalSeconds / 60;
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PanelBonusDescription panelDescription = HGL_WindowManager.I.GetWindow("PanelBonusDescription").GetComponent<PanelBonusDescription>();
        if (panelDescription.CurrentElement == this)
            panelDescription.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PanelBonusDescription panelDescription = HGL_WindowManager.I.GetWindow("PanelBonusDescription").GetComponent<PanelBonusDescription>();
        panelDescription.transform.position = transform.position;
        panelDescription.CurrentElement = this;
        panelDescription.SetText(LocalizationManager.GetLocalizedString(string.Format("game_bonus_{0}", GameBonus.BonusType.ToString().ToLower())));
        panelDescription.gameObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PanelBonusDescription panelDescription = HGL_WindowManager.I.GetWindow("PanelBonusDescription").GetComponent<PanelBonusDescription>();
        panelDescription.transform.position = transform.position;
        panelDescription.CurrentElement = this;
        panelDescription.SetText(LocalizationManager.GetLocalizedString(string.Format("game_bonus_{0}", GameBonus.BonusType.ToString().ToLower())));
        panelDescription.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PanelBonusDescription panelDescription = HGL_WindowManager.I.GetWindow("PanelBonusDescription").GetComponent<PanelBonusDescription>();
        if (panelDescription.CurrentElement == this)
            panelDescription.gameObject.SetActive(false);
    }

    public void PlayEmptyEffect()
    {
        timeoutEffect.gameObject.SetActive(true);
        timeoutEffect.Play();
    }
}
