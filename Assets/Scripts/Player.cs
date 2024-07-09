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
    public float groundMovementStrength;
    public float airMovementStrength;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidbody;

    private float horizontalMovement;

    [SerializeField]
    private bool grounded;
    private bool jumpActive;

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
        float movementStrength = grounded ? groundMovementStrength : airMovementStrength;
        if (Math.Abs(horizontalMovement) > 0)
        {
            rigidbody.AddForce(new Vector2(horizontalMovement * movementStrength, 0));
        }
        else
        {
            if (rigidbody.velocity.x < 0)
            {
                rigidbody.AddForce(new Vector2(Math.Min(-rigidbody.velocity.x, movementStrength), 0));
            }
            else
            {
                rigidbody.AddForce(new Vector2(Math.Max(-rigidbody.velocity.x, -movementStrength), 0));
            }
        }
        rigidbody.velocity = new Vector2(Math.Clamp(rigidbody.velocity.x, -speed, speed), rigidbody.velocity.y);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            rigidbody.AddForce(new Vector2(rigidbody.velocity.x, jumpHeight * 10));
            jumpActive = true;
        }

        if (Input.GetButtonUp("Jump") && jumpActive)
        {
            if (!grounded && rigidbody.velocity.y > 0)
                rigidbody.AddForce(new Vector2(0, -jumpHeight * rigidbody.velocity.y));
            jumpActive = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tilemap"))
        {
            grounded = false;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Vector2.Dot(contact.normal, Vector2.up) > 0.9)
                {
                    grounded = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tilemap"))
        {
            grounded = false;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (Vector2.Dot(contact.normal, Vector2.up) > 0.9)
                {
                    grounded = true;
                    break;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Tilemap"))
        {
            grounded = false;
        }
    }
}
