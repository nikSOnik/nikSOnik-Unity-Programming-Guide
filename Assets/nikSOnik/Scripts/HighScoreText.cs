using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    void Start()
    {
        Highscore highscore = MainManager.Instance.Highscore;
        text.text = $"Highscore: {highscore.username} {highscore.score}!";
    }
}