using UnityEngine;
using UnityEngine.UI;

public class HudBar : MonoBehaviour {
	[SerializeField] Image Fill;
	[SerializeField] Image Background;
	[SerializeField] Image DampFill;
	[SerializeField] Text Text;

	public float SpeedDamp = 5f;
	private bool isDamp = false;

	private float startFillY;
	private float startDampY;

	void Start () {
		startFillY = Fill.rectTransform.localPosition.y;

		if (DampFill != null) {
			startDampY = DampFill.rectTransform.localPosition.y;

			DampFill.rectTransform.localPosition = Fill.rectTransform.localPosition;
			DampFill.gameObject.SetActive (false);
		}
	}

	public void UpdateBar (float value, float maxValue, bool smooth) {
		if (maxValue == 0)
			return;

		if (Text != null) {
			Text.text = (int)value + "/" + (int)maxValue;
		}

		float pos = 0;
		pos = (value / maxValue) * Fill.rectTransform.rect.width - Fill.rectTransform.rect.width;

		Fill.rectTransform.localPosition = new Vector2 (pos, startFillY);

		if (!smooth) {
			if (DampFill != null)
				DampFill.rectTransform.localPosition = new Vector2 (pos, startDampY);
		}
	}

	public void CheckDampNeed (float maxDelta, float value, int maxValue) {
		if (Vector2.Distance (Fill.rectTransform.localPosition, DampFill.rectTransform.localPosition) <= maxDelta) {
			DampFill.gameObject.SetActive (false);
			DampFill.rectTransform.localPosition = Fill.rectTransform.localPosition;
		} else {
			DampFill.gameObject.SetActive (true);

			float pos = 0;
			pos = ((float)value / (float)maxValue) * DampFill.rectTransform.rect.width - DampFill.rectTransform.rect.width;

			DampFill.rectTransform.localPosition = Vector2.Lerp (DampFill.rectTransform.localPosition, new Vector2 (pos, startDampY), Time.deltaTime * SpeedDamp);
		}
	}
}
