using UnityEngine;

public static class GameDataManager
{
    public static StageDataSO SelectedStageData { get; set; }
    public static StageDataSO NextStageData { get; set; } // 추가: 다음 스테이지 데이터
    public static int CurrentAbsoluteLevel { get; set; }

    public static int MaxClearedLevel
    {
        get => PlayerPrefs.GetInt("MaxClearedLevel", 0);
        set => PlayerPrefs.SetInt("MaxClearedLevel", value);
    }

    public static bool IsPremiumUnlocked
    {
        get => PlayerPrefs.GetInt("IsPremiumUnlocked", 0) == 1;
        set => PlayerPrefs.SetInt("IsPremiumUnlocked", value ? 1 : 0);
    }
}
