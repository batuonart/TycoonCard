using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class Draggable : MonoBehaviour
{
    Vector2 ogSize;


    private float hoverSize = 1.2f;
    private float hoverDuration = 0.3f;


    private void Start()
    {
        ogSize = transform.localScale;
    }

    bool getCardIsActive()
    {
        return gameObject.GetComponent<CardData>().cardIsActive();
    }

    void OnMouseOver()
    {
       if (getCardIsActive()) transform.DOScale(ogSize * hoverSize, hoverDuration);
    }

    void OnMouseExit()
    {
        if (getCardIsActive()) transform.DOScale(ogSize, hoverDuration);
    }
}
