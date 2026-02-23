using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class UI_PanelReward : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalStarText;


    [SerializeField] private UI_ListStarReward listStarReward;
    [SerializeField] private Button buttonReward10;
    [SerializeField] private Button buttonReward20;
    [SerializeField] private Button buttonReward30;
    [SerializeField] private Button buttonNotReward10;
    [SerializeField] private Button buttonNotReward20;
    [SerializeField] private Button buttonNotReward30;

    public void LoadStarReward(int currentChapter)
    {
        int starRewrad = ProgressManager.Instance.GetStarInChapter(currentChapter);
        totalStarText.text = $"{starRewrad}" + "/30";
    }
    public void OnClickButtonReward(int currentChapter)
    {
        buttonReward10.onClick.RemoveAllListeners();
        buttonReward20.onClick.RemoveAllListeners();
        buttonReward30.onClick.RemoveAllListeners();
        buttonReward10.onClick.AddListener(() => ShowPanel(currentChapter, 0));
        buttonReward20.onClick.AddListener(() => ShowPanel(currentChapter, 1));
        buttonReward30.onClick.AddListener(() => ShowPanel(currentChapter, 2));
        buttonNotReward10.onClick.RemoveAllListeners();
        buttonNotReward20.onClick.RemoveAllListeners();
        buttonNotReward30.onClick.RemoveAllListeners();
        buttonNotReward10.onClick.AddListener(() => ShowPanel(currentChapter, 0));
        buttonNotReward20.onClick.AddListener(() => ShowPanel(currentChapter, 1));
        buttonNotReward30.onClick.AddListener(() => ShowPanel(currentChapter, 2));

    }

    void ShowPanel(int currentChapter, int index)
    {
        listStarReward.gameObject.SetActive(true);
        listStarReward.OnClick(currentChapter, index);
    }

    public void LoadUI(int chapterID)
    {
        buttonNotReward10.gameObject.SetActive(!ProgressManager.Instance.IsClaimed(chapterID, 0));
        buttonReward10.gameObject.SetActive(ProgressManager.Instance.IsClaimed(chapterID, 0));
        buttonNotReward20.gameObject.SetActive(!ProgressManager.Instance.IsClaimed(chapterID, 1));
        buttonReward20.gameObject.SetActive(ProgressManager.Instance.IsClaimed(chapterID, 1));
        buttonNotReward30.gameObject.SetActive(!ProgressManager.Instance.IsClaimed(chapterID, 2));
        buttonReward30.gameObject.SetActive(ProgressManager.Instance.IsClaimed(chapterID, 2));
    }
}
