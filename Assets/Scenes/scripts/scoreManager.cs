using UnityEngine;
using UnityEngine.UI;
using TMPro; // 如果你使用TextMeshPro

public class ScoreManager : MonoBehaviour
{
    // 单例模式，方便从任何地方访问
    public static ScoreManager Instance { get; private set; }

    [Header("UI引用")]
    public TMP_Text scoreText; // 如果是TextMeshPro

    [Header("护盾设置")]
    public float currentScore = 0;
    public string scorePrefix = "护盾: ";

    void Awake()
    {
        // 单例初始化
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 可选：场景切换时保留
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateScoreDisplay();
    }

    // 更新分数显示
    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = scorePrefix + Mathf.RoundToInt(currentScore).ToString();
        }
    }

    // 增加分数
    public void AddScore(float amount)
    {
        currentScore += amount;
        UpdateScoreDisplay();

        // 可选：播放音效或特效
        Debug.Log($"分数增加 {amount}，当前分数: {currentScore}");
    }

    // 减少分数
    public void SubtractScore(float amount)
    {
        //currentScore = Mathf.Max(0, currentScore - amount); // 确保分数不为负
        currentScore -= amount;
        UpdateScoreDisplay();
        Debug.Log($"分数减少 {amount}，当前分数: {currentScore}");
    }

    // 设置分数
    public void SetScore(float newScore)
    {
        currentScore = Mathf.Max(0, newScore);
        UpdateScoreDisplay();
    }

    // 重置分数
    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreDisplay();
    }

    // 获取当前分数
    public float GetCurrentScore()
    {
        return currentScore;
    }
}
