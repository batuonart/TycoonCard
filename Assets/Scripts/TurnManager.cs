
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static DeckManager.Card;


public class TurnManager : MonoBehaviour
{
    int playerNo;
    int currentTurn = 0;
    int turnPlayStyle = 0;
    int currentCardRank = 0;

    public TextMeshProUGUI turnText;

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



void Start()
    {
        Player p1 = new Player(1, "alex");
        Player p2 = new Player(2, "bard");
        Player p3 = new Player(3, "carian");
        Player p4 = new Player(4, "dublo");

        AnnounceTurn(p1.Username);
    }

    // Update is called once per frame
    void Update()
    {
        
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
            // Code to execute after the animation is complete, if any
        });
    }



}
