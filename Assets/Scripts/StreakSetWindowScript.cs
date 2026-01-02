using TMPro;
using UnityEngine;

public class StreakSetWindowScript : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputField;
    
    public void SetStreak()
    {
        int streakToSet = int.Parse(inputField.text);

        if (streakToSet <= 0)
        {
            streakToSet = 1;
        }
        
        StatisticManager.Instance.SetDayStreak(streakToSet);
    }
}
