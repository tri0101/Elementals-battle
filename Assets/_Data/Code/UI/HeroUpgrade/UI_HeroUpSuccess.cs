using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_HeroUpSuccess : MonoBehaviour
{
    public Transform panelHeroUp;
    public Transform textPowerPrev;
    public Transform textPowerAfter;
    public Button button;
    void Awake()
    {
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnCloseClicked);
        }
    }
    void OnCloseClicked()
    {
        gameObject.SetActive(false);
    }

    public void ShowSucces(GameObject preHero, GameObject heroAfter, int powerPre, int powerAfter)
    {
        gameObject.SetActive(true);

        if (panelHeroUp != null)
        {
           
            foreach (Transform ch in panelHeroUp)
            {
                if (ch == null) continue;
                Destroy(ch.gameObject);
            }

          
            if (preHero != null)
            {
                var prev = Instantiate(preHero, panelHeroUp);
                prev.gameObject.SetActive(true);
            }

         
            if (heroAfter != null)
            {
                var next = Instantiate(heroAfter, panelHeroUp);
                next.gameObject.SetActive(true);
            }
        }

        
        if (textPowerPrev != null)
        {
            var txt = textPowerPrev.GetComponent<TextMeshProUGUI>();
            if (txt == null) txt = textPowerPrev.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = powerPre >= 0 ? powerPre.ToString() : "-";
        }

        if (textPowerAfter != null)
        {
            var txt2 = textPowerAfter.GetComponent<TextMeshProUGUI>();
            if (txt2 == null) txt2 = textPowerAfter.GetComponentInChildren<TextMeshProUGUI>();
            if (txt2 != null) txt2.text = powerAfter >= 0 ? powerAfter.ToString() : "-";
        }
    }
}