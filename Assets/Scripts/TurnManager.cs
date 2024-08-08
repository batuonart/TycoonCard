
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static DeckManager.Card;
using UnityEngine.Events;
using System.Collections.Generic;


public class TurnManager : MonoBehaviour
{
    int currentPlayerNo = 0;
    int turnCount = 0;
    int turnPlayStyle = 0;
    public int currentTopCard { get; private set; }
    bool gameIsReversed = false;
    
    GameManager gameManager;
    DeckManager deckManager;
    
    public TextMeshProUGUI turnText;
    public UnityEvent newTurnEvent;

    public class Player
    {
        public int Id { get; private set; }
        public string Username { get; private set; }
        public int Score { get; private set; }
        public int LastRoundPlace { get; private set; }

        public Player(int id, string username )
        {
            Id = id;
            Username = username;
            Score = 0;
            LastRoundPlace = 0;
        }
        
    }
    List<Player> playerList;

    void Start()
    {
        currentTopCard = 0;

        deckManager = GetComponent<DeckManager>();
        gameManager = GetComponent<GameManager>();
        playerList = new List<Player>();        

        Player p1 = new Player(1, "alex");
        Player p2 = new Player(2, "bard");
        Player p3 = new Player(3, "carian");
        Player p4 = new Player(4, "dublo");
        playerList.Add(p1);
        playerList.Add(p2);
        playerList.Add(p3);
        playerList.Add(p4);

        StartGame(p1);
    }

    public void StartGame(Player p1)
    {
        //el aktif gözükecek
        StartTurn(p1);
    }

    public void StartTurn(Player player)
    {
        AnnounceTurn(player.Username);
        deckManager.OrganizeHand(currentTopCard);
    }

    void StartNewTurn()
    {
        currentTopCard = 0;
        turnPlayStyle = 0;
        StartTurn(playerList[0]);
        gameManager.CleanCardsOnTable();
        Debug.Log("new turn started!");
    }

    void IncrementTurnCount()
    {
        turnCount++;
        if(turnCount == 4) //4 turns have been played
        {
            turnCount = 0;
            StartNewTurn();
        }
        else
        {
            currentPlayerNo++;
            currentPlayerNo = currentPlayerNo % 4;
            StartTurn(playerList[currentPlayerNo]);
        }
    }

    public void SkipTurn()
    {
        IncrementTurnCount();
    }

    public void PlayTurn(GameManager.TurnInfo turnInfo)
    {
        currentTopCard = turnInfo.CurrentTopCard;
        turnPlayStyle = turnInfo.CurrentPlayStyle;
        Debug.Log("current top card:"+  currentTopCard +"current play style: " + turnPlayStyle );
        if (turnInfo.IsReversed) gameIsReversed = !gameIsReversed;
        IncrementTurnCount();
    }





    public void AnnounceTurn(string playerUsername)
    {
        turnText.text =  playerUsername + "'s turn!";

        // Set the initial position of the text on the left (adjust the value based on your setup)
        turnText.rectTransform.anchoredPosition = new Vector2(-Screen.width, turnText.rectTransform.anchoredPosition.y);

        // Create a sequence for the animations
        Sequence sequence = DOTween.Sequence();

        // Move the text to the center
        sequence.Append(turnText.rectTransform.DOAnchorPos(Vector2.zero, 0.5f));  // Move to center over 1 second

        // Wait for 2 seconds
        sequence.AppendInterval(1.5f);

        // Move the text to the right (adjust the value based on your setup)
        sequence.Append(turnText.rectTransform.DOAnchorPos(new Vector2(Screen.width, turnText.rectTransform.anchoredPosition.y), 0.5f));  // Move to right over 1 second

        // Optional: add a callback after the animation is done
        sequence.OnComplete(() =>
        {
            turnText.rectTransform.anchoredPosition = new Vector2(-Screen.width, turnText.rectTransform.anchoredPosition.y);

            // Code to execute after the animation is complete, if any
        });
    }



}
