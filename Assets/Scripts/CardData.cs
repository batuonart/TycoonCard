using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    public string cardSuit { get;  set; }
    public int cardRank { get;  set; }

    private bool isActive = true;

    public void disableCard()
    {
        isActive = false;   
    }

    public bool cardIsActive()
    {
        return isActive;
    }
    public void printCardData()
    {
        Debug.Log(this.cardRank + " of " + this.cardSuit);
    }
}
