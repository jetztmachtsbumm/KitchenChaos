using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private List<RecipeSO> availableRecipes;

    private List<RecipeSO> waitingRecipes;
    private float spawnRecipeTimer = 4f;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;
    private int totalRecipesDelivered;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("There are more than one DeliveryManager objects active in the scene");
            Destroy(gameObject);
        }
        Instance = this;

        waitingRecipes = new List<RecipeSO>();
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (GameManager.Instance.IsGamePlaying() && waitingRecipes.Count < waitingRecipesMax)
            {
                int waitingRecipeIndex = UnityEngine.Random.Range(0, availableRecipes.Count);
                GenerateNewWaitingRecipeClientRpc(waitingRecipeIndex);
            }
        }
    }

    [ClientRpc]
    private void GenerateNewWaitingRecipeClientRpc(int waitingRecipeIndex)
    {
        RecipeSO waitingRecipe = availableRecipes[waitingRecipeIndex];
        waitingRecipes.Add(waitingRecipe);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject, DeliveryCounter deliveryCounter)
    {
        foreach(RecipeSO waitingRecipe in waitingRecipes)
        {
            if(waitingRecipe.kitchenObjects.Count == plateKitchenObject.GetKitchenObjects().Count)
            {
                //Same number of ingredients
                bool plateContentsMatchRecipe = true;
                foreach(KitchenObjectSO recipeKitchenObject in waitingRecipe.kitchenObjects)
                {
                    //Cycling through all ingredients in the recipe
                    bool ingredientFound = false;
                    foreach(KitchenObjectSO kitchenObjectOnPlate in plateKitchenObject.GetKitchenObjects())
                    {
                        //Cycling through all ingredients on the plate
                        if(kitchenObjectOnPlate == recipeKitchenObject)
                        {
                            //Ingredient matches!
                            ingredientFound = true;
                            break;
                        }
                    }

                    if (!ingredientFound)
                    {
                        //Plate contents don't meet the recipe requirements
                        plateContentsMatchRecipe = false;
                    }
                }

                if (plateContentsMatchRecipe)
                {
                    //Player delivered the correct recipe!
                    DeliverCorrectRecipeServerRpc(waitingRecipes.IndexOf(waitingRecipe));
                    return;
                }
            }
        }

        //No matches found!
        //Player did not deliver a correct recipe
        DeliverIncorrectRecipeServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverCorrectRecipeServerRpc(int waitingRecipeIndex)
    {
        DeliverCorrectRecipeClientRpc(waitingRecipeIndex);
    }

    [ClientRpc]
    private void DeliverCorrectRecipeClientRpc(int waitingRecipeIndex)
    {
        totalRecipesDelivered++;
        waitingRecipes.RemoveAt(waitingRecipeIndex);
        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DeliverIncorrectRecipeServerRpc()
    {
        DeliverIncorrectRecipeClientRpc();
    }

    [ClientRpc]
    private void DeliverIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipes()
    {
        return waitingRecipes;
    }

    public int GetTotalRecipesDelivered()
    {
        return totalRecipesDelivered;
    }

}
