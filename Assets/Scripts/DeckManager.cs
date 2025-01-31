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
    public Sprite[] cardFaces;
    readonly string[] suitList = { "Clubs", "Diamonds", "Hearts", "Spades" };

  
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
    public List<List<Card>> playerHands;
    int giveExtraCardsTo = 0;

    void Start()
    {
        List<GameObject> currentHandAsObjects = new List<GameObject>();
    }

    public List<List<Card>> HandlePlayerCards()
    {
        currentDeck = GenerateDeck();
        Shuffle(currentDeck);
        var splitDeck = SplitHands();
        return splitDeck;
    }

    public  List<Card> GenerateDeck()
    {
        List<Card> deck = new List<Card>();
        int spriteId= 0;

        for (int rank = 3; rank < 16; rank++)
        {
            foreach (string suit in suitList)
            {
                deck.Add(new Card(suit, rank, spriteId));
                spriteId++;
            }
        }
        //TODO: add jokersprite
        deck.Add(new Card("Joker", 17, 0));
        deck.Add(new Card("Joker", 17, 0));

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
