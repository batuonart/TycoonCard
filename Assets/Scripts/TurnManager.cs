
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static DeckManager.Card;
using UnityEngine.Events;
using System.Collections.Generic;
using Unity.Netcode;


public class TurnManager : NetworkBehaviour
{

    
    public TextMeshProUGUI turnText;
   



    public void AnnounceTurn(string playerUsername)
    {
        turnText.text =  playerUsername + "'s turn!";

        // Set the initial position of the text on the left (adjust the value based on your setup)
        turnText.rectTransform.anchoredPosition = new Vector2(-Screen.width, turnText.rectTransform.anchoredPosition.y);

        // Create a sequence for the animations
        Sequence sequence = DOTween.Sequence();

        // Move the text to the center
        sequence.Append(turnText.rectTransform.DOAnchorPos(Vector2.zero, 0.5f));  // Move to center over 1 second

        // Wait for 2 seconds
        sequence.AppendInterval(1.5f);

        // Move the text to the right (adjust the value based on your setup)
        sequence.Append(turnText.rectTransform.DOAnchorPos(new Vector2(Screen.width, turnText.rectTransform.anchoredPosition.y), 0.5f));  // Move to right over 1 second

        // Optional: add a callback after the animation is done
        sequence.OnComplete(() =>
        {
            turnText.rectTransform.anchoredPosition = new Vector2(-Screen.width, turnText.rectTransform.anchoredPosition.y);

            // Code to execute after the animation is complete, if any
        });
    }



}
