using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static Action OnPlayerStep;

    private PlayerInput playerInput;
    private Vector2 moveInput;
    private bool canMove = true;
    
    [SerializeField] private GameObject hitPointVisualization; 

    private float moveTimer = 0; 
    [SerializeField] private float moveDuration = 0.5f;
    
    Vector3 startPosition;
    Vector3 targetPosition;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    
    private void Update()
    {
        moveInput = ReadMoveInput();
        
        if (moveInput != Vector2.zero && canMove)
        {
            if (CheckCollision(moveInput))
            {
                Debug.Log("Collision detected");
                canMove = true; 
                return;
            }
            
            StartCoroutine(Move(moveInput));
            OnPlayerStep?.Invoke();
        }
    }

    private Vector2 ReadMoveInput()
    { 
        Vector2 input = playerInput.actions["Move"].ReadValue<Vector2>();
        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            input = new Vector2(Mathf.Sign(input.x), 0);
        }
        else if (input == Vector2.zero)
        {
            input = Vector2.zero;
        }
        else 
        {
            input = new Vector2(0f, Mathf.Sign(input.y));
        }
        return input;
    }

    private bool CheckCollision(Vector2 moveDirection)
    {
        canMove = false;
        Vector2 hitPoint = (Vector2)transform.position + moveDirection;
        Vector2 overlapBoxSize = new Vector2(0.5f, 0.5f);
        
        hitPointVisualization.transform.position = hitPoint; 

        Collider2D boxCollider = Physics2D.OverlapBox(hitPoint, overlapBoxSize, 0);
        
        if(boxCollider != null && boxCollider.CompareTag("Wall")) 
            return true;
        
        return false; 
    }

    private IEnumerator Move(Vector2 moveDirection)
    {
        moveTimer = 0; 
        startPosition = transform.position;
        targetPosition = transform.position + (Vector3)moveDirection; 
        
        while (moveTimer < moveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, moveTimer / moveDuration);
            moveTimer += Time.deltaTime;
            
            yield return null; 
        }
        
        transform.position = targetPosition;
        canMove = true;
    }
}
