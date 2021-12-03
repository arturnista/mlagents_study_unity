using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIScore : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI _redScoreText;
    [SerializeField] private TextMeshProUGUI _blueScoreText;
    
    [Space]
    [SerializeField] private SimplePlayer _redPlayer;
    [SerializeField] private SimplePlayer _bluePlayer;
    
    private SimplePlayer_Animation _redPlayerAnimation;
    private SimplePlayer_Animation _bluePlayerAnimation;

    private int _redKillCount;
    private int _blueKillCount;

    private void OnEnable()
    {
        _redKillCount = 0;
        _blueKillCount = 0;
        _redScoreText.text = "0";
        _blueScoreText.text = "0";

        _redPlayerAnimation = _redPlayer.GetComponent<SimplePlayer_Animation>();
        _bluePlayerAnimation = _bluePlayer.GetComponent<SimplePlayer_Animation>();

        _redPlayer.OnKill += () => {
            _redKillCount += 1;
            _redScoreText.text = _redKillCount.ToString();
            LeanTween.value(gameObject, 1f, 1.5f, .3f)
            .setOnUpdate(value => _redScoreText.GetComponent<RectTransform>().localScale = Vector3.one * value)
            .setEasePunch();
        };

        _bluePlayer.OnKill += () => {
            _blueKillCount += 1;
            _blueScoreText.text = _blueKillCount.ToString();
            LeanTween.value(gameObject, 1f, 1.5f, .3f)
            .setOnUpdate(value => _blueScoreText.GetComponent<RectTransform>().localScale = Vector3.one * value)
            .setEasePunch();
        };
    }

}
