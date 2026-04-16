using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "DBS/Chapter Database")]
public class ChapterDatabase : ScriptableObject
{
    public List<ChapterConfig> chapters;

    private Dictionary<int, ChapterConfig> chapterDict;

    public void Init()
    {
        chapterDict = new Dictionary<int, ChapterConfig>();

        foreach (var chapter in chapters)
        {
            chapterDict[chapter.chapterID] = chapter;
        }
    }
    public ChapterConfig GetChapter(int chapterID)
    {
        if (chapterDict == null) Init();

        return chapterDict.TryGetValue(chapterID, out var stage)
            ? stage
            : null;
    }

}
