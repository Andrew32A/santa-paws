using UnityEngine;
using TMPro;

public class GameOverScore : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;

    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            finalScoreText.text = "Your Final Score: " + ScoreManager.Instance.score;
        }
        else
        {
            finalScoreText.text = "No Score Found";
        }
    }
}
