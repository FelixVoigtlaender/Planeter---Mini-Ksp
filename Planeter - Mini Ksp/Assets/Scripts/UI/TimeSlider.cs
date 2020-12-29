using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TimeSlider : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler
{
    public Slider slider;

    public bool clicked = false;

    public Image handleImage;
    public Sprite pauseIcon;
    public Sprite playIcon;

    bool isDragging = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
            return;

        OTime.isPaused = !OTime.isPaused;
        slider.value = slider.minValue;
        clicked = !clicked;

        CheckSprite();
    }

    public void OnDrag(PointerEventData eventData)
    {
        isDragging = true;
        OTime.isPaused = false;
        CheckSprite();
    }

    public void CheckSprite()
    {


        handleImage.sprite = !OTime.isPaused ? pauseIcon : playIcon;
    }
}
