using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour
{

    [field: SerializeField]
    public KitchenObjectSO KitchenObjectSO { get; private set; }

    public ClearCounter ClearCounter 
    { 
        get
        { 
            return ClearCounter; 
        } 
        set 
        {
            if (ClearCounter != null)
            {
                ClearCounter.ClearKitchenObject();
            }

            ClearCounter = value;

            if (ClearCounter.HasKitchenObject())
            {
                Debug.LogError("Counter already has a KitchenObject!");
            }

            ClearCounter.SetKitchenObject(this);

            transform.parent = ClearCounter.GetCounterTopPoint();
            transform.localPosition = Vector3.zero;
        } 
    }

}
