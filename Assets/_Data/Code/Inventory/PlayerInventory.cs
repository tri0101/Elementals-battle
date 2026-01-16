using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RuneStack
{
    public RuneData rune;
    public int count;
}

public class heroInventory : MonoBehaviour
{
    public static heroInventory Instance;

    public List<RuneStack> ownedRunes = new List<RuneStack>();

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

    // ================= CORE =================

    public bool HasRune(RuneData rune)
    {
        return GetRuneStack(rune) != null;
    }

    public int GetRuneCount(RuneData rune)
    {
        RuneStack stack = GetRuneStack(rune);
        return stack != null ? stack.count : 0;
    }

    public void AddRune(RuneData rune, int amount = 1)
    {
        RuneStack stack = GetRuneStack(rune);

        if (stack != null)
        {
            stack.count += amount;
        }
        else
        {
            ownedRunes.Add(new RuneStack
            {
                rune = rune,
                count = amount
            });
        }
    }

    public bool RemoveRune(RuneData rune, int amount = 1)
    {
        RuneStack stack = GetRuneStack(rune);
        if (stack == null) return false;

        stack.count -= amount;

        if (stack.count <= 0)
        {
            ownedRunes.Remove(stack);
        }

        return true;
    }

    // ================= HELPER =================

    private RuneStack GetRuneStack(RuneData rune)
    {
        foreach (var stack in ownedRunes)
        {
            if (stack.rune == rune)
                return stack;
        }
        return null;
    }
}
