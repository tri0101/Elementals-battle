using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    public List<RuneData> ownedRunes = new List<RuneData>();

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasRune(RuneData rune)
    {
        return ownedRunes.Contains(rune);
    }

    public void AddRune(RuneData rune)
    {
        if (!ownedRunes.Contains(rune))
            ownedRunes.Add(rune);
    }
}
