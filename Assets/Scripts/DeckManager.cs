using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using static DeckManager;
using static DeckManager.Card;

public class DeckManager : MonoBehaviour
{

    int playerHand = 13;

    public GameObject cardPrefab;
    public Sprite[] cardFaces;
    public Sprite jokerSprite;
  
    private static readonly System.Random rng = new System.Random();
    public class Card
    {
        public enum Suit
        {
            Clubs,
            Diamonds,
            Hearts,
            Spades
        }
        public enum Rank
        {
            Three = 3,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace,
            Two,
            Joker
            
        }
        public Suit CardSuit { get; private set; }
        public Rank CardRank { get; private set; }
        public Sprite CardArt { get; private set; }

        public Card(Suit suit, Rank rank, Sprite art)
        {
            CardSuit = suit;
            CardRank = rank;
            this.CardArt = art;
        }



    }

    List<Card> currentDeck;
    List<GameObject> handAsObjects;
    List<List<Card>> playerHands;

    public GameObject handPivot;

    int giveExtraCardsTo = 0;

    void Start()
    {
        List<GameObject> currentHandAsObjects = new List<GameObject>();
        currentDeck = GenerateDeck();
        Shuffle(currentDeck);
        SplitHands();
        DisplayHand(0);
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



    void DisplayHand(int playerNo)
    {
        handAsObjects = new List<GameObject>();
        var newHand = playerHands[playerNo];
        newHand.Sort((card1, card2) => card1.CardRank.CompareTo(card2.CardRank));
        float offSet = 0f;
        foreach (var card in newHand)
        {
            
            GameObject newCard = Instantiate(cardPrefab, new Vector2(handPivot.transform.position.x + (offSet * 1.2f), handPivot.transform.position.y), Quaternion.identity);
            newCard.GetComponent<CardData>().cardSuit = card.CardSuit.ToString();
            newCard.GetComponent<CardData>().cardRank = (int)card.CardRank;
            newCard.GetComponentInChildren<SpriteRenderer>().sprite = card.CardArt;
            newCard.GetComponentInChildren<SpriteRenderer>().sortingOrder = (int)offSet;
            offSet++;
            handAsObjects.Add(newCard);  

        }
    }

    List<Card> GenerateDeck()
    {
        List<Card> deck = new List<Card>();
        int i = 0;

        foreach (Rank rank in Enum.GetValues(typeof(Rank)))
        {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                if (rank != Rank.Joker)
                {
                    deck.Add(new Card(suit, rank, cardFaces[i]));
                    i++;
                }          
            }
        }

        deck.Add(new Card(Suit.Diamonds, Rank.Joker, jokerSprite));
        deck.Add(new Card(Suit.Diamonds, Rank.Joker, jokerSprite));


        return deck;
    }

    void SplitHands()
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
