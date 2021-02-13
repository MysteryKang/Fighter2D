using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonB : MonoBehaviour, IPointerDownHandler
{
    public Player2 player;
    public UnityEvent input;
    public bool clicked = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        clicked = true;
        input.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clicked = false;
    }
}
