using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayer_Animation : MonoBehaviour
{

    [SerializeField] private GameObject _firePrefab;

    private Rigidbody _rigidbody;
    private SimplePlayer _player;
    private Animator _animator;
    
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _player = GetComponent<SimplePlayer>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        _player.OnFire += (direction) => StartCoroutine(FireCoroutine(direction));
    }

    private void Update()
    {
        var localDirection = transform.InverseTransformDirection(_rigidbody.velocity.normalized);
        _animator.SetFloat("Vertical", localDirection.z);
        _animator.SetFloat("Horizontal", localDirection.x);
    }

    private IEnumerator FireCoroutine(Vector3 direction)
    {
        var created = Instantiate(_firePrefab, transform.position + (Vector3.up * .5f), Quaternion.LookRotation(direction, Vector3.up));
        float speed = 100f;
        float distance = 0f;
        while(distance < 100f)
        {
            distance += speed * Time.deltaTime;
            created.transform.position += direction * speed * Time.deltaTime;
            yield return null;
        }
        Destroy(created);
    }

}
