using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FruitData", menuName = "Fruit Data")]
public class FruitData : ScriptableObject
{
    public string fruitName;
    public Sprite fruitSprite;
    public int fruitScore;
    public float radius;

    public FruitData nextFruit;
}
