using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconsUI : MonoBehaviour
{

    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start()
    {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e)
    {
        UpdateVisual(e.kitchenObject);
    }

    private void UpdateVisual(KitchenObjectSO updatedKitchenObject)
    {
        foreach(KitchenObjectSO kitchenObject in plateKitchenObject.GetKitchenObjects())
        {
            if(updatedKitchenObject == kitchenObject)
            {
                Transform icon = Instantiate(iconTemplate, transform);
                icon.Find("Icon").GetComponent<Image>().sprite = kitchenObject.sprite;
                icon.gameObject.SetActive(true);
            }
        }
    }
}
