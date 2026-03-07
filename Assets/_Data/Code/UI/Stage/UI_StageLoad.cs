using UnityEngine;
using UnityEngine.UI;

public class UI_StageLoad : MonoBehaviour
{
    [Header("Stage Info")]
    [SerializeField] private int stageId; 
    public int StageId => stageId;

    [Header("UI")]
    [SerializeField] private GameObject imageLock;
    [SerializeField] private Transform starRoot;
    [SerializeField] private Button button;
    [SerializeField] private UI_PanelDetailStage detailPanel;
    [Header("Star Colors")]
    public Color earnedColor = new Color32(255, 215, 0, 255);      // vàng
    public Color notEarnedColor = new Color32(158, 101, 101, 255); // tối
    [SerializeField] private bool unLocked;
    [SerializeField] private int starsEarned;
    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);

        //if (stageId > ProgressManager.Instance.progress.currentStageId)
        //{
        //    isClear = false;
        //}
    }

  
    public void Refresh()
    {
        unLocked = IsUnlocked();
        imageLock.SetActive(!unLocked);
        button.enabled = unLocked;

        UpdateStar();
    }

    void UpdateStar()
    {
        starsEarned = GetStar();
        
        for (int i = 0; i < starRoot.childCount; i++)
        {
            Image starImage = starRoot.GetChild(i).GetComponent<Image>();
            if (starImage == null) continue;

            if (i < starsEarned)
                starImage.color = earnedColor;      
            else
                starImage.color = notEarnedColor;  
        }
    }

    bool IsUnlocked()
    {
        return stageId <= ProgressManager.Instance.progress.currentStageId;
    }

    int GetStar()
    {
        StageResultData found =
            ProgressManager.Instance.progress.stageResults
            .Find(x => x.stageId == stageId);

        if (found == null)
            return 0;

        return found.star;
    }


    public void OnClick()
    {
        detailPanel.SetStars(starsEarned);
        detailPanel.OnLoadUI(stageId);
       
    }
}
