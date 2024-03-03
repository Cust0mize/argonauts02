using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PanelResources : MonoBehaviour {
    [SerializeField] private ResourceBlock foodBlock;
    [SerializeField] private ResourceBlock woodBlock;
    [SerializeField] private ResourceBlock stoneBlock;
    [SerializeField] private ResourceBlock goldBlock;
    [SerializeField] private ResourceBlock gemBlock;

    //side alternatives
    [SerializeField] private ResourceBlock sideGoldBlock;

    private ResourceBlock FoodBlock {
        get {
            return foodBlock;
        }
    }

    private ResourceBlock WoodBlock {
        get {
            return woodBlock;
        }
    }

    private ResourceBlock StoneBlock {
        get {
            return stoneBlock;
        }
    }

    private ResourceBlock GoldBlock {
        get {
            return !isGemBlockExist ? sideGoldBlock : goldBlock;
        }
    }

    private ResourceBlock GemBlock {
        get {
            return gemBlock;
        }
    }

    private bool isGemBlockExist = false;

    private void Start() {
        isGemBlockExist = GameManager.I.LevelNumber >= 21;
        DeactiveUnusedBlocks();
    }

    private void DeactiveUnusedBlocks() {
        sideGoldBlock.gameObject.SetActive(!isGemBlockExist);
        goldBlock.gameObject.SetActive(isGemBlockExist);
        gemBlock.gameObject.SetActive(isGemBlockExist);
    }

    public void UpdateData(List<Resource> resources) {
		int food = 0, wood = 0, stone = 0, gold = 0, gem = 0;
		foreach (Resource res in resources) {
			switch (res.Type) {
				case Resource.Types.Food:
					food += res.Count;
					break;
				case Resource.Types.Wood:
					wood += res.Count;
					break;
				case Resource.Types.Stone:
					stone += res.Count;
					break;
				case Resource.Types.Gold:
					gold += res.Count;
					break;
                case Resource.Types.Gem:
                    gem += res.Count;
                    break;
            }
		}

        int lastFood = int.Parse(this.FoodBlock.Text.text);
        int lastWood = int.Parse(this.WoodBlock.Text.text);
        int lastStone = int.Parse(this.StoneBlock.Text.text);
        int lastGold = int.Parse(this.GoldBlock.Text.text);
        int lastGem = int.Parse(this.GemBlock.Text.text);

        this.FoodBlock.Text.text = food.ToString();
        this.WoodBlock.Text.text = wood.ToString();
        this.StoneBlock.Text.text = stone.ToString();
        this.GoldBlock.Text.text = gold.ToString();
        this.GemBlock.Text.text = gem.ToString();

        if (food - lastFood > 0) this.FoodBlock.PlayScale();
        if (wood - lastWood > 0) this.WoodBlock.PlayScale();
        if (stone - lastStone > 0) this.StoneBlock.PlayScale();
        if (gold - lastGold > 0) this.GoldBlock.PlayScale();
        if (gem - lastGem > 0) this.GemBlock.PlayScale();
    }

    public void CheckExistResource(List<Resource> resource)
    {
        foreach (Resource res in resource )
        {
            if(res.Type == Resource.Types.Food)
            {
                if (!GameManager.I.RequiredResourcesExist(new List<Resource> { res }))
                {
                    FoodBlock.Ping();
                }
            }
            if(res.Type == Resource.Types.Wood)
            {
                if (!GameManager.I.RequiredResourcesExist(new List<Resource> { res }))
                {
                    WoodBlock.Ping();
                }
            }
            if (res.Type == Resource.Types.Stone)
            {
                if (!GameManager.I.RequiredResourcesExist(new List<Resource> { res }))
                {
                    StoneBlock.Ping();
                }
            }
            if (res.Type == Resource.Types.Gold)
            {
                if (!GameManager.I.RequiredResourcesExist(new List<Resource> { res }))
                {
                    GoldBlock.Ping();
                }
            }
            if (res.Type == Resource.Types.Gem) {
                if (!GameManager.I.RequiredResourcesExist(new List<Resource> { res })) {
                    GemBlock.Ping();
                }
            }
        }
    }
}
