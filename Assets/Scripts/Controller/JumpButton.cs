using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class JumpButton : MonoBehaviour, IPointerDownHandler
{
    public Player2 player;

    public void OnPointerDown(PointerEventData eventData) {
        player.isJumping = true;
    }


}
