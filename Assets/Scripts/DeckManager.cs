using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DeckManager;
using static DeckManager.Card;

public class DeckManager : MonoBehaviour
{

    int playerHand = 13;

    public GameObject cardPrefab;
    public Sprite[] cardFaces;
  
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
            Two = 2,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Ace,
            Jack,
            Queen,
            King
            
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
        GenerateNewHand();
    }

    void GenerateNewHand()
    {
        List<Card> newHand = currentDeck.GetRange(0, 13);

        for (int i = 0; i < 13; i++)
        {
            var card = newHand[i];
            GameObject newCard = Instantiate(cardPrefab, new Vector2(handPivot.transform.position.x + (i * 0.75f), handPivot.transform.position.y), Quaternion.identity);
            newCard.GetComponent<CardData>().setCardSuit(card.CardSuit.ToString());
            newCard.GetComponent<CardData>().setCardRank(card.CardRank.ToString());
            newCard.GetComponentInChildren<SpriteRenderer>().sprite = currentDeck[i].CardArt;

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
                Debug.Log("Added" + suit.ToString() + " " + rank.ToString());
                deck.Add(new Card(suit, rank, cardFaces[i]));
                i++;
            }
        }

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
