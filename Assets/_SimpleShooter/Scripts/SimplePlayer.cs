using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayer : MonoBehaviour
{

    public delegate void FireHandler(Vector3 direction);
    public event FireHandler OnFire;

    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _rotateSpeed = 25f;

    private float _fireDelay;
    public bool CanFire => _fireDelay <= 0f;
    public bool IsDead { get; private set; }

    private CharacterController _characterController;
    private Rigidbody _rigidbody;
    public Rigidbody Rigidbody => _rigidbody;

    private float _maxPosition = 10f;
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
    }

    public void Rotate(float angle)
    {
        _rotateSelected = angle;
        // transform.Rotate(Vector3.up, _rotateSelected * _rotateSpeed * Time.deltaTime);
    }
    
    public void Move(Vector3 motion)
    {
        _rigidbody.AddForce(motion.normalized * _moveSpeed, ForceMode.VelocityChange);
    }

    public void MoveForward(float speed)
    {
        _rigidbody.AddForce(transform.forward * speed * _moveSpeed, ForceMode.VelocityChange);
    }

    private void Update()
    {
        _fireDelay -= Time.deltaTime;
        transform.Rotate(Vector3.up, _rotateSelected * _rotateSpeed * Time.deltaTime);
    }

    public bool Shoot()
    {

        if (!CanFire) return false;
        _fireDelay = .2f;

        RaycastHit hit;
        OnFire?.Invoke(transform.forward);
        if (Physics.Raycast(transform.position + (Vector3.up * .5f), transform.forward, out hit, 20f, LayerMask.GetMask("Player")))
        {
            SimplePlayer player;
            if (hit.collider.TryGetComponent<SimplePlayer>(out player))
            {
                player.DealDamage();
                return true;
            }
        }
        
        return false;
    }

    public void DealDamage()
    {
        IsDead = true;
    }

}
