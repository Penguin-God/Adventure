using TMPro;
using UnityEngine;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance { get; private set; }

    [Header("툴팁 UI")]
    public RectTransform tooltipPanel;   // TooltipPanel 할당
    public TextMeshProUGUI tooltipText;             // TooltipText 할당
    public Vector2 offset = new Vector2(10, -10);  // 마우스 커서 기준 오프셋

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        tooltipPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        // 툴팁이 켜져 있을 때만, 패널을 마우스 위치로 옮김
        if (tooltipPanel.gameObject.activeSelf)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tooltipPanel.parent as RectTransform,
                Input.mousePosition,
                null,    // Screen Space - Overlay 땐 null
                out pos
            );
            tooltipPanel.anchoredPosition = pos + offset;
        }
    }

    public void Show(string message)
    {
        tooltipText.text = message;
        tooltipPanel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        tooltipPanel.gameObject.SetActive(false);
    }
}
