using UnityEngine;
using TMPro;

public class GameWonScript : MonoBehaviour
{
    [SerializeField] private GameObject gameWonPanel;

    [SerializeField] private TMP_Text currentTimeText;
    [SerializeField] private TMP_Text bestTimeText;
    
    [SerializeField] private TMP_Text currentStreakText;

    private void EnableGameWonPanel()
    {
        gameWonPanel.SetActive(true);

        bestTimeText.text = FormatTime(StatisticManager.Instance.GetQuickestTime());
        currentTimeText.text = FormatTime(StatisticManager.Instance.GetCurrentTime());
        currentStreakText.text = StatisticManager.Instance.GetDayStreak().ToString();
    }
    
    private string FormatTime(float timeSeconds, bool threeDigitMilliseconds = true)
    {
        int minutes = Mathf.FloorToInt(timeSeconds / 60f);
        int seconds = Mathf.FloorToInt(timeSeconds % 60f);
        int milliseconds = Mathf.FloorToInt((timeSeconds - Mathf.Floor(timeSeconds)) * 1000f);

        if (threeDigitMilliseconds)
            return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        else
        {
            int centiseconds = Mathf.FloorToInt(milliseconds / 10f); // 0-99
            return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, centiseconds);
        }
    }
    
    private void OnEnable()
    {
        MinesweeperManager.OnGameWonEvent += EnableGameWonPanel;
    }

    private void OnDisable()
    {
        MinesweeperManager.OnGameWonEvent -= EnableGameWonPanel;
    }
}
