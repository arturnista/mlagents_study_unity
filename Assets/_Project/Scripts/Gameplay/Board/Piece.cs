using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece
{

    public static Piece CreateRandom(Board board)
    {
        return new Piece(
            (Piece.PieceType) Random.Range(0, (int)PieceType.Beach + 1),
            board
        );
    }

    public enum PieceType
    {
        Boat = 0,
        Church = 1,
        Forest = 2,
        Mountain = 3,
        House = 4,
        Beach = 5
    }

    private PieceType _type;
    public PieceType Type => _type;

    private int _score;
    public int Score => _score;

    private Vector2Int _position;
    public Vector2Int Position => _position;
    private bool _isPlaced;

    private Board _board;

    public Piece(PieceType type, Board board)
    {
        _board = board;
        _type = type;
        _position = Vector2Int.zero;

        _isPlaced = false;
    }

    public void Place(Vector2Int position)
    {
        _position = position;
        _isPlaced = true;
    }

    public int CalculateScore()
    {
        if (!_isPlaced) return 0;

        int score = 0;
        switch (_type)
        {
            case PieceType.Boat:
            {
                score = BoatScore();
                break;
            }
            case PieceType.Church:
            {
                score = ChurchScore();
                break;
            }
            case PieceType.Forest:
            {
                score = ForestScore();
                break;
            }
            case PieceType.Mountain:
            {
                score = MountainScore();
                break;
            }
            case PieceType.House:
            {
                score = HouseScore();
                break;
            }
            case PieceType.Beach:
            {
                score = BeachScore();
                break;
            }
        }
        _score = score;
        return _score;
    }

    private int BoatScore()
    {
        if (_board.BoardGround[_position] == Board.GroundType.Island) return -5;

        int score = 100;
        foreach (var item in _board.BoardGround)
        {
            if (item.Value != Board.GroundType.Island) continue;

            var distance = Mathf.RoundToInt(Vector2Int.Distance(item.Key, _position));
            if (distance < score)
            {
                score = distance;
            }
        }
        
        foreach (var piece in _board.BoardPieces)
        {
            if (piece.Value == this) continue;
            if (piece.Value.Type != PieceType.Boat) continue;

            var distance = Mathf.RoundToInt(Vector2Int.Distance(piece.Key, _position));
            if (distance < score)
            {
                score = distance;
            }
        }

        return score;
    }

    private int ChurchScore()
    {
        if (_board.BoardGround[_position] == Board.GroundType.Water) return -5;
        var pieces = _board.GetTouchingPieces(_position, 3);

        return pieces.Count(x => x.Type == PieceType.House) * 2;
    }

    private int ForestScore()
    {
        if (_board.BoardGround[_position] == Board.GroundType.Water) return -5;

        var pieces = _board.GetTouchingPieces(_position);
        return pieces.Count(x => x._type == PieceType.Forest);
    }

    private int MountainScore()
    {
        if (_board.BoardGround[_position] == Board.GroundType.Water) return -5;

        var pieces = _board.GetTouchingPieces(_position);
        return pieces.Count(x => x._type == PieceType.Forest || x._type == PieceType.Mountain);
    }

    private int HouseScore()
    {
        if (_board.BoardGround[_position] == Board.GroundType.Water) return -5;

        var pieces = _board.GetTouchingPieces(_position);

        List<Piece> uniques = new List<Piece>();
        foreach (Piece item in pieces)
        {
            if (item._type == PieceType.House) continue;
            if (uniques.Find(x => x._type == item._type) == null) uniques.Add(item);
        }
        
        return uniques.Count;
    }

    private int BeachScore()
    {
        if (_board.BoardGround[_position] == Board.GroundType.Island) return -5;
        var grounds = _board.GetTouchingGroundTypes(_position);
        return grounds.Count(x => x == Board.GroundType.Island);
    }

}
