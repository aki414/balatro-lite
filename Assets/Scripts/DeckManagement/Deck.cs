using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class Deck : MonoBehaviour
{

    public static Deck Instance { get; private set; } //Singleton

    //now we need a reference to what a deck is, a.k.a. what cards it contains -> CardCollection
    //we will work with one deck for now, but you can easily add several choices for the player to pick from
    [SerializeField] private CardCollection _playerDeck;
    [SerializeField] private GameObject _cardSlotPrefab;
    [SerializeField] private GameObject _PlayingCardgroup;

    [SerializeField] private GameObject _DiscardCardgroup;
    [SerializeField] private GameObject _PlayedHandgroup;
    public HorizontalCardHolder cardHolder;


    //now to represent the instantiated Cards
    private List<Card> _deckPile = new();
    private List<Card> _discardPile = new();

    public List<Card> HandCards { get; private set; } = new();

    private int HandSize = 8;


    private void Awake()
    {
        //typical singleton declaration
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //we will instantiate the deck once, at the start of the game/level
        InstantiateDeck();
        for (int i = 0; i < HandSize; i++)
        {
            DrawCard();
        }

    }

    private void InstantiateDeck()
    {
        for (int i = 0; i < _playerDeck.CardsInCollection.Count; i++)
        {
            GameObject cardSlot = Instantiate(_cardSlotPrefab, transform); // Instantiate the CardSlot prefab as child of the Card Canvas
            Card card = cardSlot.GetComponentInChildren<Card>(); // Get the Card prefab inside the CardSlot prefab
            card.SetUp(_playerDeck.CardsInCollection[i]);
            _deckPile.Add(card); //at the start, all cards are in the deck, none in hand, none in discard
            card.gameObject.SetActive(false);
        }
        ShuffleDeck();


    }

    //call once at start and whenever deck count hits zero
    //uses the Fisher-Yates shuffle algorithm
    private void ShuffleDeck()
    {
        for (int i = _deckPile.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            var temp = _deckPile[i];
            _deckPile[i] = _deckPile[j];
            _deckPile[j] = temp;
        }
    }

    public void DrawCard()
    {

        if (_deckPile.Count > 0)
        {
            HandCards.Add(_deckPile[0]);
            _deckPile[0].gameObject.SetActive(true);
            _deckPile[0].transform.parent.transform.SetParent(_PlayingCardgroup.transform); // Set parent of CardSlot to _PlayingCardgroup
            _deckPile.RemoveAt(0);
        }
        else
        {
            Debug.Log("The deck is empty!");
        }


    }

    public void DiscardCard()
    {
        if (cardHolder.selectedCards.Count > 5)
            return;
        else
        {
            foreach (Card card in cardHolder.selectedCards)
            {
                HandCards.Remove(card);
                _discardPile.Add(card);
                card.OnDestroy();
                DrawCard();
            }
            cardHolder.selectedCards.Clear();
        }
    }

    public void PlayHand()
    {
        if (cardHolder.selectedCards.Count > 5)
            return;
        else
        {
            foreach (Card card in cardHolder.selectedCards)
            {
                HandCards.Remove(card);
                _discardPile.Add(card);
                card.gameObject.SetActive(false);

                card.transform.parent.transform.SetParent(_PlayedHandgroup.transform);

                DrawCard();
            }
            cardHolder.selectedCards.Clear();
        }

    }

}
