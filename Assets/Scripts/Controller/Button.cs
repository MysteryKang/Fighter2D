﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Player2 player;
    public UnityEvent input;
    public bool clicked = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        clicked = true;
        if (player.isAlive)
            input.Invoke();
        else
            return;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        clicked = false;
    }

}
