using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour, IKitchenObjectParent
{

    public static Player LocalInstance { get; private set; }

    public static event EventHandler OnAnyPlayerSpawned;
    public static event EventHandler OnAnyPlayerPickup;

    public static void ResetStaticData()
    {
        OnAnyPlayerSpawned = null;
    }

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
    public event EventHandler OnPauseAction;
    public event EventHandler OnInteractAction;
    public event EventHandler OnKeyRebind;

    public enum Binding
    {
        MOVE_UP,
        MOVE_DOWN,
        MOVE_RIGHT,
        MOVE_LEFT,
        INTERACT,
        ALT_INTERACT,
        PAUSE_GAME
    }

    [SerializeField] private float moveSpeed;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private LayerMask collisionsLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;
    [SerializeField] private List<Vector3> spawnPositions;
    [SerializeField] private PlayerVisual playerVisual;

    private PlayerInputActions playerInputActions;
    private Animator animator;
    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;
    private bool isWalking;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalInstance = this;

            playerInputActions = new PlayerInputActions();

            if (PlayerPrefs.HasKey("KeyBindings"))
            {
                playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString("KeyBindings"));
            }

            playerInputActions.Player.Enable();
            playerInputActions.Player.Interact.performed += Interact_performed;
            playerInputActions.Player.AltInteract.performed += AltInteract_performed;
            playerInputActions.Player.Pause.performed += Pause_performed;

            animator = GetComponentInChildren<Animator>();

            transform.position = spawnPositions[MultiplayerManager.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

            OnPauseAction += GameManager.Instance.Player_OnPauseAction;
            OnInteractAction += GameManager.Instance.Player_OnInteractAction;
            OnKeyRebind += TutorialUI.Instance.Player_OnKeyRebind;
            TutorialUI.Instance.UpdateVisual();
            TutorialUI.Instance.Show();
        }

        PlayerData playerData = MultiplayerManager.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerVisual.SetPlayerColor(MultiplayerManager.Instance.GetPlayerColor(playerData.colorId));

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

        if (IsServer)
        {
            NetworkManager.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        if(clientId == OwnerClientId && HasKitchenObject())
        {
            KitchenObject.DestroyKitchenObject(GetKitchenObject());
        }
    }

    new private void OnDestroy()
    {
        if (IsOwner)
        {
            playerInputActions.Player.Interact.performed -= Interact_performed;
            playerInputActions.Player.AltInteract.performed -= AltInteract_performed;
            playerInputActions.Player.Pause.performed -= Pause_performed;
            playerInputActions.Dispose();
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        HandleMovement();
        HandleInteraction();
    }

    private void HandleMovement()
    {
        Vector3 moveDir = GetMoveDir();

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDir, Quaternion.identity, moveDistance, collisionsLayerMask);

        if (!canMove)
        {
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -0.5f || moveDir.x > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirX, Quaternion.identity, moveDistance, collisionsLayerMask);

            if (canMove)
            {
                moveDir = moveDirX;
            }
            else
            {
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -0.5f || moveDir.z > 0.5f) && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirZ, Quaternion.identity, moveDistance, collisionsLayerMask);

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

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            selectedCounter?.Interact(this);
        }
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void AltInteract_performed(InputAction.CallbackContext obj)
    {
        if (GameManager.Instance.IsGamePlaying())
        {
            selectedCounter?.AltInteract(this);
        }
    }

    private void Pause_performed(InputAction.CallbackContext obj)
    {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    public string GetBindingText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.MOVE_UP:
                return playerInputActions.Player.Move.bindings[1].ToDisplayString();
            case Binding.MOVE_DOWN:
                return playerInputActions.Player.Move.bindings[2].ToDisplayString();
            case Binding.MOVE_RIGHT:
                return playerInputActions.Player.Move.bindings[3].ToDisplayString();
            case Binding.MOVE_LEFT:
                return playerInputActions.Player.Move.bindings[4].ToDisplayString();
            case Binding.INTERACT:
                return playerInputActions.Player.Interact.bindings[0].ToDisplayString();
            case Binding.ALT_INTERACT:
                return playerInputActions.Player.AltInteract.bindings[0].ToDisplayString();
            case Binding.PAUSE_GAME:
                return playerInputActions.Player.Pause.bindings[0].ToDisplayString();
        }
    }

    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        playerInputActions.Player.Disable();

        InputAction inputAction = null;
        int bindingIndex = 0;

        switch (binding)
        {
            case Binding.MOVE_UP:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 1;
                break;
            case Binding.MOVE_DOWN:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 2;
                break;
            case Binding.MOVE_RIGHT:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 3;
                break;
            case Binding.MOVE_LEFT:
                inputAction = playerInputActions.Player.Move;
                bindingIndex = 4;
                break;
            case Binding.INTERACT:
                inputAction = playerInputActions.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.ALT_INTERACT:
                inputAction = playerInputActions.Player.AltInteract;
                bindingIndex = 0;
                break;
            case Binding.PAUSE_GAME:
                inputAction = playerInputActions.Player.Pause;
                bindingIndex = 0;
                break;
        }

        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback => {
            callback.Dispose();
            playerInputActions.Enable();
            onActionRebound();
            PlayerPrefs.SetString("KeyBindings", playerInputActions.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
            OnKeyRebind?.Invoke(this, EventArgs.Empty);
        }).Start();
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
            OnAnyPlayerPickup?.Invoke(this, EventArgs.Empty);
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

    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
