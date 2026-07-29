using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("전체 챕터 및 스테이지 데이터")]
    public List<ChapterData> chapters;

    // 현재 플레이 중인 절대 레벨 (0부터 시작)
    public int CurrentAbsoluteLevel { get; set; }

    public int MaxClearedLevel
    {
        get => PlayerPrefs.GetInt("MaxClearedLevel", 0);
        set => PlayerPrefs.SetInt("MaxClearedLevel", value);
    }

    public bool IsPremiumUnlocked
    {
        get => PlayerPrefs.GetInt("IsPremiumUnlocked", 0) == 1;
        set => PlayerPrefs.SetInt("IsPremiumUnlocked", value ? 1 : 0);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public StageDataSO GetStageData(int absoluteLevel)
    {
        int chapterIdx = absoluteLevel / 10;
        int stageIdx = absoluteLevel % 10;

        if (chapterIdx >= 0 && chapterIdx < chapters.Count)
        {
            if (stageIdx >= 0 && stageIdx < chapters[chapterIdx].stages.Count)
            {
                return chapters[chapterIdx].stages[stageIdx];
            }
        }
        return null;
    }

    public bool HasNextStage() => GetStageData(CurrentAbsoluteLevel + 1) != null;

    // 특정 레벨이 광고/프리미엄으로 인해 잠겨있는지 확인
    public bool IsStageLocked(int absoluteLevel)
    {
        // 10레벨(2챕터) 이상이고, 프리미엄 해금이 안 되어 있으면 잠김
        return absoluteLevel >= 10 && !IsPremiumUnlocked;
    }
}