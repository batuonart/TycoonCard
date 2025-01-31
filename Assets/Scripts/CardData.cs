using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardData : MonoBehaviour
{

    [SerializeField]
    public string cardSuit { get;  set; }
    [SerializeField]
    public int cardRank { get; set; }


    public DeckManager.Card rawCard{ get;  set; }

    private bool isActive = true;

    public bool isJoker = false;

    public void disableCard()
    {
        isActive = false;   
    }

    public void ConvertToCardData(DeckManager.Card card)
    {
        cardSuit = card.Suit;
        cardRank = card.Rank;
        rawCard = card;

        if(cardSuit == "Joker")
        {
            isJoker = true;
        }
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
