using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawned;
    private int maxPlates = 4;
    private List<GameObject> plateVisuals = new List<GameObject>();

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        spawnPlateTimer += Time.deltaTime;
        if (spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;

            if (GameManager.Instance.IsGamePlaying() && platesSpawned < maxPlates)
            {
                SpawnPlateServerRpc();
            }
        }
    }

    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }

    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        platesSpawned++;
        Transform plateVisual = Instantiate(plateKitchenObjectSO.prefab, GetKitchenObjectFollowPoint());
        float plateOffset = 0.1f;
        plateVisual.localPosition = new Vector3(0, plateOffset * plateVisuals.Count, 0);
        plateVisuals.Add(plateVisual.gameObject);
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if(platesSpawned > 0)
            {
                InteractLogicServerRpc(player.GetNetworkObject());
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        InteractLogicClientRpc(playerNetworkObjectReference);
    }

    [ClientRpc]
    private void InteractLogicClientRpc(NetworkObjectReference playerNetworkObjectReference)
    {
        platesSpawned--;

        playerNetworkObjectReference.TryGet(out NetworkObject networkObject);
        Player player = networkObject.GetComponent<Player>();

        KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
        GameObject plateVisual = plateVisuals[plateVisuals.Count - 1];
        plateVisuals.Remove(plateVisual);
        Destroy(plateVisual);
    }

}
