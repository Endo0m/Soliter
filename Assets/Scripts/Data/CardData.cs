using UnityEngine;
using CardGame.Core;

[CreateAssetMenu(fileName = "CardData", menuName = "Cards/Card Data")]
public class CardData : ScriptableObject
{
    [Header("Визуал лица")]
    public Sprite face;

    [Header("Семантика")]
    public Suit suit;
    public Rank rank;
}
