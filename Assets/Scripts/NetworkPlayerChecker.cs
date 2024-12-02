using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static DeckManager;

public class NetworkPlayerChecker : NetworkBehaviour
{

    public List<List<Card>> playerDecks = new List<List<Card>>();

    private PlayerNetwork[] players; // All players in the game

    public List<ulong> playerOrder = new List<ulong>();

    public event Action OnHandReady;

    int maxPlayers = 2;




    DeckManager deckManager = new DeckManager();
    int connectedPlayers = 0;



    private void Start()
    {
        Debug.Log("in Start");
       var  playerOrder = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

    }

    private void OnClientConnected(ulong clientId)
    {
        connectedPlayers++;
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Host connected with id: " + clientId);
        }
        else
        {
            Debug.Log("Client connected with id: " + clientId);
        }
        playerOrder.Add(clientId);

        if (connectedPlayers == maxPlayers && NetworkManager.Singleton.IsHost)
        {
            Debug.Log("All players are met, starting...");
            DistributeCards();
        }
    }


    private void OnClientDisconnected(ulong clientId)
    {
        connectedPlayers--;
        playerOrder.Remove(clientId);
        Debug.Log("Client disconnected: " + clientId);
    }

    private void DistributeCards()
    {
        players = FindObjectsOfType<PlayerNetwork>();

        playerDecks = deckManager.HandlePlayerCards();
        Debug.Log("Decks created");

        for (int i = 0; i < maxPlayers; i++)
        {
            // Convert List<Card> to Card[]
            Card[] playerDeckArray = playerDecks[i].ToArray();

            // Assign deck on the server for reference

            // Send each player's deck to them specifically
            SendDeckToClientRpc(playerDeckArray, players[i].OwnerClientId);
        }
    }

    // ClientRpc to notify each client of their deck, using array
    [ClientRpc]
    private void SendDeckToClientRpc(Card[] deck, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            PlayerNetwork player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>();
            player.ReceiveDeck(deck);
        }
    }


    void FreezeGame()
    {
        if (Time.timeScale != 0)
        {
            Time.timeScale = 0;
            Debug.Log("Game is frozen. Waiting for players...");
        }
    }
    void ResumeGame()
    {
        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            Debug.Log("All players connected. Resuming the game...");
        }
    }
}
