using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DeckManager;
using UnityEngine.XR;
using System.Linq;

public class BotManager : MonoBehaviour
{

    public class Bot
    {
        public int Id { get; private set; }

        public List<DeckManager.Card> Hand { get; private set; }   

        public Bot (int id, List<DeckManager.Card> hand)
        {
            Id = id;
            Hand = hand;
        }

        int i = 0;

        public List<DeckManager.Card> PlayTurn(GameManager.TurnInfo turnInfo)
        {
            //PLAYLOGIC

            //PLAY OR SKIP
            i++;

            var r = new List<DeckManager.Card>();
            r.Add(Hand[i]);

            return r;
        }
    }
    List<Bot> bots;
    DeckManager deckManager;
    GameManager gameManager;

    private void Start()
    {
        deckManager = GetComponent<DeckManager>();
        gameManager = GetComponent<GameManager>();
        // BOT CHECK
        for (int i = 1; i < 4;  i++)
        {
           // var bot = new Bot(i, deckManager.GetHandById(i));
            //bots.Add(bot);
        }
    }

   public void PlayBotById(int id, GameManager.TurnInfo turnInfo)
    {
        var botPlayedHand = new List<DeckManager.Card>();
        botPlayedHand = bots[id].PlayTurn(turnInfo);
        gameManager.playOtherPlayerCards(botPlayedHand, id);
    }
}
