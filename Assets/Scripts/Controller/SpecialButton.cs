using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpecialButton : MonoBehaviour, IPointerDownHandler
{
    public Player2 player;

    public void OnPointerDown(PointerEventData eventData) {
        player.isSpecialKeyPressed = true;
    }
}
