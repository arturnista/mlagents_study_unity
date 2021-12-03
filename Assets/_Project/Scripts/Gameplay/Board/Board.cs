using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{

    public enum GroundType
    {
        None = -5,
        Water = -1,
        Island = -2
    }
    
    private Dictionary<Vector2Int, Piece> _boardPieces;
    public Dictionary<Vector2Int, Piece> BoardPieces => _boardPieces;
    private Dictionary<Vector2Int, GroundType> _boardGround;
    public Dictionary<Vector2Int, GroundType> BoardGround => _boardGround;

    private int _size;
    public int Size => _size;
    
    private int _islands;

    public Board(int size, int islands)
    {
        _size = size;
        _islands = islands;

        _boardPieces = new Dictionary<Vector2Int, Piece>();
        _boardGround = new Dictionary<Vector2Int, GroundType>();
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                _boardGround.Add(position, GroundType.Water);
            }
        }
        
        CreateIslands();
    }

    private void CreateIslands()
    {
        var startTime = Time.time;
        float increase = 1f / (_size / 2f);
        // Debug.Log($"Island increase: {increase}");

        int islandOffset = 2;

        for (int i = 0; i < _islands; i++)
        {
            int xOrigin = Random.Range(islandOffset, _size - islandOffset);
            int yOrigin = Random.Range(islandOffset, _size - islandOffset);
            CreateIslandPosition(new Vector2Int(xOrigin, yOrigin), increase);
        }
    }

    private void CreateIslandPosition(Vector2Int position, float increase, float percentage = 0f)
    {
        if (percentage >= 1f)
        {
            return;
        }

        TransformIntoIsland(position);

        if (Random.value >= percentage)
        {
            CreateIslandPosition(position + Vector2Int.right, increase, percentage + increase);
        }

        if (Random.value >= percentage)
        {
            CreateIslandPosition(position + Vector2Int.left, increase, percentage + increase);
        }

        if (Random.value >= percentage)
        {
            CreateIslandPosition(position + Vector2Int.up, increase, percentage + increase);
        }

        if (Random.value >= percentage)
        {
            CreateIslandPosition(position + Vector2Int.down, increase, percentage + increase);
        }
    }

    private void TransformIntoIsland(Vector2Int position)
    {
        if (!_boardGround.ContainsKey(position)) return;
        _boardGround[position] = GroundType.Island;
    }

    public List<GroundType> GetTouchingGroundTypes(Vector2Int position)
    {
        List<GroundType> grounds = new List<GroundType>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                var groundType = GetGroundType(new Vector2Int(
                    position.x + x,
                    position.y + y
                ));
                if (groundType != GroundType.None)
                {
                    grounds.Add(groundType);
                }
            }
        }

        return grounds;
    }

    public GroundType GetGroundType(Vector2Int position)
    {
        if (!_boardGround.ContainsKey(position)) return GroundType.None;
        return _boardGround[position];
    }

    public List<Piece> GetTouchingPieces(Vector2Int position, int distance = 1)
    {
        List<Piece> pieces = new List<Piece>();
        for (int x = -distance; x <= distance; x++)
        {
            for (int y = -distance; y <= distance; y++)
            {
                if (x == 0 && y == 0) continue;

                var piece = GetPiece(new Vector2Int(
                    position.x + x,
                    position.y + y
                ));
                if (piece != null)
                {
                    pieces.Add(piece);
                }
            }
        }

        return pieces;
    }

    public Piece GetPiece(Vector2Int position)
    {
        if (!_boardPieces.ContainsKey(position)) return null;
        return _boardPieces[position];
    }

    public bool PlacePiece(Piece piece, Vector2Int position)
    {
        if (_boardPieces.ContainsKey(position)) return false;
        if (!_boardGround.ContainsKey(position)) return false;
        _boardPieces.Add(position, piece);
        piece.Place(position);
        return true;
    }

}
