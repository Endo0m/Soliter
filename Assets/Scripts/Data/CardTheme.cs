using UnityEngine;

[CreateAssetMenu(fileName = "CardTheme", menuName = "Cards/Card Theme")]
public class CardTheme : ScriptableObject
{
    [Header("Рубашка")]
    public Sprite back;

    [Header("Тайминги анимаций")]
    public float flipDuration = 0.2f;
    public float moveDuration = 0.25f;
}
