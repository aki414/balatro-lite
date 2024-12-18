using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class HorizontalCardHolder : MonoBehaviour
{

    [SerializeField] private Card draggingCard;
    [SerializeReference] private Card hoveredCard;
    public List<Card> selectedCards;

    private RectTransform rect;



    bool isCrossing = false;
    [SerializeField] private bool tweenCardReturn = true;
    public List<Card> cards;

    public Deck deck;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        cards = new List<Card>();
        selectedCards = new List<Card>();

        StartCoroutine(CheckForNewCards());
    }
    IEnumerator CheckForNewCards()
    {
        while (true)
        {
            // Check if there are new cards
            foreach (Card card in deck.HandCards)
            {
                if (!cards.Contains(card))
                {
                    // Add new card to the list
                    cards.Add(card);

                    // Add event listeners
                    card.PointerEnterEvent.AddListener(CardPointerEnter);
                    card.PointerExitEvent.AddListener(CardPointerExit);
                    card.BeginDragEvent.AddListener(BeginDrag);
                    card.EndDragEvent.AddListener(EndDrag);
                    card.PointerUpEvent.AddListener(PointerUp);

                    // Update card name
                    card.name = cards.IndexOf(card).ToString();
                }
            }

            // Check if there are removed cards
            for (int i = cards.Count - 1; i >= 0; i--)
            {
                if (!deck.HandCards.Contains(cards[i]))
                {

                    // Remove event listeners
                    cards[i].PointerEnterEvent.RemoveListener(CardPointerEnter);
                    cards[i].PointerExitEvent.RemoveListener(CardPointerExit);
                    cards[i].BeginDragEvent.RemoveListener(BeginDrag);
                    cards[i].EndDragEvent.RemoveListener(EndDrag);
                    cards[i].PointerUpEvent.RemoveListener(PointerUp);

                    cards.RemoveAt(i);

                }
            }

            // Update card visuals
            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].cardVisual != null)
                    cards[i].cardVisual.UpdateIndex(transform.childCount);
            }

            // Wait for 0.1 seconds before checking again
            yield return new WaitForSecondsRealtime(.1f);
        }
    }

    private void BeginDrag(Card card)
    {
        draggingCard = card;
    }


    void EndDrag(Card card)
    {
        if (draggingCard == null)
            return;

        draggingCard.transform.DOLocalMove(draggingCard.selected ? new Vector3(0, draggingCard.selectionOffset, 0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        draggingCard = null;

    }

    void CardPointerEnter(Card card)
    {
        hoveredCard = card;
    }

    void CardPointerExit(Card card)
    {
        hoveredCard = null;
    }


    void PointerUp(Card card, bool longPress)
    {
        if (!longPress)
        {
            if (card.selected)
            {
                if (selectedCards.Contains(card))
                {
                    selectedCards.Remove(card);
                }
            }
            else
            {
                if (!selectedCards.Contains(card))
                {
                    selectedCards.Add(card);
                }
            }
        }
    }

    void Update()
    {


        if (Input.GetMouseButtonDown(1))
        {

            foreach (Card card in cards)
            {
                card.Deselect();
                if (selectedCards.Contains(card))
                {
                    selectedCards.Remove(card);
                }
            }
        }

        if (draggingCard == null)
            return;

        if (isCrossing)
            return;

        for (int i = 0; i < cards.Count; i++)
        {

            //swap
            if (draggingCard.transform.position.x > cards[i].transform.position.x)
            {
                if (draggingCard.ParentIndex() < cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (draggingCard.transform.position.x < cards[i].transform.position.x)
            {
                if (draggingCard.ParentIndex() > cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }

    }

    void Swap(int index)
    {
        isCrossing = true;

        Transform focusedParent = draggingCard.transform.parent;
        Transform crossedParent = cards[index].transform.parent;

        cards[index].transform.SetParent(focusedParent);
        cards[index].transform.localPosition = cards[index].selected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;
        draggingCard.transform.SetParent(crossedParent);

        isCrossing = false;

        if (cards[index].cardVisual == null)
            return;

        bool swapIsRight = cards[index].ParentIndex() > draggingCard.ParentIndex();
        cards[index].cardVisual.Swap(swapIsRight ? -1 : 1);

        //Updated Visual Indexes
        foreach (Card card in cards)
        {
            card.cardVisual.UpdateIndex(transform.childCount);
        }
    }


}
