using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RightButton : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    public Player2 player;

    public void OnPointerEnter(PointerEventData eventData)
    {
        player.horizontal = 1f;
        player.isRightDirectionButtonClicked = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        player.horizontal = 0f;
        player.isRightDirectionButtonClicked = false;
    }

}
