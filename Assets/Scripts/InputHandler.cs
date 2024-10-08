using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    public GameManager gameManager;
    private Camera cam;


    private void Awake()
    {
        cam = Camera.main;
    }

    public void OnPlayClick()
    {

    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        var rayHit = Physics2D.GetRayIntersection(cam.ScreenPointToRay(Mouse.current.position.ReadValue()));
        if (!rayHit.collider) return;




        GameObject hitCard = rayHit.collider.gameObject;

        if (hitCard.GetComponent<CardData>().cardIsActive())
        {
            gameManager.checkNewCard(hitCard);
        }
    }
}
