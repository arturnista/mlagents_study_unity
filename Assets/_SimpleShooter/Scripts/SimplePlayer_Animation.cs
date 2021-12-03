using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePlayer_Animation : MonoBehaviour
{

    [SerializeField] private Transform _firePoint;
    [SerializeField] private AudioClip _fireClip;
    [Space]
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private GameObject _firePrefab;
    [SerializeField] private GameObject _damageEffect;
    [SerializeField] private GameObject _floatingPrefab;

    private Rigidbody _rigidbody;
    private SimplePlayer _player;
    private Animator _animator;
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        _rigidbody = GetComponent<Rigidbody>();
        _player = GetComponent<SimplePlayer>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {

        _player.OnFire += (direction) => {
            StartCoroutine(FireCoroutine(direction));
        };

        _player.OnDamage += (normal) => {
            Instantiate(_damageEffect, transform.position + (Vector3.up * .5f), Quaternion.LookRotation(normal, Vector3.up));
            ScreenShake.Shake(.2f, .1f);
        };

        _player.OnKill += () => {
            var created = Instantiate(_floatingPrefab, transform.position, Quaternion.identity);
            created.transform.LeanMoveLocalY(2f, 1f)
            .setOnComplete(() => Destroy(created));
        };
    }

    private void Update()
    {
        var localDirection = transform.InverseTransformDirection(_rigidbody.velocity.normalized);
        _animator.SetFloat("Vertical", localDirection.z);
        _animator.SetFloat("Horizontal", localDirection.x);
    }

    private IEnumerator FireCoroutine(Vector3 direction)
    {
        _muzzleFlash?.Play();
        ScreenShake.Shake(.05f, .1f);
        _audioSource.PlayOneShot(_fireClip);
        var created = Instantiate(_firePrefab, _firePoint.position, Quaternion.LookRotation(direction, Vector3.up));
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
