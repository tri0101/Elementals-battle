using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ListQuestion : MonoBehaviour
{
    [SerializeField] private TriviaQuestionType type;
    [SerializeField] private TriviaQuestionSO listQuestion;
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button nextQuestionButton;
    [SerializeField] private Transform panelQuestion;
    [SerializeField] private Transform panelEnd;
    [SerializeField] Transform groupAnswer;
    int currentQuestion = 1;

    private void Awake()
    {
        nextQuestionButton.onClick.RemoveAllListeners();
        nextQuestionButton.onClick.AddListener(OnClickNextQuestion);
    }
    void Start()
    {
        SetUpQuestion();
    }

    void SetUpQuestion()
    {
        this.currentQuestion = TriviaQuestionState.Instance.GetCurrentQuestionByType(type);
        if(currentQuestion == 22)
        {
            panelQuestion.gameObject.SetActive(false);
            panelEnd.gameObject.SetActive(true);
            return;
        }
        TriviaQuestion currentTriviaQuestion =
            listQuestion.GetTriviaQuestion(currentQuestion);

        questionText.text = "Question " + currentQuestion.ToString() + ": " + currentTriviaQuestion.questionText;
        if (currentTriviaQuestion != null)
        {
            for (int i = 0; i < 4; i++)
            {
                UI_Answer ui_Answer = groupAnswer.GetChild(i).GetComponent<UI_Answer>();
                ui_Answer.SetUp(i, currentTriviaQuestion.options[i], OnAnswerSelected);
            }
        }
        nextQuestionButton.gameObject.SetActive(false);
    }

    private void OnAnswerSelected(int answerIndex)
    {
        TriviaQuestion currentTriviaQuestion = listQuestion.GetTriviaQuestion(currentQuestion);
        if (currentTriviaQuestion == null)
            return;

      
        bool isCorrect = answerIndex == currentTriviaQuestion.correctOptionIndex;
        for (int i = 0; i < 4; i++)
        {
            UI_Answer ui_Answer = groupAnswer.GetChild(i).GetComponent<UI_Answer>();
            if(i == currentTriviaQuestion.correctOptionIndex)
            {
                ui_Answer.SetUpTrue();
            }
            else if(i == answerIndex)
            {
                ui_Answer.SetUpFalse();
            }
            ui_Answer.SetButton(false);
        }
        
        
        TriviaQuestionState.Instance.SetProgress(type, currentTriviaQuestion.questionID, isCorrect);
        nextQuestionButton.gameObject.SetActive(true);

    }
    void OnClickNextQuestion()
    {
        SetUpQuestion();
    }
}