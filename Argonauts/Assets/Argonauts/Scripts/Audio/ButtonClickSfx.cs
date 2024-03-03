using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonClickSfx : MonoBehaviour, IPointerClickHandler {
	public void OnPointerClick (PointerEventData eventData) {
        if (GetComponent<Button>() != null) {
            if (GetComponent<Button>().interactable) {
                AudioWrapper.I.PlaySfx("buttonclick");
            }
        } else {
            AudioWrapper.I.PlaySfx("buttonclick");
        }
	}
}
