using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StoveCounter : BaseCounter
{

    public event EventHandler OnStateChanged;

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burnt
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipes;
    [SerializeField] private BurningRecipeSO[] burningRecipes;

    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private FryingRecipeSO fryingRecipe;
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private BurningRecipeSO burningRecipe;
    private Transform sizzlingParticles;
    private Transform stoveGlowingVisual;
    private Image progressBar;

    private void Awake()
    {
        sizzlingParticles = transform.Find("StoveCounter_Visual").Find("SizzlingParticles");
        stoveGlowingVisual = transform.Find("StoveCounter_Visual").Find("StoveOnVisual");
        progressBar = transform.Find("ProgressBarUI").Find("Bar").GetComponent<Image>();
        HideProgressBar();
    }

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = fryingRecipe != null ? fryingRecipe.fryingTimerMax : 1f;
        progressBar.fillAmount = fryingTimer.Value / fryingTimerMax;
        EnableVisualEffects();
    }

    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float burningTimerMax = burningRecipe != null ? burningRecipe.burningTimerMax : 1f;
        progressBar.fillAmount = burningTimer.Value / burningTimerMax;
        EnableVisualEffects();
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        if(newValue != State.Frying || newValue != State.Fried)
        {
            DisableVisualEffects();
        }
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (HasKitchenObject())
        {
            switch (state.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;
                    
                    if (fryingTimer.Value > fryingRecipe.fryingTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(fryingRecipe.output, this);

                        state.Value = State.Fried;
                        burningTimer.Value = 0;
                        SetBurningRecipeClientRpc(MultiplayerManager.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO()));
                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;
                    if (burningTimer.Value > burningRecipe.burningTimerMax)
                    {
                        KitchenObject.DestroyKitchenObject(GetKitchenObject());
                        KitchenObject.SpawnKitchenObject(burningRecipe.output, this);
                        HideProgressBar();
                        state.Value = State.Burnt;
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
                    KitchenObject kitchenObject = player.GetKitchenObject();
                    kitchenObject.SetKitchenObjectParent(this, true);

                    InteractLogicServerRpc(MultiplayerManager.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()));
                }
            }
        }
        else
        {
            if (!player.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(player, true);
                DisableVisualEffects();
                SetStateIdleServerRpc();
            }
            else
            {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO()))
                    {
                        GetKitchenObject().DestroySelf();
                        DisableVisualEffects();
                        SetStateIdleServerRpc();
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc()
    {
        state.Value = State.Idle;
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc(int kitchenObjectSOIndex)
    {
        fryingTimer.Value = 0;
        state.Value = State.Frying;
        SetFryingRecipeClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void SetFryingRecipeClientRpc(int kitchenObjectSOIndex)
    {
        fryingRecipe = GetFryingRecipeFromInput(MultiplayerManager.Instance.GetkitchenObjectSOFromIndex(kitchenObjectSOIndex));
    }

    [ClientRpc]
    private void SetBurningRecipeClientRpc(int kitchenObjectSOIndex)
    {
        burningRecipe = GetBurningRecipeFromInput(MultiplayerManager.Instance.GetkitchenObjectSOFromIndex(kitchenObjectSOIndex));
    }

    private bool IsValidKitchenObject(KitchenObjectSO input)
    {
        return GetFryingRecipeFromInput(input) != null;
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

    public float GetBurningProgressNormalized()
    {
        return burningTimer.Value / burningRecipe.burningTimerMax;
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
        progressBar.transform.parent.gameObject.SetActive(true);
    }

    private void HideProgressBar()
    {
        progressBar.transform.parent.gameObject.SetActive(false);
    }

    public State GetState()
    {
        return state.Value;
    }

}
