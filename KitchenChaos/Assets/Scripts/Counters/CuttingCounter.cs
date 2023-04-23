using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CuttingCounter : BaseCounter
{

    public static event EventHandler OnCut;

    new public static void ResetStaticData()
    {
        OnCut = null;
    }

    [SerializeField] private CuttingRecipeSO[] cuttingRecipes;
    [SerializeField] private Animator animator;

    private int cuttingProgress;
    private Image cuttingProgressBar;

    private void Awake()
    {
        cuttingProgressBar = transform.Find("ProgressBarUI").Find("Bar").GetComponent<Image>();
        HideProgressBar();
    }

    public override void Interact(Player player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (IsValidKitchenObject(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this, true);
                    InteractLogicServerRpc();
                }
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
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                    }
                } 
                else if(IsValidKitchenObject(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject kitchenObject = GetKitchenObject();
                    KitchenObject playerKitchenObject = player.GetKitchenObject();

                    ClearKitchenObject();
                    player.ClearKitchenObject();

                    kitchenObject.SetKitchenObjectParent(player, false);
                    playerKitchenObject.SetKitchenObjectParent(this, false);

                    InteractLogicServerRpc();
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        cuttingProgress = 0;
        cuttingProgressBar.fillAmount = 0f;
        ShowProgressBar();
    }

    public override void AltInteract(Player player)
    {
        CutObjectServerRpc();
        TestCuttingProgressDoneServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        CutObjectClientRpc();
    }

    [ClientRpc]
    private void CutObjectClientRpc()
    {
        if (HasKitchenObject() && IsValidKitchenObject(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipeSO cuttingRecipe = GetCuttingRecipeFromInput(GetKitchenObject().GetKitchenObjectSO());

            cuttingProgressBar.fillAmount = (float)cuttingProgress / cuttingRecipe.cuttingProgressMax;

            animator.SetTrigger("Cut");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        CuttingRecipeSO cuttingRecipe = GetCuttingRecipeFromInput(GetKitchenObject().GetKitchenObjectSO());
        if (cuttingProgress >= cuttingRecipe.cuttingProgressMax)
        {
            HideProgressBar();

            KitchenObjectSO recipeOutput = GetCuttingRecipeOutput(GetKitchenObject().GetKitchenObjectSO());

            KitchenObject.DestroyKitchenObject(GetKitchenObject());

            KitchenObject.SpawnKitchenObject(recipeOutput, this);
        }
    }

    private bool IsValidKitchenObject(KitchenObjectSO input)
    {
        return GetCuttingRecipeFromInput(input) != null;
    }

    private KitchenObjectSO GetCuttingRecipeOutput(KitchenObjectSO input)
    {
        return GetCuttingRecipeFromInput(input).output;
    }

    private CuttingRecipeSO GetCuttingRecipeFromInput(KitchenObjectSO input)
    {
        foreach (CuttingRecipeSO cuttingRecipe in cuttingRecipes)
        {
            if (cuttingRecipe.input == input)
            {
                return cuttingRecipe;
            }
        }

        return null;
    }

    private void ShowProgressBar()
    {
        cuttingProgressBar.transform.parent.gameObject.SetActive(true);
    }

    private void HideProgressBar()
    {
        HideProgressBarServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void HideProgressBarServerRpc()
    {
        HideProgressBarClientRpc();
    }

    [ClientRpc]
    private void HideProgressBarClientRpc()
    {
        cuttingProgressBar.transform.parent.gameObject.SetActive(false);
    }

}
