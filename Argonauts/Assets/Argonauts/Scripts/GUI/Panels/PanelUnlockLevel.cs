using System;
using System.Linq;
using HGL;
using TMPro;
using UnityEngine;
using Yodo1.MAS;

public class PanelUnlockLevel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI unlockLevelAdText;
    [SerializeField] private TextMeshProUGUI unlockAllLevelsText;
    [SerializeField] private TextMeshProUGUI restorePurchasesText;
    [SerializeField] private GameObject buttonRestorePurchases;

    private int _level = 0;
    private int _watchedAds = 0;
    private int _needWatchAds = 0;

    private void OnEnable()
    {
#if UNITY_ANDROID
        buttonRestorePurchases.SetActive(false);
#endif

        Yodo1U3dMasCallback.Rewarded.OnAdOpenedEvent += OnAdOpenedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdClosedEvent += OnAdClosedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;
    }

    private void OnDisable()
    {
        Yodo1U3dMasCallback.Rewarded.OnAdOpenedEvent -= OnAdOpenedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdClosedEvent -= OnAdClosedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdReceivedRewardEvent -= OnAdReceivedRewardEvent;
    }

    private void OnAdOpenedEvent()
    {
        AudioListener.volume = 0f;
    }

    private void OnAdReceivedRewardEvent()
    {
        OnAdWatched();
    }

    private void OnAdClosedEvent()
    {
        AudioListener.volume = 1f;
    }


    public void Init(int level)
    {
        _level = level;
        _watchedAds = 0;
        _needWatchAds = GetNeedWatchedAdsForLevel(level);
        
        UpdateTexts();
    }

    private int GetNeedWatchedAdsForLevel(int level)
    {
        if (level < 7) return 0;

        if (level % 2 == 0)
        {
            return 1;
        }

        int[] threeAdsLevels = {13, 19, 25, 31, 37, 43, 49, 55};
        if (threeAdsLevels.Contains(level))
        {
            return 3;
        }

        return 2;
    }

    public void OnButtonUnlockLevelAdClicked()
    {
#if UNITY_EDITOR
        OnAdWatched();
#else
        Yodo1U3dMas.ShowRewardedAd();
#endif
    }

    public void OnButtonUnlockAllLevelsClicked()
    {
        PurchaseManager.I.OnPurchaseFinished += OnPurchaseFinished;
        PurchaseManager.I.Purchase(PurchaseManager.ALL_LEVELS_IAP);
    }

    public void OnButtonRestorePurchasesClicked() {
        if (PurchaseManager.I.RestorePurchases()) {
            HGL_WindowManager.I.CloseWindow(null, null, nameof(PanelUnlockLevel), false);
            TransitionManager.I.LoadLevel(_level);
        }
    }

    private void OnPurchaseFinished(string id, bool success)
    {
        PurchaseManager.I.OnPurchaseFinished -= OnPurchaseFinished;

        if (success)
        {
            HGL_WindowManager.I.CloseWindow(null, null, nameof(PanelUnlockLevel), false);
            TransitionManager.I.LoadLevel(_level);
        }
    }

    public void OnButtonBackClicked()
    {
        HGL_WindowManager.I.CloseWindow(null, null, nameof(PanelUnlockLevel), false);
    }

    private void OnAdWatched()
    {
        _watchedAds++;

        UpdateTexts();

        if (_watchedAds >= _needWatchAds)
        {
            UserData.I.UnlockLevel(_level);
            HGL_WindowManager.I.CloseWindow(null, null, nameof(PanelUnlockLevel), false);
            TransitionManager.I.LoadLevel(_level);
        }
    }

    private void UpdateTexts()
    {
        unlockLevelAdText.text = string.Format(LocalizationManager.GetLocalizedString("shop_txt_body_watch"), _watchedAds, _needWatchAds);
        unlockAllLevelsText.text = string.Format(LocalizationManager.GetLocalizedString("shop_txt_body_unlock"), "<color=#FFFFFF>" + PurchaseManager.I.GetLocalizedPrice(PurchaseManager.ALL_LEVELS_IAP));
        restorePurchasesText.text = LocalizationManager.GetLocalizedString("shop_txt_body_restore");
    }
}
