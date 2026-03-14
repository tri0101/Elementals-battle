using System;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public DateTime resetManaLastTime;
    public DateTime resetShopLastTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadTime();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LoadTime()
    {
        if (PlayerPrefs.HasKey("ManaTime"))
            resetManaLastTime = DateTime.Parse(PlayerPrefs.GetString("ManaTime"));
        else
            resetManaLastTime = DateTime.Now;

        if (PlayerPrefs.HasKey("ShopTime"))
            resetShopLastTime = DateTime.Parse(PlayerPrefs.GetString("ShopTime"));
        else
            resetShopLastTime = DateTime.Now;
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

        PlayerPrefs.SetString("ManaTime", resetManaLastTime.ToString());
        PlayerPrefs.Save();
    }

    // =========================
    // SHOP RESET
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

            PlayerPrefs.SetString("ShopTime", resetShopLastTime.ToString());
            PlayerPrefs.Save();

            return true;
        }

        return false;
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetString("ManaTime", resetManaLastTime.ToString());
        PlayerPrefs.SetString("ShopTime", resetShopLastTime.ToString());
        PlayerPrefs.Save();
    }

    // =========================
    // TEST EDITOR
    // =========================

    void Update()
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
    }
}