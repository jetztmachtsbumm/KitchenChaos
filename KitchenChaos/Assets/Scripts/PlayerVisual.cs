using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{

    [SerializeField] private MeshRenderer headRenderer;
    [SerializeField] private MeshRenderer bodyRenderer;

    private Material material;

    private void Awake()
    {
        material = new Material(headRenderer.material);
        headRenderer.material = material;
        bodyRenderer.material = material;
    }

    public void SetPlayerColor(Color color)
    {
        material.color = color;
    }

}
