using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game
{

    public delegate void CardPlayHandler(Card card);
    public event CardPlayHandler OnCardPlay;

    public delegate void FailCardPlayHandler(Card card, Vector2Int position);
    public event FailCardPlayHandler OnFailCardPlay;

    public delegate void GameEndHandler();
    public event GameEndHandler OnGameEnd;

    private Board _board;
    public Board Board => _board;

    private List<Card> _cards;
    public List<Card> Cards => _cards;

    private int _score;
    public int Score => _score;

    private bool _gameIsRunning;
    public bool GameIsRunning => _gameIsRunning;

    public Game(int size, int islands, int cards)
    {
        _board = new Board(size, islands);

        _cards = new List<Card>();
        for (int i = 0; i < cards; i++)
        {
            _cards.Add( new Card(Piece.CreateRandom(_board), _board) );
        }

        _gameIsRunning = true;
    }

    public bool PlayCard(Card card, Vector2Int position)
    {
        if (!_cards.Contains(card))
        {
            OnFailCardPlay?.Invoke(card, position);
            return false;
        }

        if (card.Played)
        {
            OnFailCardPlay?.Invoke(card, position);
            return false;
        }

        if (!card.ValidatePosition(position))
        {
            OnFailCardPlay?.Invoke(card, position);
            return false;
        }
        
        var result = _board.PlacePiece(card.Piece, position);
        if (!result)
        {
            OnFailCardPlay?.Invoke(card, position);
            return false;
        }

        // _cards.Remove(card);
        card.Play();
        OnCardPlay?.Invoke(card);

        if (_cards.Find(x => !x.Played) == null) EndGame();
        return true;
    }

    public int EndGame()
    {
        _gameIsRunning = false;

        int score = 0;
        foreach (var item in _board.BoardPieces)
        {
            score += item.Value.CalculateScore();
        }

        foreach (var item in _cards)
        {
            if (!item.Played) score -= 3;
        }

        _score = score;
        OnGameEnd?.Invoke();

        return score;
    }

}