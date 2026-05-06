using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager Instance { get; private set; }

    [Header("데이터")]
    public List<TimeFrameData> allFrames;

    [Header("UI 레퍼런스")]
    public Transform content;            // ScrollView > Content
    public GameObject itemPrefab;        // TimeFrameItem 프리팹

    List<TimeFrameData> sortedFrames = new List<TimeFrameData>();
    List<GameObject> spawnedItems = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RefreshTimeline();
    }

    // 1) 타임프레임 정렬 후 UI 생성
    public void RefreshTimeline()
    {
        // 기존 제거
        spawnedItems.ForEach(i => Destroy(i));
        spawnedItems.Clear();

        // order 기준 오름차순 정렬
        sortedFrames = allFrames.OrderBy(f => f.order).ToList();

        // UI 뿌리기
        foreach (var frame in sortedFrames)
        {
            var go = Instantiate(itemPrefab, content);
            var img = go.GetComponentInChildren<Image>();
            img.sprite = frame.snapshotImage;

            var btn = go.GetComponent<Button>();

            // 이동 버튼(←,→)이 프리팹에 있다면 각각에 MoveLeft/Right 연결
            go.transform.Find("BtnLeft").GetComponent<Button>()
              .onClick.AddListener(() => MoveFrame(frame, -1));
            go.transform.Find("BtnRight").GetComponent<Button>()
              .onClick.AddListener(() => MoveFrame(frame, +1));

            spawnedItems.Add(go);
        }
    }

    // 3) 프레임 순서 바꾸기
    public void MoveFrame(TimeFrameData frame, int direction)
    {
        int idx = sortedFrames.IndexOf(frame);
        int newIdx = Mathf.Clamp(idx + direction, 0, sortedFrames.Count - 1);
        if (newIdx == idx) return;

        // order 값 스왑
        int tmp = sortedFrames[newIdx].order;
        sortedFrames[newIdx].order = frame.order;
        frame.order = tmp;

        // 다시 UI 갱신
        RefreshTimeline();
    }
}
