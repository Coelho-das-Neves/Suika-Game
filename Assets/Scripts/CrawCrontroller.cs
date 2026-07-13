using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrawCrontroller : MonoBehaviour
{
    private Rigidbody2D rig;

    public Transform FruitSpot;

    public float speed;

    public float clampX;

    void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");

        Debug.Log(horizontal);

        transform.Translate(Vector2.right * horizontal * speed * Time.deltaTime);
    }
}
