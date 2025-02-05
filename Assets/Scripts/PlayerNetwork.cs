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

    HandManager handManager;
    public NetworkPlayerChecker checker;
    public List<Card> playerCards = new List<Card>();
    Card[] playerCardsAsArray;


    public override void OnNetworkSpawn()
    {
        handManager = GameObject.FindGameObjectWithTag("Hand").GetComponent<HandManager>();
        checker = GameObject.FindGameObjectWithTag("GameController").GetComponent<NetworkPlayerChecker>();

    }

    public void ReceiveDeck(Card[] deck)
    {
        if (IsOwner)
        {
            playerCardsAsArray = deck;
            playerCards = playerCardsAsArray.ToList();
            handManager.DisplayHand(playerCards);
        }
    }


    public void EndTurn(Card[] playedCards)
    {
        checker.DisableCanPlay();
        checker.EndTurnServerRpc(playedCards);
    }







}
