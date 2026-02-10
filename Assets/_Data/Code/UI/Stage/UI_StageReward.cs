using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_StageReward : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI conditionText;
    public StageConfig stageConfig;
    public Transform starRoot;
    Color earnedColor = new Color32(255, 255, 255, 255);
    Color notEarnedColor = new Color32(158, 101, 101, 255);
    private void OnEnable()
    {
        stageConfig = StageContext.selectedStage;
        SetTextCondition();
    }
    public void SetTextCondition()
    {
        switch(stageConfig.stageCondition)
        {
            case StageCondition.HeroLost:
                int heroLostParam = stageConfig.conditionParam;
                conditionText.text = "Heroes Lost <= " + heroLostParam;
                break;

            
           
        }
    }
    public void DisPlayStar(int starReward)
    {
        int index = 0;

        foreach (Transform child in starRoot)
        {
            Image img = child.GetComponent<Image>();
            if (img == null) continue;

            
            if (index < starReward)
                img.color = earnedColor;
            else
                img.color = notEarnedColor;

            index++;
        }
    }
}
