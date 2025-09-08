using UnityEngine;

public class _DashAfterImage : MonoBehaviour
{
    public float duration = 0.2f;
    public float timer;

    private SpriteRenderer sprite;
    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        timer = duration;
    }
    void Update()
    {
        timer -= Time.deltaTime;

        Color c = sprite.color;
        c.a = Mathf.Lerp(0f, 0.5f, timer / duration);
        sprite.color = c;

        if (timer < 0)
        {
            Destroy(gameObject);
        }
    }
}
