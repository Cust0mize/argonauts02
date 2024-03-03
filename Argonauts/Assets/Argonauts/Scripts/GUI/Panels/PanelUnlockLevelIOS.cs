using System;
using System.Linq;
using HGL;
using TMPro;
using UnityEngine;
using Yodo1.MAS;

public class PanelUnlockLevelIOS : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unlockAllLevelsText;
    [SerializeField] private TextMeshProUGUI restorePurchasesText;
    [SerializeField] private GameObject unlockLevelsButtonIOS;

    public void Init(int level)
    {
        UpdateTexts();
    }


    public void OnButtonUnlockAllLevelsClicked()
    {
        PurchaseManager.I.OnPurchaseFinished += OnPurchaseFinished;
        PurchaseManager.I.Purchase(PurchaseManager.ALL_LEVELS_IAP);
    }

    public void OnButtonRestorePurchasesClicked() {
        if (PurchaseManager.I.RestorePurchases()) {
            unlockLevelsButtonIOS.SetActive(!UserData.I.IsLevelUnlocked(UserData.COUNT_LEVELS_IN_GAME));
            HGL_WindowManager.I.CloseWindow(null, null, nameof(PanelUnlockLevelIOS), false);
        }
    }

    private void OnPurchaseFinished(string id, bool success)
    {
        PurchaseManager.I.OnPurchaseFinished -= OnPurchaseFinished;

        if (success)
        {
            unlockLevelsButtonIOS.SetActive(!UserData.I.IsLevelUnlocked(UserData.COUNT_LEVELS_IN_GAME));
            HGL_WindowManager.I.CloseWindow(null, null, nameof(PanelUnlockLevelIOS), false);
        }
    }

    public void OnButtonBackClicked()
    {
        HGL_WindowManager.I.CloseWindow(null, null, nameof(PanelUnlockLevelIOS), false);
    }

    private void UpdateTexts()
    {
        unlockAllLevelsText.text = string.Format(LocalizationManager.GetLocalizedString("shop_txt_body_unlock"), "<color=#FFFFFF>" + PurchaseManager.I.GetLocalizedPrice(PurchaseManager.ALL_LEVELS_IAP));
        restorePurchasesText.text = LocalizationManager.GetLocalizedString("shop_txt_body_restore");
    }
}
