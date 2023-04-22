using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;
    private FollowTransform followTransform;

    private void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

    public IKitchenObjectParent GetKitchenObjectParent()
    {
        return kitchenObjectParent;
    }

    public KitchenObjectSO GetKitchenObjectSO()
    {
        return kitchenObjectSO;
    }

    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent, bool clearKitchenObjectOnParent)
    {
        SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject(), clearKitchenObjectOnParent);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference, bool clearKitchenObjectOnParent)
    {
        SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference, clearKitchenObjectOnParent);
    }

    [ClientRpc]
    private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference, bool clearKitchenObjectOnParent)
    {
        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (this.kitchenObjectParent != null && clearKitchenObjectOnParent)
        {
            this.kitchenObjectParent.ClearKitchenObject();
        }

        this.kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject())
        {
            Debug.LogError("IKitchenObjectParent already has a KitchenObject!");
        }

        this.kitchenObjectParent.SetKitchenObject(this);

        followTransform.SetTargetTransform(kitchenObjectParent.GetKitchenObjectFollowPoint());
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    public void ClearKitchenObjectOnParent()
    {
        kitchenObjectParent.ClearKitchenObject();
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject)
    {
        if(this is PlateKitchenObject thisPlateKitchenObject)
        {
            plateKitchenObject = thisPlateKitchenObject;
            return true;
        }

        plateKitchenObject = null;
        return false;
    }

    public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        MultiplayerManager.Instance.SpawnKitchenObject(kitchenObjectSO, kitchenObjectParent);
    }

    public static void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        MultiplayerManager.Instance.DestroyKitchenObject(kitchenObject);
    }
}
