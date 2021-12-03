using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ShooterSimpleAgent : Agent
{

    private SimplePlayer _player;
    [SerializeField] private SimplePlayer _otherPlayer;

    private Vector3 _lastPosition;
    private int _stepsWaiting;

    public override void OnEpisodeBegin()
    {
        _player = GetComponentInParent<SimplePlayer>();
        if (_player.IsDead)
        {
            _player.Reset();
        }
        _lastPosition = _player.transform.localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_player.transform.localPosition.x);
        sensor.AddObservation(_player.transform.localPosition.z);
        
        var direction = (_otherPlayer.transform.localPosition - _player.transform.localPosition).normalized;
        sensor.AddObservation(Vector3.Dot(_player.transform.forward, direction));
        
        sensor.AddObservation(_otherPlayer.transform.localPosition.x);
        sensor.AddObservation(_otherPlayer.transform.localPosition.z);

        var invDirection = (_player.transform.localPosition - _otherPlayer.transform.localPosition).normalized;
        sensor.AddObservation(Vector3.Dot(_otherPlayer.transform.forward, invDirection));
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int moveAction = actionBuffers.DiscreteActions[0];
        int rotateAction = actionBuffers.DiscreteActions[1];
        bool fireAction = actionBuffers.DiscreteActions[2] == 1;

        Vector3 dirToGo = Vector3.zero;
        switch (moveAction)
        {
            case 1:
                dirToGo = Vector3.forward * 1f;
                break;
            case 2:
                dirToGo = Vector3.forward * -1f;
                break;
            case 3:
                dirToGo = Vector3.right * 1f;
                break;
            case 4:
                dirToGo = Vector3.right * -1f;
                break;
        }

        float rotation = 0f;
        switch (rotateAction)
        {
            case 1:
                rotation = 1f;
                break;
            case 2:
                rotation = -1f;
                break;
        }

        if (_player.IsDead)
        {
            EndEpisode();
            return;
        }

        AddReward(-0.005f);
        if (_player.transform.localPosition.y < -0.5f)
        {
            _player.DealDamage();
            SetReward(-1f);
            EndEpisode();
            return;
        }
        // else
        // {
        //     float stepReward = 1 - (StepCount / (StepCount + 100f));
        //     SetReward(stepReward);
        // }

        // if (Vector3.Distance(_lastPosition, _player.transform.localPosition) < 1f)
        // {
        //     _stepsWaiting += 1;
        //     if (_stepsWaiting > 50)
        //     {
        //         _player.DealDamage();
        //         SetReward(-1f);
        //         EndEpisode();
        //         return;
        //     }
        // }
        // else
        // {
        //     _lastPosition = _player.transform.localPosition;
        //     _stepsWaiting = 0;
        // }

        _player.Rotate(rotation);
        _player.Move(dirToGo);
        if (fireAction)
        {
            bool result = _player.Shoot();
            if (result)
            {
                float stepReward = 1 - (StepCount / (StepCount + 100f));
                SetReward(1f + stepReward);
                EndEpisode();
            }
        }
    }

    public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
    {
        actionMask.SetActionEnabled(0, 1, _player.CanFire);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[1] = 1;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[1] = 2;
        }

        discreteActionsOut[2] = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
    }
}