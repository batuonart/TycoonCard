using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using static DeckManager;

public class GameManager : MonoBehaviour
{
    public Button playButton;
    public Button skipButton;

    public GameObject cardOriginObject;
    Vector2 cardOriginPos = new Vector2();
    public float moveDuration = 0.4f;

    TurnManager turnManager;
    public List<GameObject> selectedCards;
    int i = 1;
    int selectedRanks = 0;

    int topCardRank = 0;
    int curStyle = -1;

    int spriteOrder = 0;

    List<GameObject> cardsOnTable;

    NetworkPlayerChecker networkPlayerChecker;


    private void Start()
    {
        cardsOnTable = new List<GameObject>(); 
        turnManager = gameObject.GetComponent<TurnManager>();
        cardOriginPos = cardOriginObject.transform.position;

        networkPlayerChecker = GetComponent<NetworkPlayerChecker>();
        networkPlayerChecker.OnStartTurn += OnStartTurn;
    }

    void OnStartTurn(bool canPlay, int topValue, int playStyle)
    {
        playButton.interactable = canPlay;
        skipButton.interactable = canPlay;
    }


    public  void CleanCardsOnTable()
    {
        
        foreach (GameObject card in cardsOnTable)
        {

            Vector3 screenRight = new Vector3(Screen.width, Screen.height / 2, 0);
            Vector3 worldRight = Camera.main.ScreenToWorldPoint(screenRight);
            Vector3 targetPosition = new Vector3(worldRight.x, card.transform.position.y, card.transform.position.z);
            card.transform.DOMove(targetPosition, moveDuration).SetEase(Ease.InOutQuad)
                .OnComplete(() => card.SetActive(false));
        }
    }


    public void checkNewCard(GameObject newCard)
    {
        var newCardData = newCard.GetComponent<CardData>();
        if (selectedCards.Contains(newCard))
        {
            removeFromSelectedCards(newCard);
        }
        else
        {
            if (selectedCards.Count == 0)
            {        
                addToSelectedCards(newCard);
                if(!newCardData.isJoker)
                {
                    selectedRanks = newCardData.cardRank;
                }
            }
            else
            {

                if (selectedRanks == newCardData.cardRank || newCardData.isJoker)
                {
                    addToSelectedCards(newCard);
                }
            }
        }
    }

    public void addToSelectedCards(GameObject gameObject)
    {
        gameObject.transform.DOMove(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f) , moveDuration).SetEase(Ease.InOutQuad);
        selectedCards.Add(gameObject);
    }
    public void removeFromSelectedCards(GameObject gameObject)
    {
        gameObject.transform.DOMove(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.5f), moveDuration).SetEase(Ease.InOutQuad);
        selectedCards.Remove(gameObject);
    }

    public void  playSelectedCards()
    {
        if(selectedCards.Count == 0)
        { return; }

        var cardList = new List<Card>();
        foreach (GameObject selectedCard in selectedCards)
        {  
            cardList.Add(selectedCard.GetComponent<CardData>().rawCard);
        }

        PlaySelectedCardsAsGO(selectedCards);

        PlayerNetwork localPlayerNetwork = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>();
        localPlayerNetwork.EndTurn(cardList.ToArray());
    }

    public void PlaySelectedCardsAsGO(List<GameObject> selectedCards)
    {

        foreach (GameObject selectedCard in selectedCards)
        {
            spriteOrder++;
            selectedCard.GetComponentInChildren<Renderer>().sortingOrder = 20 + spriteOrder;
            selectedCard.GetComponent<CardData>().disableCard();
            cardsOnTable.Add(selectedCard);


            selectedCard.transform.DORotate(new Vector3(0, 0, UnityEngine.Random.Range(-10f, 10f)), moveDuration).SetEase(Ease.InOutQuad);
            selectedCard.transform.DOMove(cardOriginPos, moveDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                });


        }
        selectedCards.Clear();
    }


    public void SkipTurn()
    {
        PlayerNetwork localPlayerNetwork = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerNetwork>();

        localPlayerNetwork.EndTurn(Array.Empty<Card>());
    }

    public void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

}
