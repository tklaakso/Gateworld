using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class Player : MonoBehaviour
{

    public static readonly float WIDTH = 1.0f;
    public static readonly float HEIGHT = 1.0f;

    public float speed;
    public float jumpHeight;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidbody;

    private float horizontalMovement;

    [SerializeField]
    private bool grounded;

    private Rigidbody2D lastCollisionBody;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        Sprite sprite = spriteRenderer.sprite;
        transform.localScale = new Vector2(WIDTH / sprite.bounds.size.x, HEIGHT / sprite.bounds.size.y);
        boxCollider.size = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        rigidbody.velocity = new Vector2(horizontalMovement * speed, rigidbody.velocity.y);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            rigidbody.AddForce(new Vector2(rigidbody.velocity.x, jumpHeight * 10));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tile>())
        {
            if (Vector2.Dot(collision.GetContact(0).normal, Vector2.up) > 0.9)
            {
                lastCollisionBody = collision.rigidbody;
                grounded = true;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tile>())
        {
            if (Vector2.Dot(collision.GetContact(0).normal, Vector2.up) > 0.9)
            {
                lastCollisionBody = collision.rigidbody;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Tile>() && collision.rigidbody.Equals(lastCollisionBody))
        {
            grounded = false;
        }
    }
}
