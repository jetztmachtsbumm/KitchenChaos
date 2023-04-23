using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MultiplayerManager : NetworkBehaviour
{
    
    public static MultiplayerManager Instance { get; private set; }

    [SerializeField] private KitchenObjectListSO kitchenObjectList;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("There is more than one MultiplayerManager object active in thr scene!");
            Destroy(gameObject);
        }
        Instance = this;
    }

    public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
    }

    public void DestroyKitchenObject(KitchenObject kitchenObject)
    {
        DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjectSO kitchenObjectSO = GetkitchenObjectSOFromIndex(kitchenObjectSOIndex);

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        if (kitchenObjectParent.HasKitchenObject())
        {
            return;
        }

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        NetworkObject networkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        networkObject.Spawn(true);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent, true);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);

        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
        KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

        kitchenObject.ClearKitchenObjectOnParent();
    }

    public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
    {
        return kitchenObjectList.kitchenObjects.IndexOf(kitchenObjectSO);
    }

    public KitchenObjectSO GetkitchenObjectSOFromIndex(int index)
    {
        return kitchenObjectList.kitchenObjects[index];
    }

}
