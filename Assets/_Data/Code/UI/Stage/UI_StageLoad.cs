using UnityEngine;
using UnityEngine.UI;

public class UI_StageLoad : MonoBehaviour
{
    [Header("Stage Info")]
    public int stageId; 

    [Header("UI")]
    public GameObject imageLock;
    public Transform starRoot;
    public Button button;

    void Awake()
    {
        if (button == null)
            button = GetComponent<Button>();
    }

    public void Refresh()
    {
        bool unlocked = IsUnlocked();
        imageLock.SetActive(!unlocked);
        //button.interactable = unlocked;

        UpdateStar();
    }

    void UpdateStar()
    {
        int star = GetStar();

        for (int i = 0; i < starRoot.childCount; i++)
        {
            starRoot.GetChild(i).gameObject.SetActive(i < star);
        }
    }

    bool IsUnlocked()
    {
        var progress = ProgressManager.Instance.progress;

     
        if (stageId % 100 == 1)
            return true;

        int prevStageId = stageId - 1;
        var prev = progress.stageResults
            .Find(s => s.stageId == prevStageId);

        return prev != null && prev.cleared;
    }

    int GetStar()
    {
        var result = ProgressManager.Instance.progress.stageResults
            .Find(s => s.stageId == stageId);

        return result != null ? result.star : 0;
    }

    public void OnClick()
    {
        Debug.Log($"Load stage {stageId}");
        
    }
}
