using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AwardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] private string baseKey;
    [SerializeField] private int level;

    public string Key {
        get {
            return baseKey;
        }
    }

    public int Level {
        get {
            return level;
        }
    }

    public void Init(bool unlocked) {
        if(!unlocked) GetComponent<Image>().sprite = AwardScreenManager.I.DisabledAwardSprite;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        AwardScreenManager.I.Show(this);
    }

    public void OnPointerExit(PointerEventData eventData) {
        AwardScreenManager.I.Hide();
    }
}
