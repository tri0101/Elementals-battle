using System.Collections.Generic;
using UnityEngine;

public sealed class BattlefieldRegistry : MonoBehaviour
{
    public static BattlefieldRegistry Instance { get; private set; }

    // unit -> slotIndex (1..6)
    private readonly Dictionary<Transform, int> slotByRoot = new Dictionary<Transform, int>(64);

    // unit -> team tag ("Hero" / "Enemy") to avoid depending on GameObject.tag
    private readonly Dictionary<Transform, string> teamByRoot = new Dictionary<Transform, string>(64);

    private const string TagHero = "Hero";
    private const string TagEnemy = "Enemy";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Clear()
    {
        slotByRoot.Clear();
        teamByRoot.Clear();
    }

    public void Register(Transform unitRoot, int slotIndex1To6, string teamTag)
    {
        if (unitRoot == null) return;

        slotByRoot[unitRoot] = Mathf.Clamp(slotIndex1To6, 1, 6);
        teamByRoot[unitRoot] = teamTag;
    }

    // Backward compatible overload (will try to infer team from transform tag)
    public void Register(Transform unitRoot, int slotIndex1To6)
    {
        if (unitRoot == null) return;

        string inferred =
            unitRoot.CompareTag(TagHero) ? TagHero :
            unitRoot.CompareTag(TagEnemy) ? TagEnemy :
            string.Empty;

        Register(unitRoot, slotIndex1To6, inferred);
    }

    public bool TryGetSlotIndex(Transform unitRoot, out int slotIndex1To6)
    {
        if (unitRoot != null && slotByRoot.TryGetValue(unitRoot, out slotIndex1To6))
            return true;

        slotIndex1To6 = 0;
        return false;
    }

    public bool TryGetTeamTag(Transform unitRoot, out string teamTag)
    {
        if (unitRoot != null && teamByRoot.TryGetValue(unitRoot, out teamTag))
            return true;

        teamTag = string.Empty;
        return false;
    }
    public int FindEmptySlot(string teamTag)
    {
        if (string.IsNullOrEmpty(teamTag))
            return 0;

        for (int slot = 1; slot <= 6; slot++)
        {
            bool occupiedAlive = false;

            foreach (var kv in teamByRoot)
            {
                Transform root = kv.Key;
                if (root == null) continue;
                if (kv.Value != teamTag) continue;

                if (!slotByRoot.TryGetValue(root, out int s)) continue;
                if (s != slot) continue;

                var recv = root.GetComponentInChildren<HeroReceiveDamagee>();
                if (recv == null || !recv.IsDead)
                {
                    occupiedAlive = true;
                    break;
                }
            }

            if (!occupiedAlive)
                return slot;
        }

        return 0;
    }
    public List<int> FindListEmptySlots(string teamTag)
    {
        var emptySlots = new List<int>(6);

        if (string.IsNullOrEmpty(teamTag))
            return emptySlots;

        for (int slot = 1; slot <= 6; slot++)
        {
            bool occupiedAlive = false;

            foreach (var kv in teamByRoot)
            {
                Transform root = kv.Key;
                if (root == null) continue;
                if (kv.Value != teamTag) continue;

                if (!slotByRoot.TryGetValue(root, out int s)) continue;
                if (s != slot) continue;

                var recv = root.GetComponentInChildren<HeroReceiveDamagee>();
                if (recv == null || !recv.IsDead)
                {
                    occupiedAlive = true;
                    break;
                }
            }

            if (!occupiedAlive)
                emptySlots.Add(slot);
        }

        return emptySlots;
    }
    public List<Transform> GetAllUnits()
    {
        var list = new List<Transform>(slotByRoot.Count);
        foreach (var kv in slotByRoot)
            if (kv.Key != null)
                list.Add(kv.Key);
        list.Sort((a, b) =>
        {
            TryGetSlotIndex(a, out int slotA);
            TryGetSlotIndex(b, out int slotB);
            return slotA.CompareTo(slotB);
        });
        return list;
    }

