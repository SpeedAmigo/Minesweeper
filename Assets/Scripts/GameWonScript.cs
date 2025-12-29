using UnityEngine;
using TMPro;

public class GameWonScript : MonoBehaviour
{
    [SerializeField] private GameObject gameWonPanel;
    
    [SerializeField] private TMP_Text bestTimeText;
    [SerializeField] private TMP_Text currentStreakText;

    private void EnableGameWonPanel()
    {
        gameWonPanel.SetActive(true);

        bestTimeText.text = StatisticManager.Instance.GetQuickestTime().ToString("F3");
        currentStreakText.text = StatisticManager.Instance.GetDayStreak().ToString();
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
