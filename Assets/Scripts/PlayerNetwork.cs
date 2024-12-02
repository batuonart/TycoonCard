using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine;
using static DeckManager.Card;
using static DeckManager;
using System;
using System.Linq;
using DG.Tweening;
using static GameManager;
public class PlayerNetwork : NetworkBehaviour
{

    DeckManager deckManager;
    public NetworkPlayerChecker checker;
    public List<Card> playerCards;
     Card[] playerCardsAsArray;
    public List<GameObject> selectedCards;
    int selectedRanks = 0;
        Vector2 cardOriginPos = new Vector2();
    public float moveDuration = 0.4f;



    void Start()
    {

        playerCards = new List<Card>();
    }

    public void ReceiveDeck(Card[] deck)
    {
        if (IsOwner)
        {
            playerCardsAsArray = deck;
            playerCards = playerCardsAsArray.ToList<Card>();
            deckManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<DeckManager>();
            deckManager.DisplayHand(playerCards);
        }
    }





}
