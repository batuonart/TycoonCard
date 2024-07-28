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

    bool isHoverable = true;






    private void Start()
    {
        ogSize = transform.localScale;
    }

    public void disableHovering()
    {
        isHoverable = false;
    }

  
  /*
    private void OnMouseDown()
    {
        diff = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
    }

    private void OnMouseDrag()
    {
        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - diff;
    }
  */

    void OnMouseOver()
    {
       if (isHoverable) transform.DOScale(ogSize * hoverSize, hoverDuration);
    }

    void OnMouseExit()
    {
        if (isHoverable) transform.DOScale(ogSize, hoverDuration);
    }
}
