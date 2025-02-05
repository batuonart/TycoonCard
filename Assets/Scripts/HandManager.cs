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

    public NetworkPlayerChecker networkPlayerChecker;

    private void Start()
    {
        var GMObject = GameObject.FindWithTag("GameController");
        networkPlayerChecker = GMObject.GetComponent<NetworkPlayerChecker>();
        gameManager = GMObject.GetComponent<GameManager>();
        networkPlayerChecker.OnStartTurn += OnStartTurn;
        networkPlayerChecker.OnNewRound += UnDarkenCards;
    }

    void OnStartTurn(bool canPlay, int topValue, int playStyle)
    {
        
        if (canPlay) 
        { 
            DarkenCards(topValue); 
        }
        else { 
            DarkenCards();
        }

    }

    public void DarkenCards (int minRank = 100)
    {
        foreach (var card in cardsInHand)
        {
            if (card.GetComponent<CardData>().cardRank <= minRank)
            {                    
                card.GetComponentInChildren<SpriteRenderer>().color = new Color(darkenFactor,darkenFactor,darkenFactor, 1); 
            }
        }
    }
    public void UnDarkenCards ()
    {
        foreach (var card in cardsInHand)
        {
            //card.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1, 1);
        }
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
            GameObject cardAsObject = Instantiate(cardPrefab, handPositions[0].transform.position, Quaternion.identity, this.transform);
            var newCardData = cardAsObject.GetComponent<CardData>();
            newCardData.ConvertToCardData(card);

            cardAsObject.name = card.Rank.ToString() + " of " + card.Suit.ToString();

            if (newCardData.isJoker)
            {
                cardAsObject.GetComponentInChildren<SpriteRenderer>().sprite = jokerSprite;
            }
            else
            {
                cardAsObject.GetComponentInChildren<SpriteRenderer>().sprite = cardFaces[card.SpriteId];
            }
            c.Add(cardAsObject);

        }
        gameManager.PlaySelectedCardsAsGO(c);
        c.Clear();
    }
}
