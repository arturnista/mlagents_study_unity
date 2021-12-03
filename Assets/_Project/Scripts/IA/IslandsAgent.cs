using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class IslandsAgent : Agent
{

    private Game _game;
    public Game Game => _game;

    public override void OnEpisodeBegin()
    {
        _game = DI.Exists<Game>() ? DI.Get<Game>() : new Game(8, 2, 10);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var pieces = _game.Board.BoardPieces;
        foreach (var item in _game.Board.BoardGround)
        {
            // sensor.AddObservation(item.Key.x); // 64
            // sensor.AddObservation(item.Key.y); // 64
            sensor.AddObservation(pieces.ContainsKey(item.Key) ? (int)pieces[item.Key].Type : (int)item.Value); // 64
        }
        
        foreach (var item in _game.Cards)
        {
            sensor.AddObservation((int)item.Piece.Type); // 10
            sensor.AddObservation((int)item.Position); // 10
            sensor.AddObservation(item.Position == Card.PositionType.Column ? item.Value.y : item.Value.x); // 10
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 4
        int cardIndex = actionBuffers.DiscreteActions[0];
        int position = actionBuffers.DiscreteActions[1];
        int shouldEndGame = actionBuffers.DiscreteActions[2];
        // Debug.Log($"{shouldEndGame}");

        cardIndex = Mathf.Clamp(cardIndex, 0, _game.Cards.Count - 1);
        var cardToPlay = _game.Cards[cardIndex];
        Vector2Int positionToPlayCard = Vector2Int.zero;

        switch (cardToPlay.Position)
        {
            case Card.PositionType.Column:
                positionToPlayCard.x = position;
                positionToPlayCard.y = cardToPlay.Value.y;
                break;

            case Card.PositionType.Row:
                positionToPlayCard.x = cardToPlay.Value.x;
                positionToPlayCard.y = position;
                break;
        }

        bool result = _game.PlayCard(
            _game.Cards[cardIndex],
            positionToPlayCard
        );
        // if (!result) AddReward(-500f);

        if (shouldEndGame == 1)
        {
            _game.EndGame();
        }
        
        if (!_game.GameIsRunning)
        {
            AddReward(_game.Score);
            EndEpisode();
        }
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        for (int i = 0; i < _game.Cards.Count; i++)
        {
            actionMask.SetActionEnabled(0, i, !_game.Cards[i].Played);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = _game.Cards.FindIndex(x => !x.Played);
        discreteActionsOut[1] = Random.Range(0, _game.Board.Size);
        discreteActionsOut[2] = Random.Range(0, 2);
    }
}