using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : BaseCounter
{

    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = transform.Find("ContainerCounter_Visual").GetComponent<Animator>();
    }

    public override void Interact(Player player)
    {
        if (player.HasKitchenObject())
        {
            return;
        }

        KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
        animator.SetTrigger("OpenClose");
    }

}
