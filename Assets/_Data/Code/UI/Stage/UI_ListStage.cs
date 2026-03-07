using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class UI_ListStage : MonoBehaviour
{
    [SerializeField] private Transform levelContent;
    [SerializeField] private List<UI_StageLoad> stages = new();

    [SerializeField] private Button prevButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private UI_PanelReward panelReward;

    PlayerProgress playerProgress;
    int currentChapter;
    int maxChapter;

    void Start()
    {
        playerProgress = ProgressManager.Instance.progress;
        maxChapter = levelContent.childCount;

        LoadChapter(playerProgress.currentChapter);
        
    }

    void LoadChapter(int chapterIndex)
    {
        stages.Clear();
        currentChapter = chapterIndex;

        foreach (Transform chapter in levelContent)
        {
            chapter.gameObject.SetActive(false);
        }

      
        Transform currentChapterTf =
            levelContent.Find(chapterIndex.ToString());

        if (currentChapterTf == null)
        {
            Debug.LogError($"Chapter {chapterIndex} not found");
            return;
        }

        currentChapterTf.gameObject.SetActive(true);

       
        foreach (Transform level in currentChapterTf)
        {
            UI_StageLoad stageLoad = level.GetComponent<UI_StageLoad>();
            if (stageLoad != null)
            {
                stages.Add(stageLoad);
            }
        }

        stages.Sort((a, b) => a.StageId.CompareTo(b.StageId));

        SetupButtons();
        RefreshAll();
        panelReward.OnClickButtonReward(currentChapter);
        panelReward.LoadUI(currentChapter);
        panelReward.LoadStarReward(currentChapter);
       
    }

    void SetupButtons()
    {
        prevButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();

        prevButton.gameObject.SetActive(currentChapter > 1);
        nextButton.gameObject.SetActive(currentChapter < maxChapter);

        if (currentChapter > 1)
            prevButton.onClick.AddListener(OnClickPrev);

        if (currentChapter < maxChapter)
            nextButton.onClick.AddListener(OnClickNext);
    }

    void OnClickPrev()
    {
        playerProgress.currentChapter--;
        LoadChapter(playerProgress.currentChapter);
        // ProgressManager.Instance.Save();
    }

    void OnClickNext()
    {
        playerProgress.currentChapter++;
        LoadChapter(playerProgress.currentChapter);
        // ProgressManager.Instance.Save();
    }

    public void RefreshAll()
    {
        foreach (var stage in stages)
        {
            stage.Refresh();
        }
    }

    public void OnStageCleared()
    {
        RefreshAll();
    }
   
}
