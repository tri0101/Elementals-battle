using NUnit.Framework.Internal.Commands;
using UnityEngine;
using UnityEngine.UI;

public class UI_PanelMission : MonoBehaviour
{
    [SerializeField] private Transform panelDailyTask;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject taskItem;
    [SerializeField] private Button closeButton;
    public void OnClickPanel()
    {
        ClearItem();
        panelDailyTask.gameObject.SetActive(true);
        SetUp();
    }
    void Start()
    {
        closeButton.onClick.RemoveAllListeners();
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }
    void SetUp()
    {

        var tasks = DailyTaskManager.Instance.GetDailyTaskProgress();
        foreach (var task in tasks)
        {
            var taskData = DailyTaskManager.Instance.GetDailyTask(task.taskID);
            if (task.isClaimed) return;
            var item = Instantiate(taskItem, content);
            item.GetComponent<UI_MissonTask>().SetUp(taskData, task);
        }
    }
    void ClearItem()
    {
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }
}
