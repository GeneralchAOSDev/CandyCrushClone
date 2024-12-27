using UnityEngine;
using DG.Tweening;

public class Icon : MonoBehaviour
{
    private const float DRAG_THRESHOLD = 0.5f;
    private const float MAX_DRAG_DISTANCE = 1.0f;
    private const float ANIMATION_DURATION = 0.3f;
    private const int GRID_COLUMNS = 6;

    [SerializeField] private float dragSpeed = 0.5f;

    private Vector3 dragStartPosition;
    private Vector3 originalPosition;
    private Vector2Int gridPosition;
    private Grid grid;
    private Animator animator;
    private bool isDragging;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        grid = GameObject.Find("background")?.GetComponent<Grid>();

        if (grid == null)
        {
            Debug.LogError($"Grid component not found for {gameObject.name}");
            enabled = false;
            return;
        }

        // Calculate grid position from object name
        int iconIndex = ParseIconIndex(gameObject.name);
        gridPosition = new Vector2Int(
            iconIndex % GRID_COLUMNS,
            iconIndex / GRID_COLUMNS
        );
    }

    private int ParseIconIndex(string objectName)
    {
        string numberString = new string(System.Array.FindAll(
            objectName.ToCharArray(),
            char.IsDigit
        ));

        return int.TryParse(numberString, out int result) ? result : 0;
    }

    private void OnMouseDown()
    {
        if (!enabled) return;

        isDragging = true;
        originalPosition = transform.position;
        dragStartPosition = GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 currentMousePos = GetMouseWorldPosition();
        Vector3 dragDelta = currentMousePos - dragStartPosition;

        // Determine primary drag direction
        if (Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
        {
            dragDelta.y = 0;
        }
        else
        {
            dragDelta.x = 0;
        }

        // Clamp drag distance
        dragDelta = Vector3.ClampMagnitude(dragDelta, MAX_DRAG_DISTANCE);

        // Animate to new position
        Vector3 targetPosition = originalPosition + dragDelta;
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            Time.deltaTime * dragSpeed
        );
    }

    private void OnMouseUp()
    {
        if (!isDragging) return;

        isDragging = false;
        Vector3 dragDelta = GetMouseWorldPosition() - dragStartPosition;

        // Return to original position
        transform.DOMove(originalPosition, ANIMATION_DURATION);

        // If drag distance is too small, ignore the swap
        if (dragDelta.magnitude < DRAG_THRESHOLD)
        {
            return;
        }

        // Calculate swap direction
        Vector2Int targetPosition = gridPosition;

        if (Mathf.Abs(dragDelta.x) > Mathf.Abs(dragDelta.y))
        {
            // Horizontal swap
            int direction = dragDelta.x > 0 ? 1 : -1;
            if (IsValidHorizontalSwap(direction))
            {
                targetPosition.x += direction;
            }
        }
        else
        {
            // Vertical swap - Note the inverted direction here
            int direction = dragDelta.y > 0 ? -1 : 1; // Changed this line to fix up/down swapping
            if (IsValidVerticalSwap(direction))
            {
                targetPosition.y += direction;
            }
        }

        // Perform swap if target position is different
        if (targetPosition != gridPosition)
        {
            grid.Swap(
                gridPosition.x, gridPosition.y,
                targetPosition.x, targetPosition.y
            );
        }
    }

    private bool IsValidHorizontalSwap(int direction)
    {
        int newX = gridPosition.x + direction;
        return newX >= 0 && newX < GRID_COLUMNS;
    }

    private bool IsValidVerticalSwap(int direction)
    {
        int newY = gridPosition.y + direction;
        return newY >= 0 && newY < 8; // Using 8 for ROWS
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return mousePos;
    }
}