using TMPro;
using UnityEngine;

public class StatisticsWindowScript : MonoBehaviour
{
    [SerializeField] private TMP_Text streakText;
    [SerializeField] private TMP_Text bestTimeText;

    private void OnEnable()
    {
        bestTimeText.text = HelperMethods.FormatTime(StatisticManager.Instance.GetQuickestTime());
        streakText.text = StatisticManager.Instance.GetDayStreak().ToString();
    }
}
