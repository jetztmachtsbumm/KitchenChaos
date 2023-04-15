using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent
{

    public static event EventHandler OnObjectDropped;

    private Transform counterTopPoint;
    private KitchenObject kitchenObject;

    protected virtual void Awake()
    {
        counterTopPoint = transform.Find("CounterTopPoint");
    }

    public virtual void Interact(Player player)
    {

    }

    public virtual void AltInteract(Player player)
    {

    }

    public Transform GetKitchenObjectFollowPoint()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if(kitchenObject != null)
        {
            OnObjectDropped?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

}
