using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class AccountProgress
{
    public int level = 1;
    public int exp = 0;
}
[Serializable]
public class AccountInfo
{
    public string namePlayer;
    public Image image;
}

public class AccountManager : Subject
{
    public static AccountManager Instance { get; private set; }

    private const string DEFAULT_PLAYER_NAME = "Abc123";

    [SerializeField] private AccountProgress accountP = new AccountProgress();
    public AccountProgress AccountP => accountP;

    [SerializeField] private AccountInfo accountI = new AccountInfo();
    public AccountInfo AccountI => accountI;

    [SerializeField] private AccountLevelConfig levelConfig;
    private const int DIAMOND_PER_LEVEL = 100;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);


        if (accountI == null)
            accountI = new AccountInfo();

        if (string.IsNullOrWhiteSpace(accountI.namePlayer))
            accountI.namePlayer = DEFAULT_PLAYER_NAME;

        if (accountP == null)
            accountP = new AccountProgress();

        if (accountP.level < 1) accountP.level = 1;
        if (accountP.exp < 0) accountP.exp = 0;
    }

    public int GetExpToNextLevel(int level, out bool isMaxLevel)
    {
        isMaxLevel = false;

        if (levelConfig == null || levelConfig.expPerLevel == null || levelConfig.expPerLevel.Length == 0)
            return level * 100;

        int index = Mathf.Max(0, level - 1);

        if (index >= levelConfig.expPerLevel.Length)
        {
            isMaxLevel = true;
            return 0;
        }

        int need = levelConfig.expPerLevel[index];
        if (need < 0) need = 0;
        return need;
    }

    public void SetAccount(AccountProgress loaded)
    {
        accountP = loaded ?? new AccountProgress();

        if (accountP.level < 1) accountP.level = 1;
        if (accountP.exp < 0) accountP.exp = 0;

        NotifyObservers();
    }
    public void SetNewName(string newName)
    {
        accountI.namePlayer = newName;
        NotifyObservers();
    }

    public void AddExp(int amount)
    {
        if (amount <= 0) return;

        int startLevel = accountP.level;
        accountP.exp += amount;

        while (true)
        {
            int need = GetExpToNextLevel(accountP.level, out bool isMax);
            if (isMax)
            {
                accountP.exp = 0;
                break;
            }

            if (need == 0)
            {
                accountP.level++;
                accountP.exp = 0;
                continue;
            }

            if (accountP.exp < need)
                break;

            accountP.exp -= need;
            accountP.level++;
        }

        int levelGained = Mathf.Max(0, accountP.level - startLevel);
        int diamondAmount = levelGained * DIAMOND_PER_LEVEL;

        if (diamondAmount > 0 && UI_LevelUp.Instance != null)
            UI_LevelUp.Instance.CallUILevelUp(diamondAmount);

        NotifyObservers();
    }

}