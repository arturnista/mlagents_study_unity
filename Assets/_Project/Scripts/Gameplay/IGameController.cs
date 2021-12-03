using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public interface IGameController
{

    void ClearHighlight();
    void HighlightCard(Card card);
    void SelectCard(Card card, UICard uiCard);
    void SelectPosition(Vector2Int position);

}
