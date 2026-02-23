using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    [SerializeField] private HeroDatabase heroDatabase;
    public HeroDatabase HeroDatabase => heroDatabase;
    [SerializeField] private StageDatabase stageDatabase;
    public StageDatabase StageDatabase => stageDatabase;
    [SerializeField] private ItemDatabase itemDatabase;
    public ItemDatabase ItemDatabase => itemDatabase;
    [SerializeField] private ChapterDatabase chapterDatabase;
    public ChapterDatabase ChapterDatabase => chapterDatabase;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
