using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
                AddIngredientServerRpc(MultiplayerManager.Instance.GetKitchenObjectSOIndex(kitchenObject));
                return true;
            }
        }

        return false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int kitchenObjectSOIndex)
    {
        AddIngredientClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void AddIngredientClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjectSO kitchenObject = MultiplayerManager.Instance.GetkitchenObjectSOFromIndex(kitchenObjectSOIndex);
        kitchenObjects.Add(kitchenObject);
        OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs
        {
            kitchenObject = kitchenObject
        });
    }

    public List<KitchenObjectSO> GetKitchenObjects()
    {
        return kitchenObjects;
    }

}
