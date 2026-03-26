using TMPro;
using UnityEngine;

public class UI_CanvasTotalDamage : MonoBehaviour
{
    [SerializeField] private Transform damageAndText;
    [SerializeField] TextMeshProUGUI totalDamageText;

    public void UpdateTotalDamage(float totalDamage)
    {
        totalDamageText.text = totalDamage.ToString();
    }
    public void Show()
    {
        damageAndText.gameObject.SetActive(true);
    }
    public void Hide()
    {
        damageAndText.gameObject.SetActive(false);
    }
}
