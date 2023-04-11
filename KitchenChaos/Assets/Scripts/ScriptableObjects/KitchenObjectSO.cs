using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/KitchenObject")]
public class KitchenObjectSO : ScriptableObject
{

    public Transform prefab;
    public Sprite sprite;
    public string objectName;

}
