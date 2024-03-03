using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameBonusesUIManager : LocalSingletonBehaviour<GameBonusesUIManager> {
    [SerializeField] private GameBonusElement gameBonusElementPrefab;
    [SerializeField] private GameObject bonusOverEffectPrefab;
    [SerializeField] private Transform elementContainer;
    private List<GameBonusElement> items = new List<GameBonusElement>();

    [SerializeField] private FlyingStarElement flyingStarElementPrefab;
    [SerializeField] private Transform flyingStarsParent;

    [SerializeField] private GameBonusBlock bonusBlock;
    [SerializeField] private bool isMap = true;

    public override void DoAwake()
    {
        foreach(GameBonus bonus in GameBonusesManager.I.GetActiveBonuses()) {
            GameBonusElement bonusElement = Instantiate(gameBonusElementPrefab, elementContainer);
            bonusElement.Init(bonus);
            items.Add(bonusElement);
        }

        if (isMap) bonusBlock.OnStarAppear(GameBonusesProgressManager.I.LastCountStars, false);
    }

    public void UpdateBonuses()
    {
        foreach (GameBonus bonus in GameBonusesManager.I.GetActiveBonuses()) {
            GameBonusElement existElement = items.Where(x => x.GameBonus == bonus).FirstOrDefault();
            if(existElement != null) {
                existElement.Init(bonus);
                continue;
            }

            GameBonusElement bonusElement = Instantiate(gameBonusElementPrefab, elementContainer);
            bonusElement.Init(bonus);
            items.Add(bonusElement);
        }
    }

    private void Update()
    {
        for (int i = 0; i < items.Count; i++) {
            if (items[i].GameBonus.BonusOver()) {
                GameObject obj = items[i].gameObject;
                GameObject eff = Instantiate(bonusOverEffectPrefab, obj.transform.parent.parent);
                eff.transform.position = obj.transform.position;
                StartCoroutine(DelayDestroy(0.3F, obj));
                items[i] = null;
            }
        }
        items.RemoveAll(x => x == null);
    }

    private IEnumerator DelayDestroy(float t, GameObject obj)
    {
        yield return new WaitForSeconds(t);
        Destroy(obj);
    }

    public GameBonusElement GetBonusElement(GameBonus.GameBonusTypes type)
    {
        return items.Where(x => x.GameBonus.BonusType == type).FirstOrDefault();
    }

    public IEnumerator AnimateFillStars(int alreadyFilledStars, int countNewStars, int levelNumber, float delayBetweenStars)
    {
        for (int i = 0; i < countNewStars; i++) {
            FlyingStarElement flyingStar = Instantiate(flyingStarElementPrefab, flyingStarsParent);

            if (MapManager.I.isExtra) {
                flyingStar.transform.position = MapManager.I.Levels[levelNumber - 51].Stars[Mathf.Clamp(i, 0, 2)].transform.position;
            }
            else {
                flyingStar.transform.position = MapManager.I.Levels[levelNumber - 1].Stars[Mathf.Clamp(i, 0, 2)].transform.position;
            }

            StartCoroutine(FlyAndDestroy(flyingStar, flyingStar.transform.position, bonusBlock.GetStar(alreadyFilledStars + i).transform, alreadyFilledStars, i));
            yield return new WaitForSeconds(delayBetweenStars);
        }
        yield return null;
    }

    public void ClearStars()
    {
        bonusBlock.ClearStars();
    }

    private IEnumerator FlyAndDestroy(FlyingStarElement star, Vector2 from, Transform to, int alreadyFilledStars, int i)
    {
        yield return star.DoFly(from, to);
        bonusBlock.OnStarAppear(alreadyFilledStars + (i + 1), false);
        Destroy(star.gameObject);
    }
}
