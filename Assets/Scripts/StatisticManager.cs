using UnityEngine;
using System;
using System.Linq.Expressions;

public class StatisticManager : MonoBehaviour
{
    public static StatisticManager Instance;
    
    [SerializeField] private float currentTime;
    [SerializeField] private float quickestTime;
    [SerializeField] private int dayStreak;
    [SerializeField] private string lastDateString;
    
    private DateTime lastDate;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        
        // XD
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        dayStreak = PlayerPrefs.GetInt("DayStreak", 1);
        quickestTime = PlayerPrefs.GetFloat("QuickestTime", float.MaxValue);
        
        lastDateString = PlayerPrefs.GetString("LastDate", "");
        
        if (!string.IsNullOrEmpty(lastDateString))
        {
            lastDate = DateTime.ParseExact(lastDateString, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
        }
        else
        {
            lastDate = DateTime.Now.Date;
        }
    }
    
    public void UpdateDayStreak()
    {
        var today = DateTime.Now.Date;
        int dayDifference = (today - lastDate).Days;

        if (dayDifference == 1)
        {
            SetDayStreak(dayStreak + 1);
        }
        else if (dayDifference > 1)
        {
            SetDayStreak(1);
        }
    }
    
    public void SetDayStreak(int streak)
    {
        if (streak < 0) return;
        
        dayStreak = streak;
        lastDate = DateTime.Now.Date;
        
        PlayerPrefs.SetInt("DayStreak", dayStreak);
        PlayerPrefs.SetString("LastDate", lastDate.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
    }

    public int GetDayStreak()
    {
        return dayStreak;
    }

    public void SetQuickestTime(float time)
    {
        if (time >= GetQuickestTime()) return;
        
        quickestTime = time;
        PlayerPrefs.SetFloat("QuickestTime", quickestTime);
        PlayerPrefs.Save();
    }

    public void SetCurrentTime(float time)
    {
        currentTime = time;
    }
    
    public float GetQuickestTime()
    {
        return quickestTime;
    }

    public float GetCurrentTime()
    {
        return currentTime;
    }

    [ContextMenu("ResetSaves")]
    public void DeleteSaves()
    {
        PlayerPrefs.DeleteAll();
    }
}
