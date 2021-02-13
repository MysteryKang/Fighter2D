using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class JumpButton : MonoBehaviour, IPointerDownHandler
{
    public Player2 player;
    public UnityEvent input;

    public void OnPointerDown(PointerEventData eventData)
    {
        input.Invoke();
    }


}
