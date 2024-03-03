using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_21  : BaseTutorial {
    private int countActivate = 0;

    public override void Launch() {
        base.Launch();
        NotificationCenter.DefaultCenter().AddObserver(this, "OnPortalActivated");
    }

    private void OnPortalActivated(Notification notif) {
        MoveArrow(notif.sender.transform.position, ArrowSide.LeftBot);
        MoveArrow(notif.sender.GetComponent<PortalObject>().MirrorPortalController.BaseBuildObject.transform.position, ArrowSide.LeftBot);

        float y = notif.sender.transform.name.Contains("greenportal") ? -850 : -650;

        ShowHint(LocalizationManager.GetLocalizedString("hint_21_1"), () => {
            HideArrows();

            countActivate++;
            if (countActivate >= 2) {
                NotificationCenter.DefaultCenter().RemoveObserver(this, "OnPortalActivated");
                StopTutorial();
            } 
        }, true, 5F, y);
    }
}
