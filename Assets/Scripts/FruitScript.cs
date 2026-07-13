using UnityEngine;

public enum FruitType
{
    Cereja,
    Morango,
    Uva,
    Maca,
    Melao,
    Abacaxi,
    Melancia
}

[RequireComponent(typeof(Rigidbody2D))]
public class FruitScript : MonoBehaviour
{
    public GameObject nextFruitPrefab;
    public FruitType fruitType;
    private bool merged = false;


    private Rigidbody2D rig;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Fruit"))
            return;

        FruitScript otherFruit = collision.gameObject.GetComponent<FruitScript>();

        if (otherFruit == null)
            return;

        if (merged || otherFruit.merged)
            return;

        if (otherFruit.fruitType != fruitType)
            return;

        merged = true;
        otherFruit.merged = true;

        Vector2 spawnPos = (transform.position + otherFruit.transform.position) / 2f;

        Instantiate(nextFruitPrefab, spawnPos, Quaternion.identity);

        Destroy(gameObject);
        Destroy(otherFruit.gameObject);
    }
}
