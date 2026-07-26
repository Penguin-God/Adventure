using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultUI : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject clearPanel;       // 클리어 시 나타날 반투명 팝업 창
    public Button lobbyButton;          // 로비로 가기 버튼
    public Button nextStageButton;      // 다음 스테이지 버튼

    void Start()
    {
        // clearPanel.SetActive(false);
        lobbyButton.onClick.AddListener(GoToLobby);
        nextStageButton.onClick.AddListener(GoToNextStage);
    }

    // 킹을 잡았을 때 GameBoardUI 등에서 이 함수를 호출해 주세요!
    public void OnStageCleared()
    {
        // 1. 최고 클리어 레벨 갱신 (저장)
        if (GameDataManager.CurrentAbsoluteLevel >= GameDataManager.MaxClearedLevel)
        {
            GameDataManager.MaxClearedLevel = GameDataManager.CurrentAbsoluteLevel + 1;
        }

        // 2. 클리어 UI 띄우기
        clearPanel.SetActive(true);

        // 3. 다음 스테이지 버튼 활성화/비활성화 처리
        // 데이터가 없거나, 다음 스테이지가 2챕터 이상인데 광고를 안 봤다면 잠금 처리
        bool hasNextData = GameDataManager.NextStageData != null;
        bool isNextStageLocked = (GameDataManager.CurrentAbsoluteLevel + 1) >= 10 && !GameDataManager.IsPremiumUnlocked;

        if (!hasNextData || isNextStageLocked)
        {
            nextStageButton.interactable = false;
        }
        else
        {
            nextStageButton.interactable = true;
        }
    }

    private void GoToLobby()
    {
        SceneManager.LoadScene("Lobby");
    }

    private void GoToNextStage()
    {
        // 다음 스테이지 데이터를 '현재 스테이지'로 덮어씌움
        GameDataManager.SelectedStageData = GameDataManager.NextStageData;
        GameDataManager.CurrentAbsoluteLevel++;

        // 현재 씬을 다시 로드하여 새 스테이지 시작 (재활용)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}