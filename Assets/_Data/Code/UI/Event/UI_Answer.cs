using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Answer : MonoBehaviour
{
    [SerializeField] private int indexQuestion;
    [SerializeField] private TextMeshProUGUI descriptionQuestion;
    [SerializeField] private Button buttonClick;
    [SerializeField] private Image image;

    private Action<int> onClicked;

    private void Awake()
    {
        descriptionQuestion = GetComponentInChildren<TextMeshProUGUI>();
        buttonClick = GetComponent<Button>();
        image = GetComponentInChildren<Image>();
    }
    void Start()
    {
        buttonClick.onClick.RemoveAllListeners();
        buttonClick.onClick.AddListener(OnAnswerClicked);
    }
    public void SetButton(bool value)
    {
        buttonClick.enabled = value;
    }
    public void SetUp(int index, string questionText, Action<int> onClickedCallback)
    {
        indexQuestion = index;
        descriptionQuestion.text = questionText;
        onClicked = onClickedCallback;
        SetButton(true);
        SetUpNormal();
    }
    public void SetUpNormal()
    {
        image.color = new Color(255f/255f, 255f/255f, 255f/255f);
    }
    public void SetUpTrue()
    {
        image.color = new Color(5f/255f ,255f/255f, 0f/255f);
    }
    public void SetUpFalse()
    {
        image.color = new Color(255f/255f, 0f/255f, 0f/255f);
    }
    void OnAnswerClicked()
    {
        onClicked?.Invoke(indexQuestion);
    }
}