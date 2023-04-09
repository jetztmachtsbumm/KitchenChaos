using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private KitchenObject kitchenObject;
    private Transform counterTopPoint;

    private void Awake()
    {
        counterTopPoint = transform.Find("CounterTopPoint");
    }

    public void Interact()
    {
        if(kitchenObject == null)
        {
            Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
            kitchenObjectTransform.GetComponent<KitchenObject>().ClearCounter = this;
            kitchenObjectTransform.localPosition = Vector3.zero;

            kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
            kitchenObject.ClearCounter = this;
        }
    }

    public Transform GetCounterTopPoint()
    {
        return counterTopPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
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
