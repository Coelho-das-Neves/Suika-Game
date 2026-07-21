using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawController : MonoBehaviour
{
    private Rigidbody2D rig;

    public Transform FruitSpot;

    public float speed;

    public float clampX;

    private bool isDragging = false;
    private float offsetX = 0f;

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
            HandleMouseInput();
        #else
            HandleTouchInput();
        #endif
    }

    void FixedUpdate()
    {
        if (isDragging)
        {
            float targetX = Mathf.Clamp(GetPointerWorldX() + offsetX, -clampX, clampX);
            Vector2 newPos = Vector2.Lerp(rig.position, new Vector2(targetX, rig.position.y), speed * Time.fixedDeltaTime);
            rig.MovePosition(newPos);
        }
    }

    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BeginDrag(ScreenToWorldX(Input.mousePosition));
        } else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    void HandleTouchInput()
    {
        if (Input.touchCount == 0)
        {
            isDragging = false;
            return;
        }

        Touch touch = Input.GetTouch(0);
        float worldX = ScreenToWorldX(touch.position);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                BeginDrag(worldX);
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                break;
            case TouchPhase.Ended:
                EndDrag();
                break;
        }
    }

    void BeginDrag(float pointerworldX)
    {
        isDragging = true;
        offsetX = rig.position.x - pointerworldX;
    }

    void EndDrag()
    {
        isDragging = false;
    }

    float GetPointerWorldX()
    {
        #if UNITY_EDITOR || UNITY_STANDALONE
            return ScreenToWorldX(Input.mousePosition);
        #else
            return Input.touchCount > 0 ? ScreenToWorldX(Input.GetTouch(0).position) : rig.position.x;
        #endif
    }

    float ScreenToWorldX(Vector2 screenPos)
    {
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, -Camera.main.transform.position.z));
        return worldPoint.x;
    }
}
