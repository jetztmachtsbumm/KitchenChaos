using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/BurningRecipe")]
public class BurningRecipeSO : ScriptableObject
{

    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burningTimerMax;

}
