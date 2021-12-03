using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController2D : MonoBehaviour, IGameController
{
    [Space]
    [SerializeField] private int _size = 10;
    [SerializeField] private int _islands = 2;
    [SerializeField] private int _cards = 25;
    [SerializeField] private SpriteGroundPiece _waterPrefab;
    [SerializeField] private SpriteGroundPiece _islandPrefab;
    [Space]
    [SerializeField] private GameObject _positionCounterPrefab;
    [Space]
    [SerializeField] private GenericDictionary<Piece.PieceType, UIPiece> _piecesPrefabs;
    [Space]
    [SerializeField] private Transform _cardParent;
    [SerializeField] private UICard _cardPrefab;
    [Space]
    [SerializeField] private TextMeshProUGUI _endGameText;
    [SerializeField] private Button _endGameButton;
    [Space]
    [SerializeField] private GameObject _agent;

    private Dictionary<Vector2Int, SpriteGroundPiece> _pieces;
    private Dictionary<Card, UICard> _uiCards;
    
    private Game _game;

    private Camera _mainCamera;

    private Card _cardSelected;
    private UICard _uiCardSelected;

    private GameObject _tilesParent;
    private GameObject _piecesParent;

    private void Awake()
    {
        DI.Set<IGameController>(this);

        _piecesParent = new GameObject("Pieces");
        _tilesParent = new GameObject("Tiles");

        CreateGame();

        _endGameText.gameObject.SetActive(false);
        _endGameButton.onClick.AddListener(() => _game.EndGame());
        
        _game.OnGameEnd += HandleGameEnd;
        _game.OnCardPlay += HandleCardPlay;
        _game.OnFailCardPlay += HandleFailCardPlay;
    }

    private void CreateGame()
    {
        _game = new Game(_size, _islands, _cards);
        DI.Set<Game>(_game);

        _pieces = new Dictionary<Vector2Int, SpriteGroundPiece>();
        _uiCards = new Dictionary<Card, UICard>();

        Vector3 centerPosition = Vector3.zero;

        foreach (var item in _game.Board.BoardGround)
        {
            var pos = BoardToWorldPosition(item.Key);
            
            SpriteGroundPiece piece = null;
            if (item.Value == Board.GroundType.Island)
            {
                piece = Instantiate(_islandPrefab, pos, Quaternion.identity, _tilesParent.transform);
            }
            else if (item.Value == Board.GroundType.Water)
            {
                piece = Instantiate(_waterPrefab, pos, Quaternion.identity, _tilesParent.transform);
            }
            _pieces.Add(item.Key, piece);
            piece.Init(item.Value, item.Key);

            if (pos.x > centerPosition.x) centerPosition.x = pos.x;
            if (pos.z > centerPosition.z) centerPosition.z = pos.z;
        }

        for (int i = 0; i < _size; i++)
        {
            var created = Instantiate(_positionCounterPrefab, new Vector3(i, -1f, 0f), Quaternion.identity);
            created.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
            
            created = Instantiate(_positionCounterPrefab, new Vector3(-1f, i, 0f), Quaternion.identity);
            created.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
        }

        centerPosition.x = centerPosition.x / 2f;
        centerPosition.z = centerPosition.z / 2f;

        foreach (var item in _game.Cards)
        {
            var card = Instantiate(_cardPrefab, _cardParent);
            card.Init(item);
            _uiCards.Add(item, card);
        }

        _mainCamera = Camera.main;
        _mainCamera.transform.parent.position = centerPosition;
        _mainCamera.transform.position = new Vector3(
            (_size / 2f) - .5f,
            (_size / 2f) - .5f - 2f,
            -10f
        );

        if (_agent) _agent.SetActive(true);
    }

    private void HandleGameEnd()
    {
        _endGameText.gameObject.SetActive(true);
        _endGameText.text = $"Score: {_game.Score}";

        var pieces = _piecesParent.GetComponentsInChildren<UIPiece>();
        foreach (var item in pieces)
        {
            item.ShowScore();
        }
    }
    private void HandleCardPlay(Card card)
    {
        var pos = BoardToWorldPosition(card.Piece.Position);
        var uiPiece = Instantiate(_piecesPrefabs[card.Piece.Type], pos, Quaternion.identity, _piecesParent.transform);
        uiPiece.Init(card.Piece);

        Destroy(_uiCards[card].gameObject);

        _cardSelected = null;

        ClearHighlight();
        ShowCards();
    }

    private void HandleFailCardPlay(Card card, Vector2Int position)
    {
        Debug.Log($"Failed to play {card.Piece.Type} on POS = {position.x}, {position.y}");
        _cardSelected = null;   

        ClearHighlight();
        ShowCards();
    }

    public void HideCards()
    {
        _cardParent.gameObject.SetActive(false);
    }

    public void ShowCards()
    {
        _cardParent.gameObject.SetActive(true);
    }

    public void ClearHighlight()
    {
        foreach (var item in _pieces)
        {
            item.Value.SetNormal();
        }
    }

    public void HighlightCard(Card card)
    {
        foreach (var item in _pieces)
        {
            if (card.ValidatePosition(item.Key)) item.Value.SetHighlight();
            else item.Value.SetNormal();
        }
    }

    public void SelectCard(Card card, UICard uiCard)
    {
        if (_cardSelected != null) return;
        _cardSelected = card;
        
        HighlightCard(card);
        HideCards();
    }

    public void SelectPosition(Vector2Int position)
    {
        if (_cardSelected == null) return;
        _game.PlayCard(_cardSelected, position);
    }

    private Vector3 BoardToWorldPosition(Vector2Int position)
    {
        return new Vector3(position.x, position.y, 0f);
    }

}
