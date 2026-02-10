using System.Collections.Generic;
using UnityEngine;

public class BattleResult : MonoBehaviour
{

    [Header("Transform")]
    public Transform backHeroExp;
    public Transform listHeroPrefab;
    [Header("Stage")]
    public StageConfig stageConfig;
    Dictionary<int, bool> listHeroStatus = new Dictionary<int, bool>();// true = live , false = dead
    public UI_StageReward uiStageReward;
    public int heroTotal;
    private void Awake()
    {
        stageConfig = StageContext.selectedStage;
        
    }
    void Start()
    {
       
        listHeroStatus.Clear();
        
    }

    public void SetExpPlus()
    {
        foreach (Transform child in backHeroExp)
        {
            var ui = child.GetComponent<UI_HeroExpPlus>();
            if (ui != null)
            {
                if (listHeroStatus[int.Parse(ui.name)])
                {
                    ui.SetExpPlus(stageConfig.expForAliveHero);
                }
                else
                {
                    ui.SetExpPlus(stageConfig.expForAliveHero / 2);
                }
            }

        }
    }
    public void SetList(int indexSlot, bool statusValue)
    {

        listHeroStatus[indexSlot] = statusValue;
    }
    
    public void CheckHeroesLost()
    {
        int activeCount = 0;
        int totalHero = listHeroPrefab.childCount;

        foreach (Transform child in listHeroPrefab)
        {
            if (child.gameObject.activeSelf)
            {
                activeCount++;
            }
        }

        if (activeCount >= totalHero || totalHero == 1)
        {
            uiStageReward.DisPlayStar(3);
        }
        else
        {
            switch (stageConfig.stageCondition)
            {
                case StageCondition.HeroLost:
                    int heroLostParam = stageConfig.conditionParam;
                    if((totalHero - activeCount) <= heroLostParam)
                    {
                        uiStageReward.DisPlayStar(2);
                    }
                    else
                    {
                        uiStageReward.DisPlayStar(1);
                    }
                    break;



            }
        }
           

    }
}
