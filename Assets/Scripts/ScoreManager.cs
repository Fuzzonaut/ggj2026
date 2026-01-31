using UnityEngine;
using TMPro; 
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score = 0;
    
  
    public TextMeshProUGUI scoreText; 
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        UpdateScoreText();
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }
}