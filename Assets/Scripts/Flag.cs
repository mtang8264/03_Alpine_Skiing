using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public FlagColor flagColor;
    public Direction direction;

    Collider2D collider;
    public SpriteRenderer spriteRenderer;

    public Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((direction == Direction.HORIZONTAL && player.position.y < transform.position.y) ||
            (direction == Direction.VERTICAL && player.position.y < transform.position.y - 3))
        {
            spriteRenderer.color = new Color(255, 0, 0, 0.25f);
            GameManager.instance.SendMessage("Miss");
            Destroy(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            GameManager.instance.SendMessage("Score");
        }
        spriteRenderer.enabled = false;
        Destroy(this);
    }

    public enum FlagColor { RED, BLUE };
    public enum Direction { HORIZONTAL, VERTICAL };
}
