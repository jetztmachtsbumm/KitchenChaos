using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoveCounter : BaseCounter
{

    private enum State
    {
        Idle,
        Frying,
        Fried,
        Burnt
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipes;
    [SerializeField] private BurningRecipeSO[] burningRecipes;

    private State state;
    private float fryingTimer;
    private FryingRecipeSO fryingRecipe;
    private float burningTimer;
    private BurningRecipeSO burningRecipe;
    private Transform sizzlingParticles;
    private Transform stoveGlowingVisual;
    private Image fryingProgressBar;

    protected override void Awake()
    {
        base.Awake();
        sizzlingParticles = transform.Find("StoveCounter_Visual").Find("SizzlingParticles");
        stoveGlowingVisual = transform.Find("StoveCounter_Visual").Find("StoveOnVisual");
        fryingProgressBar = transform.Find("ProgressBarUI").Find("Bar").GetComponent<Image>();
        HideProgressBar();
    }

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
            switch (state)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    fryingProgressBar.fillAmount = fryingTimer / fryingRecipe.fryingTimerMax;
                    EnableVisualEffects();
                    if (fryingTimer > fryingRecipe.fryingTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(fryingRecipe.output, this);
                        burningRecipe = GetBurningRecipeFromInput(GetKitchenObject().GetKitchenObjectSO());
                        state = State.Fried;
                        burningTimer = 0;
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;
                    fryingProgressBar.fillAmount = burningTimer / burningRecipe.burningTimerMax;
                    EnableVisualEffects();
                    if (burningTimer > burningRecipe.burningTimerMax)
                    {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningRecipe.output, this);
                        HideProgressBar();
                        state = State.Burnt;
                    }
                    break;
                case State.Burnt:
                    break;
            }
        }
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
                    fryingRecipe = GetFryingRecipeFromInput(GetKitchenObject().GetKitchenObjectSO());
                    state = State.Frying;
                    fryingTimer = 0;
                }
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player, true);
                DisableVisualEffects();
                state = State.Idle;
            }
            else
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        DisableVisualEffects();
                        state = State.Idle;
                    }
                }
            }
        }
    }

    private bool IsValidKitchenObject(KitchenObjectSO input)
    {
        return GetFryingRecipeFromInput(input) != null;
    }

    private KitchenObjectSO GetFryingRecipeOutput(KitchenObjectSO input)
    {
        return GetFryingRecipeFromInput(input).output;
    }

    private FryingRecipeSO GetFryingRecipeFromInput(KitchenObjectSO input)
    {
        foreach (FryingRecipeSO fryingRecipe in fryingRecipes)
        {
            if (fryingRecipe.input == input)
            {
                return fryingRecipe;
            }
        }

        return null;
    }

    private BurningRecipeSO GetBurningRecipeFromInput(KitchenObjectSO input)
    {
        foreach (BurningRecipeSO burningRecipe in burningRecipes)
        {
            if (burningRecipe.input == input)
            {
                return burningRecipe;
            }
        }

        return null;
    }

    private void EnableVisualEffects()
    {
        sizzlingParticles.gameObject.SetActive(true);
        stoveGlowingVisual.gameObject.SetActive(true);
        ShowProgressBar();
    }

    private void DisableVisualEffects()
    {
        sizzlingParticles.gameObject.SetActive(false);
        stoveGlowingVisual.gameObject.SetActive(false);
        HideProgressBar();
    }

    private void ShowProgressBar()
    {
        fryingProgressBar.transform.parent.gameObject.SetActive(true);
    }

    private void HideProgressBar()
    {
        fryingProgressBar.transform.parent.gameObject.SetActive(false);
    }

}