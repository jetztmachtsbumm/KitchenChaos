using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManager : MonoBehaviour
{

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private List<RecipeSO> availableRecipes;

    private List<RecipeSO> waitingRecipes;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipesMax = 4;

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
        spawnRecipeTimer -= Time.deltaTime;
        if(spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (waitingRecipes.Count < waitingRecipesMax)
            {
                RecipeSO waitingRecipe = availableRecipes[UnityEngine.Random.Range(0, availableRecipes.Count)];
                waitingRecipes.Add(waitingRecipe);
                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
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
                    waitingRecipes.Remove(waitingRecipe);
                    OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
                    return;
                }
            }
        }

        //No matches found!
        //Player did not deliver a correct recipe
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }

    public List<RecipeSO> GetWaitingRecipes()
    {
        return waitingRecipes;
    }

}
