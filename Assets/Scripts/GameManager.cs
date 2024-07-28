using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Button playButton;

    public GameObject cardOriginObject;
    Vector2 cardOriginPos = new Vector2();
    public float moveDuration = 0.4f;



    public List<GameObject> selectedCards;

    public GameObject currentTopCard;
    public int currentPlayStyle = 0;

    private void Start()
    {
        cardOriginPos = cardOriginObject.transform.position;
    }

    private void Update()
    {
        if (selectedCards.Count > 0)
        {
            playButton.interactable = true;
        }
        else
        {
            playButton.interactable = false;

        }
    }


    public void checkNewCard(GameObject gameObject)
    {
        if(selectedCards.Contains(gameObject))
        {
            removeFromSelectedCards(gameObject);
        }
        else
        {
            if (selectedCards.Count == 0)
            {
                addToSelectedCards(gameObject);
            }
            else
            {
                if (selectedCards[0].GetComponent<CardData>().getCardRank() == gameObject.GetComponent<CardData>().getCardRank()) addToSelectedCards(gameObject);
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

    void printLogs()
    {
        Debug.Log("Played hand of " + currentPlayStyle + ", top card:");
        currentTopCard.GetComponent<CardData>().printCardData();

        foreach (GameObject gameObject in selectedCards)
        {
            Debug.Log(gameObject.name);
        }
    }

    public void playSelectedCards()
    {
        currentPlayStyle = selectedCards.Count;
        currentTopCard = selectedCards[0];

        printLogs();
        foreach (GameObject selectedCard in selectedCards)
        {
            selectedCard.transform.DOMove(cardOriginPos, moveDuration).SetEase(Ease.InOutQuad);
            selectedCard.GetComponent<Draggable>().disableHovering();
        }

        selectedCards.Clear();
    }

}
