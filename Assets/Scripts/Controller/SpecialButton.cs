using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SpecialButton : MonoBehaviour, IPointerDownHandler
{
    public Player2 player;
    public UnityEvent input;
    public bool clicked;

    public void OnPointerDown(PointerEventData eventData) {
        clicked = true;
        input.Invoke();
    }
}
