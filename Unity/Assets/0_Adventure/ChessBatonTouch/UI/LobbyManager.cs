using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    private int currentChapterIndex = 0; // 현재 화면에 띄운 챕터

    [Header("UI 연결")]
    public TMP_Text chapterTitleText;
    public Button[] stageButtons; // 10개의 스테이지 버튼
    public Button prevChapterBtn;
    public Button nextChapterBtn;

    [Header("광고 잠금 시스템")]
    public GameObject adLockPanel;
    public Button watchAdButton;

    void Start()
    {
        prevChapterBtn.onClick.AddListener(ShowPrevChapter);
        nextChapterBtn.onClick.AddListener(ShowNextChapter);
        watchAdButton.onClick.AddListener(OnWatchAdClicked);

        UpdateLobbyUI();
    }

    private void UpdateLobbyUI()
    {
        // LevelManager에서 챕터 데이터를 가져옴
        var chapters = LevelManager.Instance.chapters;
        ChapterData currentChapter = chapters[currentChapterIndex];
        chapterTitleText.text = currentChapter.chapterName;

        prevChapterBtn.interactable = currentChapterIndex > 0;
        nextChapterBtn.interactable = currentChapterIndex < chapters.Count - 1;

        // LevelManager를 통해 잠금 여부 확인 (해당 챕터의 첫 번째 레벨 기준)
        if (LevelManager.Instance.IsStageLocked(currentChapterIndex * 10))
        {
            adLockPanel.SetActive(true);
            LockAllStageButtons();
            return;
        }

        adLockPanel.SetActive(false);
        SetupStageButtons(currentChapter);
    }

    private void LockAllStageButtons()
    {
        foreach (var btn in stageButtons)
        {
            if (btn != null) btn.interactable = false;
        }
    }

    private void SetupStageButtons(ChapterData chapter)
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            if (i >= chapter.stages.Count)
            {
                stageButtons[i].gameObject.SetActive(false);
                continue;
            }

            stageButtons[i].gameObject.SetActive(true);
            int stageIdx = i;
            int absoluteLevel = (currentChapterIndex * 10) + stageIdx;

            // 해금 여부 확인
            bool isUnlocked = absoluteLevel <= LevelManager.Instance.MaxClearedLevel;

            Button btn = stageButtons[i];
            btn.interactable = isUnlocked;
            btn.GetComponentInChildren<TMP_Text>().text = (stageIdx + 1).ToString();

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnStageSelected(absoluteLevel));
        }
    }

    private void OnStageSelected(int absoluteLevel)
    {
        // 데이터 복사 로직 제거! 단순히 현재 레벨만 갱신하고 씬 이동
        LevelManager.Instance.CurrentAbsoluteLevel = absoluteLevel;
        SceneManager.LoadScene("Puzzle");
    }

    private void ShowNextChapter()
    {
        if (currentChapterIndex < LevelManager.Instance.chapters.Count - 1) currentChapterIndex++;
        UpdateLobbyUI();
    }

    private void ShowPrevChapter()
    {
        if (currentChapterIndex > 0) currentChapterIndex--;
        UpdateLobbyUI();
    }

    private void OnWatchAdClicked()
    {
        Debug.Log("광고 시청 완료! 전체 챕터가 해금되었습니다.");
        LevelManager.Instance.IsPremiumUnlocked = true;
        UpdateLobbyUI();
    }
}