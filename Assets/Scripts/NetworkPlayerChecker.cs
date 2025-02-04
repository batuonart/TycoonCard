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

    int currentTopValue = 0;
    int currentPlayStyle = 0;

    bool canPlay = false;

    public event Action<bool, int, int> OnStartTurn;

    public bool CanPlay => canPlay;

    int currentPlayerNo = -1;


    DeckManager deckManager;
    int connectedPlayers = 0;
    HandManager handManager;
    


    public override void OnNetworkSpawn()
    {
        Debug.Log("spawned");
        deckManager = FindObjectOfType<DeckManager>();
        handManager = FindObjectOfType<HandManager>();
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


    [ClientRpc]
    private void SendDeckToClientRpc(Card[] deck, ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        if (NetworkManager.Singleton.LocalClientId == clientId)
        {
            PlayerNetwork player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>();
            player.ReceiveDeck(deck);
        }
    }

  
    [ServerRpc(RequireOwnership = false)]
    public void EndTurnServerRpc(Card[] cards, ServerRpcParams rpcParams = default)
    {
        Debug.Log(currentPlayerNo + "'s turn ended from HOST/SERVER");

        var clientId = rpcParams.Receive.SenderClientId;
        currentTopValue = cards[0].Rank;
        currentPlayStyle = cards.Length;

        ShowPlayAnimationClientRpc(cards, clientId);
        NextTurn();
    }
    void NextTurn()
    {
        currentPlayerNo++;

        if(currentPlayerNo == maxPlayers) { currentPlayerNo = 0; }
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { playerIds[currentPlayerNo] }
            }
        };
        StartTurnClientRpc(currentTopValue, currentPlayStyle, clientRpcParams);
    }

    [ClientRpc]
    private void StartTurnClientRpc(int  topVal, int playStyle, ClientRpcParams clientRpcParams = default)
    {
        Debug.Log(currentPlayerNo + "'s turn started");
        PlayerNetwork player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>();
        EnableCanPlay();
        player.StartTurn();

        canPlay = true;
        Debug.Log("canPlay enabled.");
        OnStartTurn?.Invoke(canPlay, topVal, playStyle);
    }

    [ClientRpc]
    public void ShowPlayAnimationClientRpc(Card[] cards, ulong playerWhoPlayed)
    {
        if (NetworkManager.Singleton.LocalClientId == playerWhoPlayed)
            return;

        Debug.Log("anim called from " + playerWhoPlayed);
        handManager.PlayTurnWith(cards);
    }

    public void EnableCanPlay()
    {

    }

    public void DisableCanPlay()
    {
        canPlay = false;
        Debug.Log("canPlay disabled.");
        OnStartTurn?.Invoke(canPlay, currentTopValue, currentPlayStyle);
    }


}
