using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapArrowScrolling : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [SerializeField]
    private Sprite arrowActive;
    [SerializeField]
    private Sprite arrowPressed;
    [SerializeField]
    private Sprite arrowNormal;

    private bool isActive;
    private bool isPressed;

    private bool IsActive
    {
        get { return isActive; }
        set { isActive = value; UpdateState(); }
    }
    private bool IsPressed
    {
        get { return isPressed; }
        set {isPressed = value; UpdateState(); }
    }

    private void UpdateState()
    {
        if (IsPressed)
        {
            GetComponent<Image>().sprite = arrowPressed;
        }
        else
        {
            if(IsActive)
            {
                GetComponent<Image>().sprite = arrowActive;
            }
            else
            {
                GetComponent<Image>().sprite = arrowNormal;
                
            }
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        IsActive = true;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        IsPressed = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsActive = false;
    }
}