    public List<Transform> GetUnitsByTeam(string teamTag)
    {
        var list = new List<Transform>(slotByRoot.Count);
        foreach (var kv in teamByRoot)
        {
            if (kv.Key == null) continue;
            if (kv.Value == teamTag)
                list.Add(kv.Key);
        }
        list.Sort((a, b) =>
        {
            TryGetSlotIndex(a, out int slotA);
            TryGetSlotIndex(b, out int slotB);
            return slotA.CompareTo(slotB);
        });
        return list;
    }

    public List<Transform> GetEnemyUnits(Transform casterRoot)
    {
        if (casterRoot == null)
            return new List<Transform>(0);

        string casterTeam;
        if (!TryGetTeamTag(casterRoot, out casterTeam))
        {
            // fallback to GameObject tag if not registered
            casterTeam = casterRoot.CompareTag(TagHero) ? TagHero :
                         casterRoot.CompareTag(TagEnemy) ? TagEnemy : string.Empty;
        }

        string enemyTeam = casterTeam == TagHero ? TagEnemy :
                           casterTeam == TagEnemy ? TagHero : string.Empty;

        if (string.IsNullOrEmpty(enemyTeam))
            return new List<Transform>(0);

        return GetUnitsByTeam(enemyTeam);
    }

    /// <summary>
    /// Row mapping (3 rows, each row has 2 slots):
    /// Row1: (1,4)  Row2: (2,5)  Row3: (3,6)
    /// </summary>
    public static int SlotToRow(int slotIndex1To6)
    {
        if (slotIndex1To6 == 6) return 3;
        int mod = slotIndex1To6 % 3;
        return mod == 0 ? 3 : mod;
    }

    /// <summary>
    /// Column mapping (2 columns, each column has 3 slots):
    /// Column1 (front): 1,2,3
    /// Column2 (back):  4,5,6
    /// </summary>
    public static int SlotToColumn(int slotIndex1To6)
    {
        return slotIndex1To6 <= 3 ? 1 : 2;
    }

    // ===== Helpers for selection =====

    public List<Transform> GetAliveUnitsInColumn(string teamTag, int columnIndex1To2)
    {
        var team = GetUnitsByTeam(teamTag);
        var list = new List<Transform>(team.Count);

        for (int i = 0; i < team.Count; i++)
        {
            var root = team[i];
            if (root == null) continue;

            if (!TryGetSlotIndex(root, out int slot)) continue;
            if (SlotToColumn(slot) != columnIndex1To2) continue;

            var recv = root.GetComponentInChildren<HeroReceiveDamagee>();
            if (recv != null && recv.IsDead) continue;

            list.Add(root);
        }

        return list;
    }

    public Transform GetRandomAliveUnit(List<Transform> source)
    {
        if (source == null || source.Count == 0)
            return null;

        // sanitize nulls
        for (int i = source.Count - 1; i >= 0; i--)
            if (source[i] == null)
                source.RemoveAt(i);

        if (source.Count == 0)
            return null;

        return source[Random.Range(0, source.Count)];
    }

    public float GetUnitSpeed(Transform unitRoot)
    {
        if (unitRoot == null) return 0f;

        var hc = unitRoot.GetComponent<HeroControl>();
        if (hc == null) return 0f;

        var stat = hc.GetComponent<HeroStatRuntime>();
        if (stat != null && stat.FinalStat != null)
            return stat.FinalStat.speed;

        return hc.HeroInfo != null ? hc.HeroInfo.speed : 0f;
    }
    public bool TryGetSlotIndex(HeroControl unit, string teamTag, out int slotIndex1To6)
    {
        slotIndex1To6 = 0;

        if (unit == null)
            return false;
        if (!string.IsNullOrEmpty(teamTag))
        {
            if (TryGetTeamTag(unit.transform, out string registeredTeam))
            {
                if (registeredTeam != teamTag)
                    return false;
            }
            else
            {
                if (!unit.CompareTag(teamTag))
                    return false;
            }
        }

        return TryGetSlotIndex(unit.transform, out slotIndex1To6);
    }
}