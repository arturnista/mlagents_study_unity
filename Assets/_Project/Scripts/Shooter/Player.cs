using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 25f;
    [SerializeField] private bool _autoReset = false;
    [SerializeField] private bool _hasTimeToLive = false;
    [SerializeField] private float _timeToLive = 5f;

    private float _fireDelay;
    public bool CanFire => _fireDelay <= 0f;
    public bool IsDead { get; private set; }

    private CharacterController _characterController;
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;

    private float _maxPosition = 10f;
    private float _liveTime;
    public float LiveTime => _liveTime;
    private float _rotateSelected;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _rigidbody = GetComponent<Rigidbody>();
        _fireDelay = 0f;
    }

    public void Reset()
    {
        _fireDelay = 0f;
        IsDead = false;
        transform.localPosition = new Vector3(Random.Range(-_maxPosition, _maxPosition), 0f, Random.Range(-_maxPosition, _maxPosition));
        _rigidbody.velocity = Vector3.zero;
        transform.rotation = Quaternion.identity;
        _liveTime = _timeToLive;
    }

    public void Rotate(float angle)
    {
        _rotateSelected = angle;
    }
    
    public void Move(Vector3 motion)
    {
        // _characterController.SimpleMove(motion.normalized * _moveSpeed);
        _rigidbody.AddForce(motion.normalized * _moveSpeed, ForceMode.VelocityChange);
    }

    public void MoveForward(float speed)
    {
        // _characterController.SimpleMove(transform.forward * speed * _moveSpeed);
        _rigidbody.AddForce(transform.forward * speed * _moveSpeed, ForceMode.VelocityChange);
    }

    private void Update()
    {
        _fireDelay -= Time.deltaTime;        
        transform.Rotate(Vector3.up, _rotateSelected * _rotateSpeed * Time.deltaTime);

        _liveTime -= Time.deltaTime;
        if (_hasTimeToLive && _liveTime < 0f)
        {
            DealDamage();
        }
    }

    public bool Shoot()
    {

        if (!CanFire) return false;
        _fireDelay = .2f;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + (Vector3.up * .5f), transform.forward, out hit, 20f, LayerMask.GetMask("Player", "Cover")))
        {
            Player player;
            if (hit.collider.TryGetComponent<Player>(out player))
            {
                player.DealDamage();
                _liveTime = _timeToLive;
                return true;
            }
        }
        
        return false;
    }

    public void DealDamage()
    {
        IsDead = true;
        if (_autoReset) Reset();
    }

}
