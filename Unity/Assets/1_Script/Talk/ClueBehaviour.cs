using UnityEngine;

public class ClueBehaviour : MonoBehaviour
{
    public ClueTypeSO type;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public string ID => type.id;

    void SetAlpha(float a)
    {
        var c = spriteRenderer.color;
        c.a = a;
        spriteRenderer.color = c;
    }

    public void Reveal(float a)
    {
        gameObject.SetActive(true);
        SetAlpha(a);
    }
}
