using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent
{

    public static Player Instance { get; private set; }

    public event EventHandler OnPickup;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs
    {
        public BaseCounter SelectedConter { get; private set; }

        public OnSelectedCounterChangedEventArgs(BaseCounter selectedConter)
        {
            SelectedConter = selectedConter;
        }
    }

    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask countersLayerMask;

    private PlayerInputActions playerInputActions;
    private Animator animator;
    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;
    private Transform kitchenObjectHoldPoint;
    private bool isWalking;

    private void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("There are more than one 'Player' objects active in the scene");
            Destroy(gameObject);
        }
        Instance = this;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
        playerInputActions.Player.Interact.performed += Interact_performed;
        playerInputActions.Player.AltInteract.performed += AltInteract_performed;

        animator = GetComponentInChildren<Animator>();
        kitchenObjectHoldPoint = transform.Find("KitchenObjectHoldPoint");
    }

    private void Update()
    {
        HandleMovement();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        Vector3 moveDir = GetMoveDir();

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = moveDir.x != 0 && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove)
                {
                    moveDir = moveDirZ;
                }
            }
        }

        if (canMove)
        {
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        float rotationSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);

        isWalking = moveDir != Vector3.zero;
        animator.SetBool("IsWalking", isWalking);
    }

    private void HandleInteraction()
    {
        Vector3 moveDir = GetMoveDir();

        if(moveDir != Vector3.zero)
        {
            lastInteractDirection = moveDir;
        }

        float interatioDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit hit, interatioDistance, countersLayerMask))
        {
            if(hit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                if(baseCounter != selectedCounter)
                {
                    SetSelectedCounter(baseCounter);
                }
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        selectedCounter?.Interact(this);
    }

    private void AltInteract_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        selectedCounter?.AltInteract(this);
    }

    private Vector3 GetMoveDir()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();
        return new Vector3(inputVector.x, 0, inputVector.y).normalized;
    }

    private void SetSelectedCounter(BaseCounter selectedCounter)
    {
        this.selectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs(selectedCounter));
    }

    public Transform GetKitchenObjectFollowPoint()
    {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if(kitchenObject != null)
        {
            OnPickup?.Invoke(this, EventArgs.Empty);
        }
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }

    public bool IsWalking()
    {
        return isWalking;
    }
}
