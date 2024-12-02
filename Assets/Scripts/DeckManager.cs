using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using Unity.VisualScripting;    
using UnityEngine;
using UnityEngine.UI;
using static DeckManager;
using static DeckManager.Card;

public class DeckManager : MonoBehaviour
{

    int playerHand = 13;

    public GameObject cardPrefab;
    public Sprite[] cardFaces;
    readonly string[] suitList = { "Clubs", "Diamonds", "Spades", "Hearts" };
    public Sprite jokerSprite;
  
    private static readonly System.Random rng = new System.Random();
    public class Card : INetworkSerializable
    {
        public string Suit;
        public int Rank; // 11 J, 12 Q, 13 K, 14 A, 15 "2", 16 Joker
        public int SpriteId;

        public Card(string suit, int rank, int spriteId)
        {
            Suit = suit;
            Rank = rank;
            SpriteId = spriteId;
        }

        public Card() { }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Suit);
            serializer.SerializeValue(ref Rank);
            serializer.SerializeValue(ref SpriteId);
        }



    }

    List<Card> currentDeck;
    List<GameObject> handAsObjects;
    public List<List<Card>> playerHands;

    public GameObject handPivot;

    int giveExtraCardsTo = 0;

    void Start()
    {
        List<GameObject> currentHandAsObjects = new List<GameObject>();
       // currentDeck = GenerateDeck();
        //Shuffle(currentDeck);
        //SplitHands();
    }

    public void GrayLowerCards(int topValue)
    {
        foreach (var card in handAsObjects)
        {   
            if(card.GetComponent<CardData>().cardRank <= topValue)
            {
                Color originalColor = card.GetComponentInChildren<SpriteRenderer>().color;

                // Calculate the grayscale value (average of R, G, and B components)
                float grayValue = (originalColor.r + originalColor.g + originalColor.b) / 3f;

                // Set the color to a gray tone with the same alpha value
                card.GetComponentInChildren<SpriteRenderer>().color = new Color(grayValue, grayValue, grayValue, originalColor.a);
            }
        }
    }
    

    public void OrganizeHand(int currentTopRank)
    {
        //darken

        foreach(var card in handAsObjects)
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


    public List<List<Card>> HandlePlayerCards()
    {
        currentDeck = GenerateDeck();
        Shuffle(currentDeck);
        var splitDeck = SplitHands();
        return splitDeck;
    }

    public void DisplayHand(List<Card> playerCards)
    {
        handAsObjects = new List<GameObject>();
        var newHand = playerCards;
        newHand.Sort((card1, card2) => card1.Rank.CompareTo(card2.Rank));
        int i = 0;
        float offSet = 0f;
        foreach (var card in newHand)
        {
            
            GameObject newCard = Instantiate(cardPrefab, new Vector2(handPivot.transform.position.x + (offSet * 1.2f), handPivot.transform.position.y), Quaternion.identity);
            newCard.GetComponent<CardData>().cardSuit = card.Suit;
            newCard.GetComponent<CardData>().cardRank = card.Rank;
            if(card.Suit.Equals("Joker"))
            {
                newCard.GetComponentInChildren<SpriteRenderer>().sprite = jokerSprite;
            }
            else
            {
                newCard.GetComponentInChildren<SpriteRenderer>().sprite = cardFaces[card.SpriteId];
            }
            newCard.GetComponentInChildren<SpriteRenderer>().sortingOrder = (int)offSet;
            offSet++;
            handAsObjects.Add(newCard);
            i++;
        }
    }

    public  List<Card> GenerateDeck()
    {
        List<Card> deck = new List<Card>();
        int spriteId= 0;

        for (int rank = 0; rank < 13; rank++)
        {
            foreach (string suit in suitList)
            {
                deck.Add(new Card(suit, rank, spriteId));
                spriteId++;
            }
        }
        //TODO: add jokersprite
        deck.Add(new Card("Joker", 13, 0));
        deck.Add(new Card("Joker", 13, 0));

        return deck;
    }

    public  List<List<Card>> SplitHands()
    {
         playerHands = new List<List<Card>> {
            currentDeck.GetRange(0, 13),
            currentDeck.GetRange(13, 13),
            currentDeck.GetRange(26, 13),
            currentDeck.GetRange(39, 13)
        };

        playerHands[giveExtraCardsTo].Add(currentDeck[52]);
        playerHands[giveExtraCardsTo + 1].Add(currentDeck[53]);

        giveExtraCardsTo++;
        giveExtraCardsTo = giveExtraCardsTo % 4;

        return playerHands;
    }

    public static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
 }
