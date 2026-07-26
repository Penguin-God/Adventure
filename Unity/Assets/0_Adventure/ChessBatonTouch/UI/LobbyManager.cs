using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class ChapterData
{
    public string chapterName;
    public List<StageDataSO> stages = new List<StageDataSO>(10);
}

public class LobbyManager : MonoBehaviour
{
    [Header("챕터 및 스테이지 데이터")]
    public List<ChapterData> chapters;
    private int currentChapterIndex = 0; // 현재 화면에 띄운 챕터

    [Header("UI 연결")]
    public TMP_Text chapterTitleText;
    public Button[] stageButtons; // 10개의 스테이지 버튼을 에디터에서 연결
    public Button prevChapterBtn;
    public Button nextChapterBtn;

    [Header("광고 잠금 시스템")]
    public GameObject adLockPanel;    // 챕터가 잠겼을 때 버튼들을 가리는 패널
    public Button watchAdButton;      // 광고 보기 버튼

    void Start()
    {
        // 버튼 이벤트 연결
        prevChapterBtn.onClick.AddListener(ShowPrevChapter);
        nextChapterBtn.onClick.AddListener(ShowNextChapter);
        watchAdButton.onClick.AddListener(OnWatchAdClicked);

        // 첫 화면 렌더링
        UpdateLobbyUI();
    }

    private void UpdateLobbyUI()
    {
        ChapterData currentChapter = chapters[currentChapterIndex];
        chapterTitleText.text = currentChapter.chapterName;

        // 이전/다음 챕터 버튼 활성화 여부
        prevChapterBtn.interactable = currentChapterIndex > 0;
        nextChapterBtn.interactable = currentChapterIndex < chapters.Count - 1;

        // [광고 해금 체크] 2챕터(인덱스 1) 이상인데 광고를 안 봤다면 잠금 화면 표시
        if (currentChapterIndex > 0 && !GameDataManager.IsPremiumUnlocked)
        {
            adLockPanel.SetActive(true);
            LockAllStageButtons();
            return; // 잠겼으므로 아래의 스테이지 세팅은 건너뜀
        }

        adLockPanel.SetActive(false);
        SetupStageButtons(currentChapter);
    }

    private void LockAllStageButtons()
    {
        for (int i = 0; i < stageButtons.Length; i++)
        {
            if (stageButtons[i] != null)
            {
                stageButtons[i].interactable = false;
            }
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
            int stageIdx = i; // 클로저 문제 방지용 로컬 변수
            int absoluteLevel = (currentChapterIndex * 10) + stageIdx;
            bool isUnlocked = absoluteLevel <= GameDataManager.MaxClearedLevel;

            Button btn = stageButtons[i];
            btn.interactable = isUnlocked;
            btn.GetComponentInChildren<TMP_Text>().text = (stageIdx + 1).ToString();

            btn.onClick.RemoveAllListeners();
            // ★ 클릭 시 현재 챕터 인덱스와 스테이지 인덱스를 통째로 넘겨줍니다.
            btn.onClick.AddListener(() => OnStageSelected(currentChapterIndex, stageIdx, absoluteLevel));
        }
    }

    private void OnStageSelected(int chapterIdx, int stageIdx, int absoluteLevel)
    {
        GameDataManager.CurrentAbsoluteLevel = absoluteLevel;
        GameDataManager.SelectedStageData = chapters[chapterIdx].stages[stageIdx];

        // ★ 다음 스테이지 데이터 미리 계산해서 담아두기
        GameDataManager.NextStageData = null;
        if (stageIdx + 1 < chapters[chapterIdx].stages.Count)
        {
            // 같은 챕터의 다음 스테이지
            GameDataManager.NextStageData = chapters[chapterIdx].stages[stageIdx + 1];
        }
        else if (chapterIdx + 1 < chapters.Count && chapters[chapterIdx + 1].stages.Count > 0)
        {
            // 다음 챕터의 첫 번째 스테이지
            GameDataManager.NextStageData = chapters[chapterIdx + 1].stages[0];
        }

        SceneManager.LoadScene("Puzzle");
    }

    private void ShowNextChapter()
    {
        if (currentChapterIndex < chapters.Count - 1) currentChapterIndex++;
        UpdateLobbyUI();
    }

    private void ShowPrevChapter()
    {
        if (currentChapterIndex > 0) currentChapterIndex--;
        UpdateLobbyUI();
    }

    private void OnWatchAdClicked()
    {
        // TODO: 실제 광고 SDK 로직 호출 (완료 콜백에서 아래 코드 실행)
        Debug.Log("광고 시청 완료! 전체 챕터가 해금되었습니다.");
        GameDataManager.IsPremiumUnlocked = true;
        UpdateLobbyUI(); // 화면 갱신
    }
}