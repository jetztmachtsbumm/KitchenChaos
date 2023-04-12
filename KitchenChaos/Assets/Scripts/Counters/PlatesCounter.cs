using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter
{

    [SerializeField] private KitchenObjectSO plateKitchenObjectSO;

    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawned;
    private int maxPlates = 4;
    private List<GameObject> plateVisuals = new List<GameObject>();

    private void Update()
    {
        spawnPlateTimer += Time.deltaTime;
        if(spawnPlateTimer > spawnPlateTimerMax)
        {
            spawnPlateTimer = 0f;

            if(platesSpawned < maxPlates)
            {
                platesSpawned++;
                Transform plateVisual = Instantiate(plateKitchenObjectSO.prefab, GetKitchenObjectFollowPoint());
                float plateOffset = 0.1f;
                plateVisual.localPosition = new Vector3(0, plateOffset * plateVisuals.Count, 0);
                plateVisuals.Add(plateVisual.gameObject);
            }
        }
    }

    public override void Interact(Player player)
    {
        if (!player.HasKitchenObject())
        {
            if(platesSpawned > 0)
            {
                platesSpawned--;

                KitchenObject.SpawnKitchenObject(plateKitchenObjectSO, player);
                GameObject plateVisual = plateVisuals[plateVisuals.Count - 1];
                plateVisuals.Remove(plateVisual);
                Destroy(plateVisual);
            }
        }
    }

}
