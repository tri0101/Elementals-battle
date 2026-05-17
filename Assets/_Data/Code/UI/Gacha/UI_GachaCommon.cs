using UnityEngine;
using UnityEngine.UI;

public class UI_GachaCommon : MonoBehaviour
{
    [SerializeField] private Button buttonOpenRate;
    [SerializeField] private Button buttonReward;
    [SerializeField] private Button buttonRate;
    [SerializeField] private Button buttonCloseRate;


    [SerializeField] private Transform panelRate;
    [SerializeField] private Transform panelReward;
    [SerializeField] private Transform panelOpenRate;
    private void Start()
    {
        buttonOpenRate.onClick.AddListener(OnClickOpenRate);
        buttonCloseRate.onClick.AddListener(OnClickCloseRate);
        buttonReward.onClick.AddListener(OnClickReward);
        buttonRate.onClick.AddListener(OnClickRate);
    }
    void OnClickOpenRate()
    {
        panelOpenRate.gameObject.SetActive(true);
    }
    void OnClickCloseRate()
    {
        panelOpenRate.gameObject.SetActive(false);
    }
    void OnClickReward()
    {
        if(!panelReward.gameObject.activeSelf)
        {
            panelRate.gameObject.SetActive(false);
            panelReward.gameObject.SetActive(true);
        }
        
    }
    void OnClickRate()
    {
        if(!panelRate.gameObject.activeSelf)
        {
            panelReward.gameObject.SetActive(false);
            panelRate.gameObject.SetActive(true);
        }
    }

}
