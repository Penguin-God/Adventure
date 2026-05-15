using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class BedInteract : MonoBehaviour, IInteractable
{
    [Header("Dependencies")]
    public DayManager dayManager;

    public InteractType Type => InteractType.Input;

    public void Interact()
    {
        if (dayManager != null)
        {
            dayManager.GoToNextDay();
            Debug.Log("침대에서 잠을 자고 다음 날이 되었습니다!");
        }
        else
        {
            Debug.LogWarning("DayManager가 침대에 연결되지 않았습니다.");
        }
    }
}