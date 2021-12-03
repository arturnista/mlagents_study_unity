using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class ShooterAgent : Agent
{

    private Player _player;
    [SerializeField] private Player _otherPlayer;
    [SerializeField] private Transform _covers;

    private float _maxDistance = 8f;

    public override void OnEpisodeBegin()
    {
        _player = GetComponentInParent<Player>();
        if (_player.IsDead)
        {
            _player.Reset();
        }

        foreach (Transform cover in _covers)
        {
            cover.localPosition = new Vector3(
                Random.Range(-_maxDistance, _maxDistance),
                0f,
                Random.Range(-_maxDistance, _maxDistance)
            );
            cover.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(_player.transform.localPosition.x);
        sensor.AddObservation(_player.transform.localPosition.z);
        sensor.AddObservation(_player.transform.rotation.eulerAngles.y);
        sensor.AddObservation(_player.Rigidbody.velocity.x);
        sensor.AddObservation(_player.Rigidbody.velocity.z);
        sensor.AddObservation(_player.LiveTime);
        
        sensor.AddObservation(_otherPlayer.transform.localPosition.x);
        sensor.AddObservation(_otherPlayer.transform.localPosition.z);
        sensor.AddObservation(_otherPlayer.transform.rotation.eulerAngles.y);

        foreach (Transform cover in _covers)
        {
            sensor.AddObservation(cover.localPosition.x);
            sensor.AddObservation(cover.localPosition.z);
            sensor.AddObservation(cover.localRotation.eulerAngles.y);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int moveAction = actionBuffers.DiscreteActions[0];
        bool fireAction = actionBuffers.DiscreteActions[1] == 1;

        float moveForward = 0f;
        float rotation = 0f;
        switch (moveAction)
        {
            case 1:
                moveForward = 1f;
                break;
            case 2:
                moveForward = -1f;
                break;
            case 3:
                rotation = 1f;
                break;
            case 4:
                rotation = -1f;
                break;
        }

        if (_player.IsDead)
        {
            SetReward(-1f);
            EndEpisode();
            return;
        }

        if (_player.transform.localPosition.y < -0.5f)
        {
            _player.DealDamage();
            SetReward(-1f);
            EndEpisode();
            return;
        }
        
        _player.Rotate(rotation);
        _player.MoveForward(moveForward);
        if (fireAction)
        {
            bool result = _player.Shoot();
            if (result)
            {
                float stepReward = 1 - (StepCount / (StepCount + 100f));
                AddReward(1f + stepReward);
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
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }

        // if (Input.GetKey(KeyCode.RightArrow))
        // {
        //     discreteActionsOut[1] = 1;
        // }
        // else if (Input.GetKey(KeyCode.LeftArrow))
        // {
        //     discreteActionsOut[1] = 2;
        // }

        discreteActionsOut[1] = Input.GetKeyDown(KeyCode.Space) ? 1 : 0;
    }
}