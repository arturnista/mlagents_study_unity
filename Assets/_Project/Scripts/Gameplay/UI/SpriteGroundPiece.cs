using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteGroundPiece : MonoBehaviour
{

    [SerializeField] private GameObject _select;

    private IGameController _gameController;

    private Board.GroundType _type;
    private Vector2Int _position;

    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _gameController = DI.Get<IGameController>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _select.SetActive(false);
    }

    public void Init(Board.GroundType type, Vector2Int position)
    {
        _type = type;
        _position = position;
    }
    
    public void SetNormal()
    {
        _spriteRenderer.color = Color.white;
    }
    
    public void SetHighlight()
    {
        _spriteRenderer.color = Color.magenta;
    }

    public void OnMouseEnter()
    {
        _select.SetActive(true);
    }

    public void OnMouseExit()
    {
        _select.SetActive(false);
    }

    public void OnMouseDown()
    {
        _gameController.SelectPosition(_position);
    }
}
