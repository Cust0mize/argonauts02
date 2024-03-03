using HGL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameBonusBlock : MonoBehaviour
{
    [SerializeField] private GameObject openChestObject;
    [SerializeField] private GameObject closeChestObject;

    [SerializeField] private Sprite activeStarSprite;
    [SerializeField] private Sprite unactiveStarSprite;

    [SerializeField] private Image[] stars;

    [SerializeField] private FlyingChestElement flyingChestElementPrefab;
    [SerializeField] private Transform flyingChestParent;

    public Image GetFirstUnactiveStar()
    {
        foreach (Image starImage in stars) {
            if (starImage.sprite == unactiveStarSprite) {
                return starImage;
            }
        }
        return stars[stars.Length - 1];
    }

    public Image GetStar(int i)
    {
        return stars[i];
    }

    public int CountActiveStars()
    {
        int result = 0;
        for (int i = 0; i < stars.Length; i++) {
            if (stars[i].sprite == activeStarSprite) {
                result++;
            }
        }
        return result;
    }

    public void OnStarAppear(int countNowStars, bool canDecrease = true)
    {
        if (!canDecrease && countNowStars < CountActiveStars()) return;

        if (countNowStars >= 5) {
            openChestObject.gameObject.SetActive(true);
            closeChestObject.gameObject.SetActive(false);
        } else {
            openChestObject.gameObject.SetActive(false);
            closeChestObject.gameObject.SetActive(true);
        }

        UpdateStarImages(countNowStars);

        if(countNowStars >= 5 && !UserData.I.TakePrizeHintShowed()) {
            UserData.I.SetTakePrizeHintStatus(true);

            HGL_WindowManager.I.OpenWindow(null, null, "PanelTakePrizeHint", false, true);
            StartCoroutine(HideTakePrizeHintWithDelay(7F));
        }
    }

    public void ClearStars()
    {
        openChestObject.gameObject.SetActive(false);
        closeChestObject.gameObject.SetActive(true);
        UpdateStarImages(0);
    }

    public void OnClick()
    {
        if (HGL_WindowManager.I.IsOpenOrOpening("PanelTakePrizeHint")) {
            HGL_WindowManager.I.CloseWindow(null, null, "PanelTakePrizeHint", false);
        }

        GameBonusesProgressManager.I.ClearOnUse();

        GameBonusPanel panel = HGL_WindowManager.I.GetWindow("GameBonusPanel").GetComponent<GameBonusPanel>();
        panel.Reset();
        HGL_WindowManager.I.OpenWindow(null, () => { StartCoroutine(AnimateChests(0.2F)); }, "GameBonusPanel", false, true);
    }

    private void UpdateStarImages(int countNowStars)
    {
        for (int i = 0; i < stars.Length; i++) {
            stars[i].sprite = i < countNowStars ? activeStarSprite : unactiveStarSprite;
        }
    }

    public IEnumerator AnimateChests(float delayBetweenChests)
    {
        GameBonusPanel panel = HGL_WindowManager.I.GetWindow("GameBonusPanel").GetComponent<GameBonusPanel>();
        List<GameObject> chestsToDestroy = new List<GameObject>();

        for (int i = 0; i < 3; i++) {
            FlyingChestElement flyingChest = Instantiate(flyingChestElementPrefab, flyingChestParent);
            flyingChest.transform.position = closeChestObject.transform.position;

            StartCoroutine(FlyAndDestroy(
                flyingChest,
                flyingChest.transform.position,
                panel.Chests[i].transform,
                closeChestObject.GetComponent<RectTransform>().sizeDelta,
                panel.Chests[i].GetComponent<RectTransform>().sizeDelta,
                chestsToDestroy));

            yield return new WaitForSeconds(delayBetweenChests);
        }

        yield return DestroyChests(chestsToDestroy);
        panel.SetActiveChests(true);
        panel.SetActiveTitle(true);
    }

    private IEnumerator FlyAndDestroy(FlyingChestElement chest, Vector2 from, Transform to, Vector2 fromScale, Vector2 toScale, List<GameObject> chestsToDestroy)
    {
        yield return chest.DoFly(from, to, fromScale, toScale);
        chestsToDestroy.Add(chest.gameObject);
    }

    private IEnumerator DestroyChests(List<GameObject> chestsToDestroy)
    {
        while(chestsToDestroy.Count < 3) {
            yield return null;
        }
        foreach (GameObject chest in chestsToDestroy) {
            Destroy(chest);
        }
    }

    private IEnumerator HideTakePrizeHintWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HGL_WindowManager.I.CloseWindow(null, null, "PanelTakePrizeHint", false);
    }
}
