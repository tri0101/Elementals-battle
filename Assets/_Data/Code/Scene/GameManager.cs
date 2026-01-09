using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public HeroData selectedHero;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void OnClickLoad()
    {
        if (selectedHero == null)
        {
            Debug.LogWarning("Chưa chọn hero!");
            return;
        }

        SceneManager.LoadScene("SampleScene");
    }
}
