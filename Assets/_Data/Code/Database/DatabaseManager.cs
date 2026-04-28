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
    [SerializeField] private FightSoulDatabase fightSoulDatabase;
    public FightSoulDatabase FightSoulDatabase => fightSoulDatabase;
    [SerializeField] private ShopItemDatabase shopItemDatabase;
    public ShopItemDatabase ShopItemDatabase => shopItemDatabase;

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
    private void Start()
    {
        Init();
    }
    private void Init()
    {
        chapterDatabase.Init();
        heroDatabase.Init();
        stageDatabase.Init();
        fightSoulDatabase.Init();
        shopItemDatabase.Init();
        itemDatabase.Init();
    }

}
