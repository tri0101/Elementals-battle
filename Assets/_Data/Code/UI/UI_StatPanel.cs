using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_StatPanel : MonoBehaviour
{
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject statTextPrefab;

    // Lưu text theo từng RuneStat
    private Dictionary<RuneStat, TextMeshProUGUI> statTextMap = new();

    /// <summary>
    /// Nhận stat tổng từ RunePage và hiển thị
    /// </summary>
    public void UpdateStats(Dictionary<RuneStat, float> totalStats)
    {
        // Tạo / cập nhật stat đang có
        foreach (var stat in totalStats)
        {
            if (!statTextMap.TryGetValue(stat.Key, out var text))
            {
                GameObject go = Instantiate(statTextPrefab, contentParent);
                text = go.GetComponent<TextMeshProUGUI>();
                statTextMap.Add(stat.Key, text);
            }

            text.gameObject.SetActive(true);
            text.text = $"{GetStatName(stat.Key)}: {stat.Value}";
        }

        // Ẩn stat không còn
        foreach (var pair in statTextMap)
        {
            if (!totalStats.ContainsKey(pair.Key))
            {
                pair.Value.gameObject.SetActive(false);
            }
        }
    }

    private string GetStatName(RuneStat stat)
    {
        return stat switch
        {
            RuneStat.PhysicalDmg => "Physical Dmg",
            RuneStat.MagicalDmg => "Magical Dmg",
            RuneStat.HP => "Máu",
            RuneStat.PhysicalArmor => "Physical Arm",
            RuneStat.MagicalArmor => "Magical Arm",
            _ => stat.ToString()
        };
    }
}
