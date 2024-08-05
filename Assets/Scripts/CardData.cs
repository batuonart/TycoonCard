using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{
    [SerializeField]
    private string cardSuit;
    [SerializeField]
    private string cardRank;

    private bool isActive = true;

    public void disableCard()
    {
        isActive = false;   
    }

    public bool cardIsActive()
    {
        return isActive;
    }

    public void setCardSuit(string cardSuit)
    {
        this.cardSuit = cardSuit;
    }
    public void setCardRank(string cardRank)
    {
        this.cardRank = cardRank;
    }
    public string getCardSuit()
    {
        return this.cardSuit;
    }
    public string getCardRank()
    {
        return this.cardRank;
    }

    

    public void printCardData()
    {
        Debug.Log(this.cardRank + " of " + this.cardSuit);
    }
}
