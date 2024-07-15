using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{

    public static readonly float WIDTH = 0.5f;
    public static readonly float HEIGHT = 0.5f;

    public float speed;
    public float jumpHeight;
    public float groundMovementStrength;
    public float airMovementStrength;
    public float zoomDecay;
    public float zoomStrength;
    public float zoomEdgeSmoothing;
    public float minZoom;
    public float maxZoom;
    public GameObject mainCamera;
    public GameObject game;

    private InventoryManager inventory;

    private Camera camera;

    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidbody;

    private float horizontalMovement;

    [SerializeField]
    private bool grounded;
    private bool jumpActive;

    private float zoomVelocity;
    private float zoom = 5;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        rigidbody = GetComponent<Rigidbody2D>();
        camera = mainCamera.GetComponent<Camera>();
        inventory = game.GetComponent<InventoryManager>();
        Sprite sprite = spriteRenderer.sprite;
        transform.localScale = new Vector2(WIDTH / sprite.bounds.size.x, HEIGHT / sprite.bounds.size.y);
        boxCollider.size = new Vector2(sprite.bounds.size.x, sprite.bounds.size.y);
    }

    // Update is called once per frame
    void Update()
    {
        bool inventoryOpen = inventory.IsOpen();
        mainCamera.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
        zoom = Math.Clamp(zoom, minZoom, maxZoom);
        camera.orthographicSize = zoom;
        horizontalMovement = inventoryOpen ? 0 : Input.GetAxisRaw("Horizontal");
        float movementStrength = (grounded ? groundMovementStrength : airMovementStrength) * Time.deltaTime * 1000;
        if (Math.Abs(horizontalMovement) > 0)
        {
            rigidbody.velocity += new Vector2(horizontalMovement * movementStrength, 0);
        }
        else
        {
            if (rigidbody.velocity.x < 0)
            {
                rigidbody.velocity += new Vector2(Math.Min(-rigidbody.velocity.x, movementStrength), 0);
            }
            else
            {
                rigidbody.velocity += new Vector2(Math.Max(-rigidbody.velocity.x, -movementStrength), 0);
            }
        }
        rigidbody.velocity = new Vector2(Math.Clamp(rigidbody.velocity.x, -speed, speed), rigidbody.velocity.y);

        if (Input.GetButtonDown("Jump") && grounded && !inventoryOpen)
        {
            rigidbody.AddForce(new Vector2(rigidbody.velocity.x, jumpHeight * 10));
            jumpActive = true;
        }

        if (Input.GetButtonUp("Jump") && jumpActive && !inventoryOpen)
        {
            if (!grounded && rigidbody.velocity.y > 0)
                rigidbody.AddForce(new Vector2(0, -jumpHeight * rigidbody.velocity.y));
            jumpActive = false;
        }

        if (!inventoryOpen)
            zoomVelocity += -Input.mouseScrollDelta.y * zoomStrength * Mathf.Lerp(0.5f, 1, (zoom - minZoom) / (maxZoom - minZoom));
        zoomVelocity *= zoomDecay;
        if (zoomVelocity < 0 && zoom - minZoom <= -zoomVelocity * zoomEdgeSmoothing)
        {
            zoomVelocity = -(zoom - minZoom) / zoomEdgeSmoothing;
        }
        if (zoomVelocity > 0 && maxZoom - zoom <= zoomVelocity * zoomEdgeSmoothing)
        {
            zoomVelocity = (maxZoom - zoom) / zoomEdgeSmoothing;
        }
        zoom += zoomVelocity;
        if (Input.GetMouseButtonDown(1))
        {
            Item item = inventory.GetSelectedToolbarInventorySlot().GetItem();
            if (item.quantity == 0)
                return;
            if (inventory.GetSelectedToolbarInventorySlot().GetItem().Activate(Input.mousePosition))
            {
                if (item.disposable)
                {
                    inventory.GetSelectedToolbarInventorySlot().Consume(1);
                }
            }
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
