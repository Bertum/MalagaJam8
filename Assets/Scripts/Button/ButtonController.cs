using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private Sprite redButton, redButtonPushed;
    private bool pushed;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        redButton = Resources.Load<Sprite>("Sprites/Buttons/RedButton");
        redButtonPushed = Resources.Load<Sprite>("Sprites/Buttons/RedButtonPushed");
        pushed = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !pushed)
        {
            spriteRenderer.sprite = redButtonPushed;
            pushed = true;
        }
    }
}
