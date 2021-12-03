using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UICard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private Image _iconImage;
    [SerializeField] private TextMeshProUGUI _positionText;
    [Space]
    [SerializeField] private GenericDictionary<Piece.PieceType, Sprite> _spriteByType;

    private IGameController _gameController;
    
    private Card _card;

    private void Awake()
    {
        _gameController = DI.Get<IGameController>();
    }

    public void Init(Card card)
    {
        _card = card;

        _nameText.text = _card.Piece.Type.ToString();
        switch (_card.Position)
        {
            case Card.PositionType.Column:
                _positionText.text = $"{_card.Position}: {_card.Value.y}";
                break;
            case Card.PositionType.Row:
                _positionText.text = $"{_card.Position}: {_card.Value.x}";
                break;
            case Card.PositionType.Position:
                _positionText.text = $"{_card.Position}: {_card.Value.x}, {_card.Value.y}";
                break;
        }

        if (_spriteByType.ContainsKey(_card.Piece.Type))
        {
            _iconImage.sprite = _spriteByType[_card.Piece.Type];
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _gameController.SelectCard(_card, this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var rect = GetComponent<RectTransform>();
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, 1f, 1.2f, .1f)
        .setOnUpdate(value => rect.localScale = Vector3.one * value);

        _gameController.HighlightCard(_card);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var rect = GetComponent<RectTransform>();
        LeanTween.cancel(gameObject);
        LeanTween.value(gameObject, 1.2f, 1f, .05f)
        .setOnUpdate(value => rect.localScale = Vector3.one * value);

        _gameController.ClearHighlight();
    }
}
