using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FruitScript : MonoBehaviour
{
    public FruitData fruitData;

    private Rigidbody2D rig;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = fruitData.fruitSprite;
    }
}
