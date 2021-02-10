using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonA : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Player2 player;

    public void OnPointerDown(PointerEventData eventData) {
        player.punching = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }
}
