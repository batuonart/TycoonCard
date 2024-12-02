using DG.Tweening;
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

    public GameObject cardOriginObject;
    Vector2 cardOriginPos = new Vector2();
    public float moveDuration = 0.4f;

    bool cardsHaveMoved = false;

    TurnManager turnManager;
    public List<GameObject> selectedCards;
    int i = 1;
    int selectedRanks = 0;

    int topCardRank = 0;
    int curStyle = -1;

    List<GameObject> cardsOnTable;

    public struct TurnInfo
    {
        public int CurrentTopCard { get; private set; }
        public int CurrentPlayStyle { get; private set; }
        public bool IsReversed { get; private set; }

        public TurnInfo(int currentTopCard, int currentPlayStyle, bool isReversed)
        {
            CurrentTopCard = currentTopCard;
            CurrentPlayStyle = currentPlayStyle;
            IsReversed = isReversed;
        }
    }

    private void Start()
    {
        cardsOnTable = new List<GameObject>(); // Initialize the list here
        turnManager = gameObject.GetComponent<TurnManager>();
        cardOriginPos = cardOriginObject.transform.position;
    }

    private void Update()
    {
        if (selectedCards.Count == curStyle || (turnManager.isFirstTurn() && selectedCards.Count > 0))
        {
            playButton.interactable = true;
        }
        else
        {
            playButton.interactable = false;

        }
    }

    public async void CleanCardsOnTable()
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
  

        if (selectedCards.Contains(newCard))
        {
            removeFromSelectedCards(newCard);
        }
        else
        {
            if (selectedCards.Count == 0)
            {        
                addToSelectedCards(newCard);
                if(newCard.GetComponent<CardData>().cardRank != 16)
                {
                    selectedRanks = newCard.GetComponent<CardData>().cardRank;
                }
            }
            else
            {

                if (selectedRanks == newCard.GetComponent<CardData>().cardRank ||
                    newCard.GetComponent<CardData>().cardRank == 16)
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

        curStyle = selectedCards.Count;
        bool isReversed = (curStyle == 4);
        
        foreach (GameObject selectedCard in selectedCards)
        {
            i++;
            float randomRotation = Random.Range(-10f, 10f); // Adjust the range as needed
            selectedCard.GetComponentInChildren<Renderer>().sortingOrder = 20 + i;
            selectedCard.GetComponent<CardData>().disableCard();
            cardsOnTable.Add(selectedCard);
            selectedCard.transform.DORotate(new Vector3(0, 0, randomRotation), moveDuration).SetEase(Ease.InOutQuad);
            selectedCard.transform.DOMove(cardOriginPos, moveDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                });
        }
        turnManager.PlayTurn(new TurnInfo(selectedCards[0].gameObject.GetComponent<CardData>().cardRank, curStyle, isReversed));
        selectedCards.Clear();
    }

    public void playOtherPlayerCards(List<DeckManager.Card> otherCards, int id)
    {
        foreach (DeckManager.Card card in otherCards)
        {
            /*
            //INSTANTIATE
            cardsOnTable.Add(selectedCard);
            selectedCard.transform.DORotate(new Vector3(0, 0, randomRotation), moveDuration).SetEase(Ease.InOutQuad);
            selectedCard.transform.DOMove(cardOriginPos, moveDuration)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                });
            */
        }
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
