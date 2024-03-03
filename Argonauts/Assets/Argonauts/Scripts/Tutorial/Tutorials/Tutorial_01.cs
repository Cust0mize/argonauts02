using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_01 : BaseTutorial {
    private bool foodClicked;
    private bool woodClicked;

    private int foodWoodPicked = 0;

    private GameObject foodArrow;
    private GameObject woodArrow;

    private BaseInObject lastObject;

    private GameObject goldArrow;

    public override void Launch() {
        base.Launch();

        BaseInObject food = GameManager.I.GetNearestObject(GameManager.I.CampObject.transform.position, "food");
        if (food == null) throw new NullReferenceException("food");

        OverseerController.I.OnObjectAddedInOrder += FirstFood_AddedInOrder;
 
        MoveArrow(food.transform.position, ArrowSide.LeftBot);
        ShowHint(LocalizationManager.GetLocalizedString("hint_1_1"), new Vector2(0.5F, 0.535F), new Vector2(0.5F, 0.535F), 0, null, false, 1);
    }

    private void FirstFood_AddedInOrder(BaseInObject obj) {
        OverseerController.I.OnObjectAddedInOrder -= FirstFood_AddedInOrder;

        HideHint();
        HideLastArrow();

        OverseerController.I.OnArriveHomeEvent += OnArriveHomeFirstFood;
    }

    private void OnArriveHomeFirstFood(BaseInObject obj) {
        OverseerController.I.OnArriveHomeEvent -= OnArriveHomeFirstFood;

        BaseInObject obst_wood02 = GameManager.I.GetObjectOnNamePart("obst_wood02");
        if (obst_wood02 == null) throw new NullReferenceException("obst_wood02");

        OverseerController.I.OnObjectAddedInOrder += Obst_Wood02_AddedInOrder;

        MoveArrow(obst_wood02.transform.position, ArrowSide.LeftBot);
        ShowHint(LocalizationManager.GetLocalizedString("hint_1_2"), new Vector2(0.5F, 0.535F), new Vector2(0.5F, 0.535F), 0, null, false, 1);
    }

    private void Obst_Wood02_AddedInOrder(BaseInObject obj) {
        OverseerController.I.OnObjectAddedInOrder -= Obst_Wood02_AddedInOrder;

        HideHint();
        HideLastArrow();

        OverseerController.I.OnArriveHomeEvent += OnArriveHomeObstWood2;
    }

    private void OnArriveHomeObstWood2(BaseInObject obj) {
        OverseerController.I.OnArriveHomeEvent -= OnArriveHomeObstWood2;

        BaseInObject wood = GameManager.I.GetObjectOnNamePart("wood");
        BaseInObject food = GameManager.I.GetObjectOnNamePart("food");
        if (wood == null) throw new NullReferenceException("wood");
        if (food == null) throw new NullReferenceException("food");

        OverseerController.I.OnObjectAddedInOrder += FoodWood_AddedInOrder;
        OverseerController.I.OnArriveHomeEvent += OnArriveHomeFoodWood;

        woodArrow = MoveArrow(wood.transform.position, ArrowSide.RightTop);
        foodArrow = MoveArrow(food.transform.position, ArrowSide.LeftBot);
        ShowHint(LocalizationManager.GetLocalizedString("hint_1_3"), new Vector2(0.5F, 0.535F), new Vector2(0.5F, 0.535F), 0, null, false, 1);
    }

    private void FoodWood_AddedInOrder(BaseInObject obj) {
        if (obj.gameObject.name.ToLower().Contains("wood")) {
            HideArrow(woodArrow);
            woodClicked = true;
        }
        if (obj.gameObject.name.ToLower().Contains("food")) {
            HideArrow(foodArrow);
            foodClicked = true;
        }

        if (woodClicked && foodClicked) HideHint();
    }

    private void OnArriveHomeFoodWood(BaseInObject obj) {
        foodWoodPicked++;

        if (foodWoodPicked >= 2) {
            OverseerController.I.OnArriveHomeEvent -= OnArriveHomeFoodWood;
            HandleBridge();
        }
    }

    private void HandleBridge() {
        OverseerController.I.OnObjectAddedInOrder -= FoodWood_AddedInOrder;

        BaseInObject bridge = GameManager.I.GetObjectOnNamePart("bridge");
        if (bridge == null) throw new NullReferenceException("bridge");

        OverseerController.I.OnObjectAddedInOrder += Bridge_AddedInOrder;

        MoveArrow(bridge.transform.position, ArrowSide.LeftTop);
        ShowHint(LocalizationManager.GetLocalizedString("hint_1_4"), new Vector2(0.5F, 0.535F), new Vector2(0.5F, 0.535F), 0, null, false, 1);
    }

    private void Bridge_AddedInOrder(BaseInObject obj) {
        OverseerController.I.OnObjectAddedInOrder -= Bridge_AddedInOrder;

        HideHint();
        HideLastArrow();

        obj.OnDestroyed += Bridge_OnDestroyed;
    }

    private void Bridge_OnDestroyed(Node node) {
        BaseInObject gold = GameManager.I.GetObjectOnNamePart("gold");
        BaseInObject obst_bush = GameManager.I.GetObjectOnNamePart("obst_bush");
        if (gold == null) throw new NullReferenceException("gold");
        if (obst_bush == null) throw new NullReferenceException("obst_bush");

        lastObject = obst_bush;

        OverseerController.I.OnObjectAddedInOrder += ObstBush_AddedInOrder;

        goldArrow = MoveArrow(gold.transform.position, ArrowSide.LeftTop);
        MoveArrow(obst_bush.transform.position, ArrowSide.LeftTop);
        ShowHint(LocalizationManager.GetLocalizedString("hint_1_5"), new Vector2(0.5F, 0.535F), new Vector2(0.5F, 0.535F), 0, null, false, 1);
    }

    private void ObstBush_AddedInOrder(BaseInObject obj) {
        if (obj == lastObject)
            StopTutorial();
        else HideArrow(goldArrow);
    }
}
