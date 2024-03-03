using UnityEngine;
using UnityEngine.UI;

public class GameBonusPanelChest : MonoBehaviour
{
    [SerializeField] private ParticleSystem emptyEffect;
    [SerializeField] private Sprite openChestSprite;
    [SerializeField] private Sprite closeChestSprite;

    [SerializeField] private Image fakePrizeImage;

    public void SetOpenState(bool isOpen)
    {
        GetComponent<Image>().sprite = isOpen ? openChestSprite : closeChestSprite;
        GetComponent<Image>().color = isOpen ? new Color(0.5f, 0.5f, 0.5f, 0.5f) : Color.white;
    }

    public void PlayEmptyEffect()
    {
        emptyEffect.gameObject.SetActive(true);
        emptyEffect.Play();
    }

    public void SetPosition(float pos)
    {
        GetComponent<RectTransform>().anchoredPosition = new Vector2(GetComponent<RectTransform>().anchoredPosition.x, pos);
    }

    public void SetActiveFakePrize(Sprite prizeSprite)
    {
        if (prizeSprite != null) {
            fakePrizeImage.gameObject.SetActive(true);
            fakePrizeImage.sprite = prizeSprite;
        } else {
            fakePrizeImage.gameObject.SetActive(false);
        }
    }

    public void Reset()
    {
        emptyEffect.gameObject.SetActive(false);
        SetOpenState(false);
        gameObject.SetActive(false);
        SetActiveFakePrize(null);
    }
}
