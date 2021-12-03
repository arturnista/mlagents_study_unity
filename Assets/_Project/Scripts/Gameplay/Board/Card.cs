using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card
{

    public enum PositionType
    {
        Column = 0,
        Row = 1,
        Position = 2
    }

    public Piece Piece { get; private set; }
    public PositionType Position { get; private set; }
    public Vector2Int Value { get; private set; }
    public bool Played { get; private set; }

    private Board _board;

    public Card(Piece piece, Board board)
    {
        _board = board;
        Piece = piece;
        CreatePosition();
    }

    private void CreatePosition()
    {

        Position = (PositionType)Random.Range(0, 2);
        var positionValue = Vector2Int.zero;
        
        switch (Position)
        {
            case PositionType.Column:
                positionValue.y = Random.Range(0, _board.Size);
                break;
            case PositionType.Row:
                positionValue.x = Random.Range(0, _board.Size);
                break;
            case PositionType.Position:
                positionValue = new Vector2Int(
                    Random.Range(1, _board.Size - 1),
                    Random.Range(1, _board.Size - 1)
                );
                break;
        }

        Value = positionValue;
    }

    public void Play()
    {
        Played = true;
    }

    public bool ValidatePosition(Vector2Int position) 
    {
        switch (Position)
        {
            case PositionType.Column:
                return position.y == Value.y;
            case PositionType.Row:
                return position.x == Value.x;
            case PositionType.Position:
                int xDist = Mathf.Abs(position.x - Value.x);
                int yDist = Mathf.Abs(position.y - Value.y);
                return xDist <= 1 && yDist <= 1;
            default:
               return false;
        }
    }

}
