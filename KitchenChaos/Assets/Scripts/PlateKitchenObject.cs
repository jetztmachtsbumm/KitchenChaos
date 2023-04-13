using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject
{

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs
    {
        public KitchenObjectSO kitchenObject;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjects;

    private List<KitchenObjectSO> kitchenObjects = new List<KitchenObjectSO>();

    public bool TryAddIngredient(KitchenObjectSO kitchenObject)
    {
        if (validKitchenObjects.Contains(kitchenObject))
        {
            if (kitchenObjects.Contains(kitchenObject))
            {
                return false;
            }
            else
            {
                kitchenObjects.Add(kitchenObject);
                OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
                {
                    kitchenObject = kitchenObject
                });
                return true;
            }
        }

        return false;
    }

    public List<KitchenObjectSO> GetKitchenObjects()
    {
        return kitchenObjects;
    }

}
