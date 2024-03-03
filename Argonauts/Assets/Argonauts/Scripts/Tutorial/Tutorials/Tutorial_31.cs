using System;

public class Tutorial_31 : BaseTutorial {

    public override void Launch() {
        base.Launch();

        BaseInObject torch = GameManager.I.GetNearestObjects(GameManager.I.CampObject.transform.position, "torch")[1];
        if (torch == null) throw new NullReferenceException("torch");

        torch.OnBecameAvailable += Torch_OnBecameAvailable;
    }

    private void Torch_OnBecameAvailable(BaseInObject torch) {
        torch.OnBecameAvailable -= Torch_OnBecameAvailable;

        MoveArrow(torch.transform.position, ArrowSide.RightTop);
        ShowHint(LocalizationManager.GetLocalizedString("hint_31_1"), StopTutorial, true, 5F, -750);
    }
}
