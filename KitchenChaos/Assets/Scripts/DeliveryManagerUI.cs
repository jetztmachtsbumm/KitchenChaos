using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliveryManagerUI : MonoBehaviour
{

    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;

        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, System.EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach(Transform child in container)
        {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (RecipeSO recipe in DeliveryManager.Instance.GetWaitingRecipes())
        {
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);

            recipeTransform.Find("RecipeNameText").GetComponent<TextMeshProUGUI>().text = recipe.recipeName;

            Transform iconContainer = recipeTransform.Find("IconContainer");
            Transform iconTemplate = iconContainer.Find("IngredientImage");
            iconTemplate.gameObject.SetActive(false);

            foreach(KitchenObjectSO kitchenObject in recipe.kitchenObjects)
            {
                Image ingredientImage = Instantiate(iconTemplate, iconContainer).GetComponent<Image>();
                ingredientImage.sprite = kitchenObject.sprite;
                ingredientImage.gameObject.SetActive(true);
            }
        }
    }

}
