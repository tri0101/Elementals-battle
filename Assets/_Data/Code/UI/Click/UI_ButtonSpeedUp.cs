using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class UI_ButtonSpeedUp : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI modeText;
    [SerializeField] private Button buttonSpeedUp;
    string currentMode ;

    private void Awake()
    {
        
        buttonSpeedUp.onClick.AddListener(OnClick);
        currentMode = "x1";
        
    }

    void OnClick()
    {
        if(currentMode == "x1")
        {
            currentMode = "x2";
            modeText.text = "X 2.0";
            Time.timeScale = 2f;
        }
        else if(currentMode == "x2")
        {
            currentMode = "x3";
            modeText.text = "X 3.0";
            Time.timeScale = 3f;
        }
        else if(currentMode == "x3")
        {
            currentMode = "x1";
            modeText.text = "X 1.0";
            Time.timeScale = 1f;
        }
    }
}
