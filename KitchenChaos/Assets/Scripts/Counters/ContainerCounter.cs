using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private Animator animator;

    private void Awake()
    {
        animator = transform.Find("ContainerCounter_Visual").GetComponent<Animator>();
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            return;
        }

        KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
        InteractLogicServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        animator.SetTrigger("OpenClose");
    }

}
