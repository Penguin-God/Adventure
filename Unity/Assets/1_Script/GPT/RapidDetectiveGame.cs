using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RapidDetectiveGame : MonoBehaviour
{
    [Header("Left - Location Buttons")]
    [SerializeField] Button officeButton;
    [SerializeField] Button loungeButton;

    [Header("Center - Collected Evidence List UI")]
    [SerializeField] Transform evidenceListParent;     // ScrollView Content
    [SerializeField] TMP_Text evidenceListLinePrefab;  // TMP_Text 프리팹(없으면 Text 하나 만들어서 연결)

    [Header("Right - Notebook UI")]
    [SerializeField] TMP_Dropdown suspectDropdown;
    [SerializeField] TMP_Dropdown weaponDropdown;
    [SerializeField] TMP_Dropdown timeDropdown;
    [SerializeField] Button accuseButton;
    [SerializeField] TMP_Text resultText;

    [Header("Location Popup UI")]
    [SerializeField] GameObject locationPanel;
    [SerializeField] TMP_Text locationTitleText;
    [SerializeField] Button closeLocationButton;
    [SerializeField] Transform locationEvidenceParent;  // 버튼들이 생성될 부모
    [SerializeField] EvidenceButton evidenceButtonPrefab;

    // --- Data ---------------------------------------------------------

    enum Location { Office, Lounge }

    [Serializable]
    class Evidence
    {
        public string Id;
        public Location Location;
        public string Title;
        public string Body;

        public string FullText => $"[{Title}] {Body}";
    }

    string[] suspects = { "경비 박", "인턴 이", "매니저 최" };
    string[] weapons = { "종이칼", "트로피", "약병" };
    string[] times = { "18:00~19:00", "19:00~20:00", "20:00~21:00" };

    // 정답(바꿔도 됨)
    readonly string answerSuspect = "매니저 최";
    readonly string answerWeapon = "트로피";
    readonly string answerTime = "19:00~20:00";

    readonly List<Evidence> allEvidence = new();
    readonly HashSet<string> discovered = new();

    // --- Unity --------------------------------------------------------

    void Awake()
    {
        // 버튼 연결
        officeButton.onClick.AddListener(() => OpenLocation(Location.Office));
        loungeButton.onClick.AddListener(() => OpenLocation(Location.Lounge));

        closeLocationButton.onClick.AddListener(CloseLocation);

        accuseButton.onClick.AddListener(Accuse);

        locationPanel.SetActive(false);
        resultText.text = "";
    }

    void Start()
    {
        BuildCaseData();
        SetupDropdowns();
        RenderCollectedEvidenceList();
    }

    void BuildCaseData()
    {
        // 사건: 사무실에서 피해자가 머리를 강타당해 사망.
        // 빠른 프로토타입이라 “논리”는 텍스트로만 제공하고,
        // 플레이어는 단서 읽고 직접 추리 후 드롭다운으로 지목.

        allEvidence.Clear();

        // --- Office (사무실) 단서 5개
        allEvidence.Add(new Evidence
        {
            Id = "O1",
            Location = Location.Office,
            Title = "파손된 트로피",
            Body = "책상 옆 트로피 받침이 깨져 있고, 금속 부분에 미세한 혈흔이 남아 있습니다."
        });
        allEvidence.Add(new Evidence
        {
            Id = "O2",
            Location = Location.Office,
            Title = "출입 로그",
            Body = "19:12에 '매니저 최' 카드로 사무실 문이 열렸습니다. 19:36에 다시 열렸습니다."
        });
        allEvidence.Add(new Evidence
        {
            Id = "O3",
            Location = Location.Office,
            Title = "피해자 메모",
            Body = "메모지에 '19시, 그 건으로 이야기하자. 최.' 라고 적혀 있습니다."
        });
        allEvidence.Add(new Evidence
        {
            Id = "O4",
            Location = Location.Office,
            Title = "약병",
            Body = "서랍에서 수면제 약병이 발견됐지만, 새것처럼 깨끗하고 지문이 없습니다."
        });
        allEvidence.Add(new Evidence
        {
            Id = "O5",
            Location = Location.Office,
            Title = "종이칼 위치",
            Body = "종이칼은 제자리에 정돈돼 있고 날에 이상 흔적이 없습니다."
        });

        // --- Lounge (휴게실) 단서 4개
        allEvidence.Add(new Evidence
        {
            Id = "L1",
            Location = Location.Lounge,
            Title = "CCTV 캡처",
            Body = "19:05~19:55 동안 '인턴 이'가 휴게실 자동판매기 앞을 거의 떠나지 않습니다."
        });
        allEvidence.Add(new Evidence
        {
            Id = "L2",
            Location = Location.Lounge,
            Title = "경비 순찰 기록",
            Body = "경비 박은 20:10에 1층 로비 순찰 체크를 완료했습니다(기계 기록)."
        });
        allEvidence.Add(new Evidence
        {
            Id = "L3",
            Location = Location.Lounge,
            Title = "깨진 받침 조각",
            Body = "휴게실 쓰레기통에서 트로피 받침 조각과 같은 재질의 조각이 나옵니다."
        });
        allEvidence.Add(new Evidence
        {
            Id = "L4",
            Location = Location.Lounge,
            Title = "닦아낸 흔적",
            Body = "휴게실 세면대 주변이 유독 젖어 있고, 걸레가 평소보다 축축합니다."
        });
    }

    void SetupDropdowns()
    {
        suspectDropdown.ClearOptions();
        weaponDropdown.ClearOptions();
        timeDropdown.ClearOptions();

        suspectDropdown.AddOptions(suspects.ToList());
        weaponDropdown.AddOptions(weapons.ToList());
        timeDropdown.AddOptions(times.ToList());

        suspectDropdown.value = 0;
        weaponDropdown.value = 0;
        timeDropdown.value = 0;
    }

    // --- Location UI --------------------------------------------------

    void OpenLocation(Location location)
    {
        locationPanel.SetActive(true);
        locationTitleText.text = location == Location.Office ? "사무실" : "휴게실";

        // 기존 버튼 제거
        for (int i = locationEvidenceParent.childCount - 1; i >= 0; i--)
            Destroy(locationEvidenceParent.GetChild(i).gameObject);

        // 해당 장소 단서 버튼 생성
        var list = allEvidence.Where(e => e.Location == location).ToList();
        foreach (var ev in list)
        {
            bool already = discovered.Contains(ev.Id);
            var btn = Instantiate(evidenceButtonPrefab, locationEvidenceParent);

            string title = already ? $"(수집됨) {ev.Title}" : ev.Title;
            btn.Setup(title, !already, () => DiscoverEvidence(ev.Id));
        }
    }

    void CloseLocation()
    {
        locationPanel.SetActive(false);
    }

    void DiscoverEvidence(string evidenceId)
    {
        if (!discovered.Add(evidenceId)) return;

        RenderCollectedEvidenceList();

        // 장소 화면 다시 그려서 버튼 비활성 갱신
        // (현재 열린 장소를 알기 위해 제목 기반으로 간단 처리)
        if (locationPanel.activeSelf)
        {
            if (locationTitleText.text.Contains("사무실"))
                OpenLocation(Location.Office);
            else
                OpenLocation(Location.Lounge);
        }
    }

    // --- Evidence List ------------------------------------------------

    void RenderCollectedEvidenceList()
    {
        // 기존 라인 제거
        for (int i = evidenceListParent.childCount - 1; i >= 0; i--)
            Destroy(evidenceListParent.GetChild(i).gameObject);

        if (discovered.Count == 0)
        {
            var line = Instantiate(evidenceListLinePrefab, evidenceListParent);
            line.text = "아직 단서를 수집하지 않았습니다.";
            return;
        }

        var collected = allEvidence
            .Where(e => discovered.Contains(e.Id))
            .OrderBy(e => e.Location)
            .ThenBy(e => e.Id);

        foreach (var ev in collected)
        {
            var line = Instantiate(evidenceListLinePrefab, evidenceListParent);
            line.text = ev.FullText;
        }
    }

    // --- Accuse -------------------------------------------------------

    void Accuse()
    {
        string pickSuspect = suspectDropdown.options[suspectDropdown.value].text;
        string pickWeapon = weaponDropdown.options[weaponDropdown.value].text;
        string pickTime = timeDropdown.options[timeDropdown.value].text;

        bool ok = pickSuspect == answerSuspect
               && pickWeapon == answerWeapon
               && pickTime == answerTime;

        if (ok)
        {
            resultText.text =
                "정답입니다!\n" +
                $"범인: {answerSuspect}\n흉기: {answerWeapon}\n시간: {answerTime}\n\n" +
                "이제 다음 사건(단서 텍스트만 교체)로 확장하면 됩니다.";
        }
        else
        {
            resultText.text =
                "오답입니다.\n" +
                $"선택: {pickSuspect} / {pickWeapon} / {pickTime}\n\n" +
                "단서를 다시 읽고 후보를 좁혀보세요.";
        }
    }
}
