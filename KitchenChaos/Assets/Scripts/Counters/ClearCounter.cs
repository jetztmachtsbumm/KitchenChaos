using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{


    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                player.GetKitchenObject().SetKitchenObjectParent(this, true);
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player, true);
            }
            else
            {
                KitchenObject kitchenObject = GetKitchenObject();
                KitchenObject playerKitchenObject = player.GetKitchenObject();

                ClearKitchenObject();
                player.ClearKitchenObject();

                kitchenObject.SetKitchenObjectParent(player, false);
                playerKitchenObject.SetKitchenObjectParent(this, false);
            }
        }
    }

}
