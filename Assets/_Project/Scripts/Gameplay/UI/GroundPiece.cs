using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GroundPiece : MonoBehaviour
{

    private IGameController _gameController;

    private Board.GroundType _type;
    private Vector2Int _position;
    
    private void Awake()
    {
        _gameController = DI.Get<IGameController>();
    }

    public void Init(Board.GroundType type, Vector2Int position)
    {
        _type = type;
        _position = position;
    }

    public void OnMouseEnter()
    {
        LeanTween.moveLocalY(gameObject, .3f, .3f);
    }

    public void OnMouseExit()
    {
        LeanTween.moveLocalY(gameObject, 0f, .3f);
    }

    public void OnMouseDown()
    {
        _gameController.SelectPosition(_position);
    }

}
