using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CuttingCounter : BaseCounter
{

    [SerializeField] private CuttingRecipeSO[] cuttingRecipes;
    [SerializeField] private Animator animator;

    private int cuttingProgress;
    private Image cuttingProgressBar;

    protected override void Awake()
    {
        base.Awake();
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
                    player.GetKitchenObject().SetKitchenObjectParent(this, true);
                    CuttingRecipeSO cuttingRecipe = GetCuttingRecipeFromInput(GetKitchenObject().GetKitchenObjectSO());
                    cuttingProgress = 0;
                    cuttingProgressBar.fillAmount = (float) cuttingProgress / cuttingRecipe.cuttingProgressMax;
                    ShowProgressBar();
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
                if (IsValidKitchenObject(player.GetKitchenObject().GetKitchenObjectSO()))
                {
                    KitchenObject kitchenObject = GetKitchenObject();
                    KitchenObject playerKitchenObject = player.GetKitchenObject();

                    ClearKitchenObject();
                    player.ClearKitchenObject();

                    kitchenObject.SetKitchenObjectParent(player, false);
                    playerKitchenObject.SetKitchenObjectParent(this, false);

                    CuttingRecipeSO cuttingRecipe = GetCuttingRecipeFromInput(GetKitchenObject().GetKitchenObjectSO());
                    cuttingProgress = 0;
                    cuttingProgressBar.fillAmount = (float)cuttingProgress / cuttingRecipe.cuttingProgressMax;
                    ShowProgressBar();
                }
            }
        }
    }

    public override void AltInteract(Player player)
    {
        if (HasKitchenObject() && IsValidKitchenObject(GetKitchenObject().GetKitchenObjectSO()))
        {
            cuttingProgress++;

            CuttingRecipeSO cuttingRecipe = GetCuttingRecipeFromInput(GetKitchenObject().GetKitchenObjectSO());

            cuttingProgressBar.fillAmount = (float)cuttingProgress / cuttingRecipe.cuttingProgressMax;

            animator.SetTrigger("Cut");

            if (cuttingProgress >= cuttingRecipe.cuttingProgressMax)
            {
                HideProgressBar();

                KitchenObjectSO recipeOutput = GetCuttingRecipeOutput(GetKitchenObject().GetKitchenObjectSO());

                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(recipeOutput, this);
            }
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
        cuttingProgressBar.transform.parent.gameObject.SetActive(false);
    }

}
