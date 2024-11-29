using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Holds all data for each individual poker card
/// </summary>

[CreateAssetMenu(menuName = "PokerCardData")]
public class ScriptableCard : ScriptableObject
{
    //field: SerializeField lets you reveal properties in the inspector like they were public fields
    [field: SerializeField] public string CardName { get; private set; }
    [field: SerializeField] public CardSuit[] Suits { get; private set; }
    [field: SerializeField] public CardRank[] Ranks { get; private set; }
    [field: SerializeField] public Sprite CardSprite { get; private set; }
    [field: SerializeField] public Sprite EnhanceSprite { get; private set; }

    [field: SerializeField] public int CardValue { get; private set; }

}

public enum CardSuit
{
    Hearts,
    Diamonds,
    Clubs,
    Spades
}

public enum CardRank
{
    Ace,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King
}