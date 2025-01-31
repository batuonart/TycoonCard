using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DeckManager;

public class HandManager : MonoBehaviour
{

    List<GameObject> cardsInHand = new List<GameObject>();
    public GameObject cardPrefab;
    public Sprite[] cardFaces;
    public Sprite jokerSprite;
    public GameObject handPivot;
    float darkenFactor = 0.4f;
    Color originalColor;
    int sortOrder = 0;
    public GameObject[] handPositions;
    public GameManager gameManager;

    private void Start()
    {
    }


    public void DarkenCards (int topValue = 100)
    {

        foreach (var card in cardsInHand)
        {
            card.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            if (card.GetComponent<CardData>().cardRank <= topValue)
            {                    
                card.GetComponentInChildren<SpriteRenderer>().color = new Color(darkenFactor,darkenFactor,darkenFactor, 1); 
            }
        }
    }

    public void DegrayCards()
    {
        foreach (var card in cardsInHand)
        {
            card.GetComponentInChildren<SpriteRenderer>().color = originalColor;
        }
    }

    public void OrganizeHand(int currentTopRank)
    {
        //darken

        foreach (var card in cardsInHand)
        {
            SpriteRenderer spriteRenderer = card.GetComponentInChildren<SpriteRenderer>();

            card.GetComponent<BoxCollider2D>().enabled = true;
            spriteRenderer.color = Color.white;

            if (card.GetComponent<CardData>().cardRank <= currentTopRank)
            {
                card.GetComponent<BoxCollider2D>().enabled = false;
                spriteRenderer.color = new Color(spriteRenderer.color.r * 0.5f, spriteRenderer.color.g * 0.5f, spriteRenderer.color.b * 0.5f, spriteRenderer.color.a);
            }
        }

        //disable
    }
    public void DisplayHand(List<Card> playerCards)
    {
        var newHand = playerCards;
        newHand.Sort((card1, card2) => card1.Rank.CompareTo(card2.Rank));
        int i = 0;
        float offSet = 0f;
        foreach (var card in newHand)
        {
            sortOrder++;

            GameObject newCard = Instantiate(cardPrefab, new Vector2(handPivot.transform.position.x + (offSet * 1.2f), handPivot.transform.position.y), Quaternion.identity, this.transform);
            newCard.GetComponent<CardData>().ConvertToCardData(card);
            
            
            newCard.name = (card.Rank + 3).ToString() + " of " + card.Suit.ToString();

            if (newCard.GetComponent<CardData>().isJoker)
            {
                newCard.GetComponentInChildren<SpriteRenderer>().sprite = jokerSprite;
            }
            else
            {
                newCard.GetComponentInChildren<SpriteRenderer>().sprite = cardFaces[card.SpriteId];
            }
            newCard.GetComponentInChildren<SpriteRenderer>().sortingOrder = sortOrder;
            offSet++;
            cardsInHand.Add(newCard);
            i++;
        }
        originalColor = cardsInHand[0].GetComponentInChildren<SpriteRenderer>().color;
    }

    public void PlayTurnWith(Card[] cards)
    {
        var c = new List<GameObject>();
        foreach (var card in cards)
        {
            sortOrder++;
            GameObject newCard = Instantiate(cardPrefab, handPositions[0].transform.position, Quaternion.identity, this.transform);
            var newCardData = newCard.GetComponent<CardData>();
            newCardData.ConvertToCardData(card);

            newCard.name = card.Rank.ToString() + " of " + card.Suit.ToString();

            if (newCardData.isJoker)
            {
                newCard.GetComponentInChildren<SpriteRenderer>().sprite = jokerSprite;
            }
            else
            {
                newCard.GetComponentInChildren<SpriteRenderer>().sprite = cardFaces[card.SpriteId];
            }

            c.Add(newCard);

        }
        gameManager.PlaySelectedCardsAsGO(c);
        c.Clear();

    }
}
