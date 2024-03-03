using HGL;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameBonusPanel : MonoBehaviour
{
    [SerializeField] private GameBonusPanelChest[] chests;
    private bool chestSelected = false;

    [SerializeField] private Transform prizeEndPoint;
    [SerializeField] private FlyingPrizeElement flyingPrize;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI prizeDescription;
    [SerializeField] private GameObject bonusElementPointPrefab;
    [SerializeField] private Transform bonusElementPointContainer;

    [SerializeField] private float normalChestPosition;
    [SerializeField] private float highChestPosition;

    private GameBonus lastPrize;
    private bool readyToSkip;

    public GameBonusPanelChest[] Chests {
        get {
            return chests;
        }
    }

    public void Reset()
    {
        foreach (GameBonusPanelChest chest in chests) {
            chest.Reset();
            chest.SetPosition(normalChestPosition);
            chest.GetComponent<TweenAnchoredPosition>().ResetToBeginning();
        }
        chestSelected = false;
        flyingPrize.gameObject.SetActive(false);
        prizeDescription.gameObject.SetActive(false);
        title.gameObject.SetActive(false);
    }

    public void OnClick(GameBonusPanelChest chest)
    {
        if(!chestSelected) {
            chest.SetPosition(normalChestPosition);

            bool win = GameBonusesManager.I.ChestRandomBlock.Next();

            chest.SetOpenState(true);
            SetActiveTitle(false);

            if (!win) {
                chest.PlayEmptyEffect();
                StartCoroutine(EmptyChest(chest));
            } else {
                flyingPrize.transform.position = chest.transform.position;
                StartCoroutine(WinChest(chest));
            }

            chestSelected = true;
        }
    }

    public void SetActiveChests(bool value)
    {
        foreach(GameBonusPanelChest chest in chests) {
            chest.gameObject.SetActive(value);
        }
    }

    public void SetActiveTitle(bool value)
    {
        title.gameObject.SetActive(value);
    }

    private IEnumerator EmptyChest(GameBonusPanelChest chest)
    {
        List<GameBonus.GameBonusTypes> exceptionTypes = new List<GameBonus.GameBonusTypes>();

        foreach (GameBonusPanelChest ch in chests) {
            ch.SetOpenState(true);

            if (ch != chest) {
                GameBonus.GameBonusTypes prType = GetRandomBonus(exceptionTypes);
                exceptionTypes.Add(prType);

                GameBonus pr = new GameBonus(prType, DateTime.UtcNow);
                ch.SetActiveFakePrize(flyingPrize.GetIconSprite(pr));
            }
        }

        yield return new WaitForSeconds(2F);
        foreach (GameBonusPanelChest ch in chests) {
            ch.GetComponent<TweenAnchoredPosition>().PlayForward();
        }
        yield return new WaitForSeconds(0.5F);
        HGL_WindowManager.I.CloseWindow(null, null, "GameBonusPanel", false);
    }

    private IEnumerator WinChest(GameBonusPanelChest chest)
    {
        lastPrize = new GameBonus(GameBonusesManager.I.PrizeRandomBlock.Next(), DateTime.UtcNow);

        yield return new WaitForSeconds(0.1F);

        bool firstEmptyChest = UnityEngine.Random.Range(0, 1).Equals(1);
        bool emptyChest = false;
        int i = 0;

        List<GameBonus.GameBonusTypes> exceptionTypes = new List<GameBonus.GameBonusTypes>() {
            lastPrize.BonusType
        };

        foreach (GameBonusPanelChest ch in chests) {
            ch.SetOpenState(true);

            if (ch != chest) {
                if (!emptyChest) {
                    if (firstEmptyChest && i == 0) {
                        GameBonus.GameBonusTypes prType = GetRandomBonus(exceptionTypes);
                        exceptionTypes.Add(prType);

                        GameBonus pr = new GameBonus(prType, DateTime.UtcNow);
                        ch.SetActiveFakePrize(flyingPrize.GetIconSprite(pr));
                    } else {
                        GameBonus.GameBonusTypes prType = GetRandomBonus(exceptionTypes);
                        exceptionTypes.Add(prType);

                        GameBonus pr = new GameBonus(prType, DateTime.UtcNow);
                        ch.SetActiveFakePrize(flyingPrize.GetIconSprite(pr));
                    }
                    emptyChest = true;
                } else {
                    ch.PlayEmptyEffect();
                }

                i++;
            }
        }
        flyingPrize.gameObject.SetActive(true);
        flyingPrize.UpdateSprite(lastPrize);

        yield return new WaitForSeconds(2F);

        foreach (GameBonusPanelChest ch in chests) {
            ch.GetComponent<TweenAnchoredPosition>().PlayForward();
        }

        yield return flyingPrize.DoFly(300F, flyingPrize.transform.position, prizeEndPoint);
        prizeDescription.gameObject.SetActive(true);
        prizeDescription.text = LocalizationManager.GetLocalizedString(string.Format("game_bonus_{0}", lastPrize.BonusType.ToString().ToLower()));

        readyToSkip = true;

        yield return new WaitForSeconds(7F);
        yield return DropPrize();
    }

    private IEnumerator DropPrize()
    {
        bool needDestroyPoint = false;
        GameBonusElement bonusElement = GameBonusesUIManager.I.GetBonusElement(lastPrize.BonusType);
        GameObject point = null;
        if (bonusElement != null) point = bonusElement.gameObject;
        else {
            needDestroyPoint = true;
            point = Instantiate(bonusElementPointPrefab, bonusElementPointContainer);
        }
        yield return flyingPrize.DoFly(1500F, flyingPrize.transform.position, point.transform, flyingPrize.GetComponent<RectTransform>().sizeDelta, Vector2.one * 100F);
        if (needDestroyPoint) Destroy(point);
        GameBonusesManager.I.StartBonus(lastPrize.BonusType);
        GameBonusesUIManager.I.UpdateBonuses();

        if (!UserData.I.BonusAppearHintShowed()) {
            UserData.I.SetBonusAppearHintStatus(true);

            HGL_WindowManager.I.OpenWindow(null, null, "PanelBonusAppearHint", false, true);
        }

        HGL_WindowManager.I.CloseWindow(null, null, "GameBonusPanel", false);
    }

    private GameBonus.GameBonusTypes GetRandomBonus(List<GameBonus.GameBonusTypes> exceptions)
    {
        GameBonus.GameBonusTypes rnd = (GameBonus.GameBonusTypes)UnityEngine.Random.Range(0, 7);
        while (exceptions.Contains(rnd)) {
            rnd = (GameBonus.GameBonusTypes)UnityEngine.Random.Range(0, 7);
        }
        return rnd;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && readyToSkip) {
            readyToSkip = false;
            StopAllCoroutines();
            StartCoroutine(DropPrize());
        }
    }
}
