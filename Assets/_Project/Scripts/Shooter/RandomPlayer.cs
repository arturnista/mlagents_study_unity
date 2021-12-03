using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPlayer : MonoBehaviour
{

    private Player _player;

    private float _timeToMove;
    private float _speed;

    private float _timeToRotate;
    private float _rotate;

    private float _timeToFire;

    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Update()
    {
        _timeToMove -= Time.deltaTime;
        if (_timeToMove < 0f)
        {
            _timeToMove = Random.Range(.1f, .5f);
            _speed = Random.Range(-1f, 1f);
        }

        _timeToRotate -= Time.deltaTime;
        if (_timeToRotate < 0f)
        {
            _rotate = Random.Range(-1f, 1f);
        }

        _timeToFire -= Time.deltaTime;
        if (_timeToFire < 0f)
        {
            _timeToFire = Random.Range(.5f, 1f);
            _player.Shoot();
        }

        _player.MoveForward(_speed);
        _player.Rotate(_rotate);

        if (transform.localPosition.y < -.05f)
        {
            _player.DealDamage();
        }
    }

}
