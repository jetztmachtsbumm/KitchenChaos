using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Recipe")]
public class RecipeSO : ScriptableObject
{

    public List<KitchenObjectSO> kitchenObjects;
    public string recipeName;

}
