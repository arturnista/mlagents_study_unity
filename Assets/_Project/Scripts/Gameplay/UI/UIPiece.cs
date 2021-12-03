using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIPiece : MonoBehaviour
{
    
    [SerializeField] private TextMeshProUGUI _scoreText;

    private Piece _piece;

    private void Awake()
    {
        _scoreText.transform.parent.gameObject.SetActive(false);
    }

    public void Init(Piece piece)
    {
        _piece = piece;
    }

    public void ShowScore()
    {
        _scoreText.text = _piece.Score.ToString();
        if (_piece.Score > 0) _scoreText.color = Color.yellow;
        else if (_piece.Score < 0) _scoreText.color = Color.red;
        _scoreText.transform.parent.gameObject.SetActive(true);
    }
    
}
