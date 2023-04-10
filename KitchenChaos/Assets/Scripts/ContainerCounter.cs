using System.Collections;
using System.Collections.Generic;
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

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(player, true);
        animator.SetTrigger("OpenClose");
    }

}
