using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    // Public fields kept for compatibility with existing code
    public DateTime resetManaLastTime;
    public DateTime resetShopLastTime;

    // New: daily mission reset (00:00)
    public DateTime resetDailyMissionLastTime;

    private const string MANA_TIME_KEY = "ManaTime";
    private const string SHOP_TIME_KEY = "ShopTime";
    private const string DAILY_MISSION_TIME_KEY = "DailyMissionTime";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadTime();
    }

    private void LoadTime()
    {
        resetManaLastTime = LoadDateTimeOrNow(MANA_TIME_KEY);
        resetShopLastTime = LoadDateTimeOrNow(SHOP_TIME_KEY);
        resetDailyMissionLastTime = LoadDateTimeOrNow(DAILY_MISSION_TIME_KEY);
    }

    private static DateTime LoadDateTimeOrNow(string key)
    {
        if (!PlayerPrefs.HasKey(key))
            return DateTime.Now;

        string value = PlayerPrefs.GetString(key);
        if (DateTime.TryParse(value, out DateTime dt))
            return dt;

        return DateTime.Now;
    }

    private static void SaveDateTime(string key, DateTime dt)
    {
        PlayerPrefs.SetString(key, dt.ToString());
    }

    // =========================
    // MANA
    // =========================

    public int GetManaSecondsPassed()
    {
        return (int)(DateTime.Now - resetManaLastTime).TotalSeconds;
    }

    public void AdvanceManaTime(int seconds)
    {
        resetManaLastTime = resetManaLastTime.AddSeconds(seconds);
        SaveDateTime(MANA_TIME_KEY, resetManaLastTime);
        PlayerPrefs.Save();
    }

    // =========================
    // SHOP RESET (0/6/12/18h)
    // =========================

    public DateTime GetLastResetTime()
    {
        DateTime now = DateTime.Now;

        DateTime reset0 = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);
        DateTime reset6 = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0);
        DateTime reset12 = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0);
        DateTime reset18 = new DateTime(now.Year, now.Month, now.Day, 18, 0, 0);

        if (now >= reset18) return reset18;
        if (now >= reset12) return reset12;
        if (now >= reset6) return reset6;

        return reset0;
    }

    public bool ShouldResetShop()
    {
        DateTime lastReset = GetLastResetTime();

        if (resetShopLastTime < lastReset)
        {
            resetShopLastTime = DateTime.Now;
            SaveDateTime(SHOP_TIME_KEY, resetShopLastTime);
            PlayerPrefs.Save();
            return true;
        }

        return false;
    }

    // =========================
    // DAILY MISSION RESET (00:00)
    // =========================

    public bool ShouldResetDailyMission()
    {
        DateTime now = DateTime.Now;
        DateTime todayMidnight = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0);

        // If last mission reset happened before today's midnight => crossed day boundary
        if (resetDailyMissionLastTime < todayMidnight)
        {
            resetDailyMissionLastTime = now;
            SaveDateTime(DAILY_MISSION_TIME_KEY, resetDailyMissionLastTime);
            PlayerPrefs.Save();
            return true;
        }

        return false;
    }

    private void OnApplicationQuit()
    {
        SaveDateTime(MANA_TIME_KEY, resetManaLastTime);
        SaveDateTime(SHOP_TIME_KEY, resetShopLastTime);
        SaveDateTime(DAILY_MISSION_TIME_KEY, resetDailyMissionLastTime);
        PlayerPrefs.Save();
    }

    // =========================
    // TEST EDITOR
    // =========================

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            resetManaLastTime = DateTime.Now.AddMinutes(-10);
            Debug.Log("Test mana offline 10 phút");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            resetShopLastTime = DateTime.Now.AddHours(-7);
            Debug.Log("Test shop vượt reset");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            resetDailyMissionLastTime = DateTime.Now.AddDays(-1);
            Debug.Log("Test daily mission vượt 0h");
        }
    }
}