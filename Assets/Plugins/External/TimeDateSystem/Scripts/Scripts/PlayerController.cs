using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private Sprite tilledSprite;
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private GameObject highlightSprite;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private GridLayout grid;

    private Rigidbody2D rb;

    private Vector2 moveDir;

    float lastY, lastX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        var pos = grid.WorldToCell(interactionPoint.position);
        var tile = tilemap.GetCellCenterWorld(pos);

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Tile newTile = new Tile();
            newTile.sprite = tilledSprite;

            tilemap.SetTile(pos, newTile);
        }

        if (tile != null)
        {
            highlightSprite.transform.position = tile;
            if (!highlightSprite.activeInHierarchy)highlightSprite.SetActive(true);
        }
        else highlightSprite.SetActive(false);

        
    }

    bool flipped;
    private void HandleMovement()
    {
        float x = 0f;
        float y = 0f;

        if (Keyboard.current.wKey.isPressed)
        {
            y = 1f;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            y = -1f;
        }
        if (Keyboard.current.aKey.isPressed)
        {
            x = -1f;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            x = 1f;
        }

        playerAnimator.SetFloat("Horizontal", x);
        playerAnimator.SetFloat("Vertical", y);
        playerAnimator.SetFloat("Speed", rb.velocity.sqrMagnitude);

        if (x != 0 || y != 0)interactionPoint.position = new Vector2(transform.position.x + x, transform.position.y + y);

        moveDir = new Vector2(x, y);
        
    }

    private void FixedUpdate()
    {
        rb.velocity = moveDir.normalized * moveSpeed;
    }
}
