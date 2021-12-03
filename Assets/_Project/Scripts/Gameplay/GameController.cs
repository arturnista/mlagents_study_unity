using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour, IGameController
{
    
    [SerializeField] private GroundPiece _waterPrefab;
    [SerializeField] private GroundPiece _islandPrefab;
    [Space]
    [SerializeField] private GenericDictionary<Piece.PieceType, UIPiece> _piecesPrefabs;
    [Space]
    [SerializeField] private Transform _cardParent;
    [SerializeField] private UICard _cardPrefab;
    [Space]
    [SerializeField] private TextMeshProUGUI _endGameText;
    [SerializeField] private Button _endGameButton;
    
    private Game _game;

    private Camera _mainCamera;

    private Card _cardSelected;
    private UICard _uiCardSelected;

    private GameObject _tilesParent;
    private GameObject _piecesParent;

    private void Awake()
    {

        DI.Set<IGameController>(this);

        int size = 10;
        int islands = 2;
        _game = new Game(size, islands, 25);

        _piecesParent = new GameObject("Pieces");
        _tilesParent = new GameObject("Tiles");

        Vector3 centerPosition = Vector3.zero;

        foreach (var item in _game.Board.BoardGround)
        {
            var pos = BoardToWorldPosition(item.Key);
            
            GroundPiece piece = null;
            if (item.Value == Board.GroundType.Island)
            {
                piece = Instantiate(_islandPrefab, pos, Quaternion.identity, _tilesParent.transform);
            }
            else if (item.Value == Board.GroundType.Water)
            {
                piece = Instantiate(_waterPrefab, pos, Quaternion.identity, _tilesParent.transform);
            }
            piece.Init(item.Value, item.Key);

            if (pos.x > centerPosition.x) centerPosition.x = pos.x;
            if (pos.z > centerPosition.z) centerPosition.z = pos.z;
        }

        centerPosition.x = centerPosition.x / 2f;
        centerPosition.z = centerPosition.z / 2f;

        foreach (var item in _game.Cards)
        {
            var card = Instantiate(_cardPrefab, _cardParent);
            card.Init(item);
        }

        _mainCamera = Camera.main;
        _mainCamera.transform.parent.position = centerPosition;
        _mainCamera.transform.position = new Vector3(
            (size / 2f) - .5f,
            (size / 2f) - .5f,
            -(size * .3f) - 1f
        );

        _endGameText.gameObject.SetActive(false);
        _endGameButton.onClick.AddListener(() =>
        {
            _game.EndGame(); 
            _endGameText.gameObject.SetActive(true);
            _endGameText.text = _game.Score.ToString();

            var pieces = _piecesParent.GetComponentsInChildren<UIPiece>();
            foreach (var item in pieces)
            {
                item.ShowScore();
            }
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            _mainCamera.transform.parent.LeanRotateAround(Vector3.up, 90f, .5f);
        }
        else  if (Input.GetKeyDown(KeyCode.A))
        {
            _mainCamera.transform.parent.LeanRotateAround(Vector3.up, -90f, .5f);
        }
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

    }

    public void HighlightCard(Card card)
    {

    }

    public void SelectCard(Card card, UICard uiCard)
    {
        if (_cardSelected != null) return;
        _cardSelected = card;
        _uiCardSelected = uiCard;
        HideCards();
    }

    public void SelectPosition(Vector2Int position)
    {
        if (_cardSelected == null) return;
        bool result = _game.PlayCard(_cardSelected, position);
        if (result)
        {
            var pos = BoardToWorldPosition(position);
            RaycastHit hit;
            if (Physics.Raycast(pos + Vector3.up * 10f, Vector3.down, out hit))
            {
                var uiPiece = Instantiate(_piecesPrefabs[_cardSelected.Piece.Type], hit.point, Quaternion.identity, _piecesParent.transform);
                uiPiece.Init(_cardSelected.Piece);
            }

            Destroy(_uiCardSelected.gameObject);
        }

        _cardSelected = null;
        ShowCards();
    }

    private Vector3 BoardToWorldPosition(Vector2Int position)
    {
        return new Vector3(position.x, 0f, position.y);
    }

}
