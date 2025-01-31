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

    public List<ulong> playerIds = new List<ulong>();

    public event Action OnHandReady;

    int maxPlayers = 2;

    int currentPlayerNo = 0;

    private NetworkVariable<int> currentTopValue = new NetworkVariable<int>(0);
    public int CurrentTopValue => currentTopValue.Value;




    DeckManager deckManager = new DeckManager();
    int connectedPlayers = 0;
    HandManager handManager;
    GameManager gameManager;


    public override void OnNetworkSpawn()
    {
        gameManager = GetComponent<GameManager>();
        handManager = FindObjectOfType<HandManager>();
        Debug.Log("Manager spawned");
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
        playerIds.Add(clientId);

        if (connectedPlayers == maxPlayers && NetworkManager.Singleton.IsHost)
        {
            Debug.Log("All players are met, starting...");
            DistributeCards();
        }
    }


    private void OnClientDisconnected(ulong clientId)
    {
        connectedPlayers--;
        playerIds.Remove(clientId);
        Debug.Log("Client disconnected: " + clientId);
    }

    private void DistributeCards()
    {
        players = FindObjectsOfType<PlayerNetwork>();

        playerDecks = deckManager.HandlePlayerCards();  
        Debug.Log("Decks created");

        for (int i = 0; i < maxPlayers; i++)
        {
            Card[] playerDeckArray = playerDecks[i].ToArray();
            SendDeckToClientRpc(playerDeckArray, players[i].OwnerClientId);
        }

        NextTurn();
    }

    void NextTurn()
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { playerIds[currentPlayerNo] }
            }
        };
        StartTurnClientRpc();
    }

    [ClientRpc]
    private void SendDeckToClientRpc(Card[] deck, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            PlayerNetwork player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>();
            player.ReceiveDeck(deck);
        }
    }

    [ClientRpc]
    private void StartTurnClientRpc(ClientRpcParams clientRpcParams = default)
    {
        
        PlayerNetwork player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>();
        player.StartTurn();
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRPc(Card[] cards, ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        currentPlayerNo++;
        Debug.Log(currentPlayerNo + "'s turn");

        currentTopValue.Value = cards[0].Rank;

        Debug.Log("Top card: " + cards[0].Rank + " in style: " + cards.Length);


        ShowPlayAnimationClientRpc(cards, clientId);

        if (currentPlayerNo == maxPlayers) currentPlayerNo = 0;
        NextTurn();
    }

    [ClientRpc]
    public void ShowPlayAnimationClientRpc(Card[] cards, ulong playerWhoPlayed)
    {
        if (NetworkManager.Singleton.LocalClientId == playerWhoPlayed)
            return;

        Debug.Log("anim called from " + playerWhoPlayed);
        handManager.PlayTurnWith(cards);
    }

}
