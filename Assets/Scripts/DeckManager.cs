using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
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

    public GameObject handPivot;



    void Start()
    {
        currentDeck = GenerateDeck();
        Shuffle(currentDeck);
        GenerateNewHand();
    }

    

    void GenerateNewHand()
    {
        List<Card> newHand = currentDeck.GetRange(0, 13);

        newHand.Sort((card1, card2) => card1.CardRank.CompareTo(card2.CardRank));
        float offSet = 0f;
        foreach (var card in newHand)
        {
            offSet++;
            GameObject newCard = Instantiate(cardPrefab, new Vector2(handPivot.transform.position.x + (offSet * 0.75f), handPivot.transform.position.y), Quaternion.identity);
            newCard.GetComponent<CardData>().setCardSuit(card.CardSuit.ToString());
            newCard.GetComponent<CardData>().setCardRank(card.CardRank.ToString());
            newCard.GetComponentInChildren<SpriteRenderer>().sprite = card.CardArt;

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
                    Debug.Log("Added" + suit.ToString() + " " + rank.ToString());
                    deck.Add(new Card(suit, rank, cardFaces[i]));
                    i++;
                }          
            }
        }

        deck.Add(new Card(Suit.Diamonds, Rank.Joker, jokerSprite));
        deck.Add(new Card(Suit.Diamonds, Rank.Joker, jokerSprite));


        return deck;
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
