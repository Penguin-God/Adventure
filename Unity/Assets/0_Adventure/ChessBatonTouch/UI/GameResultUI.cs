using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameResultUI : MonoBehaviour
{
    [Header("UI 연결")]
    public GameObject clearPanel;
    public Button lobbyButton;
    public Button nextStageButton;

    void Start()
    {
        lobbyButton.onClick.AddListener(GoToLobby);
        nextStageButton.onClick.AddListener(GoToNextStage);
    }

    public void OnStageCleared()
    {
        int currentLevel = LevelManager.Instance.CurrentAbsoluteLevel;

        // 최고 클리어 레벨 갱신
        if (currentLevel >= LevelManager.Instance.MaxClearedLevel)
        {
            LevelManager.Instance.MaxClearedLevel = currentLevel + 1;
        }

        clearPanel.SetActive(true);

        // 다음 스테이지 존재 여부 및 잠금 여부 확인
        int nextLevel = currentLevel + 1;
        bool hasNextStage = LevelManager.Instance.HasNextStage();
        bool isNextStageLocked = LevelManager.Instance.IsStageLocked(nextLevel);

        // 다음 스테이지 버튼 활성화/비활성화 처리
        nextStageButton.interactable = hasNextStage && !isNextStageLocked;
    }

    void GoToLobby() => SceneManager.LoadScene("Lobby");

    void GoToNextStage()
    {
        // 단순히 현재 레벨을 1 올리고 씬을 다시 로드하면 끝!
        LevelManager.Instance.CurrentAbsoluteLevel++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}