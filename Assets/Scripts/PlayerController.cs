using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int maxBeans = 3;
    [SerializeField] private List<GameObject> beanStack = new List<GameObject>();
    [HideInInspector] private GameObject heldCup;
    [SerializeField] private InputActionAsset inputActions;
    [SerializeField] private string moveActionName = "Move";
    private InputAction moveAction;
    [SerializeField] private float clickMoveStopDistance = 0.5f;
    private Vector3 moveTargetPosition;
    private GameObject moveTargetObject;
    private bool isMovingToTarget = false;
    [SerializeField] private bool debugClickMove = true;
    [SerializeField] private bool rotateOnMove = true;
    [SerializeField] private float rotationSpeed = 720f; // degrees per second
    public Transform beanStackParent;
    public GameObject beanBagPrefab;
    public Transform cupHoldPoint;
    private bool awaitingBrewCup = false;

    private void OnEnable()
    {
        if (inputActions != null)
        {
            moveAction = inputActions.FindAction(moveActionName);
            moveAction?.Enable();
        }
    }

    private void OnDisable()
    {
        moveAction?.Disable();
    }

    private void Update()
    {
        // If player is moving to a clicked target, drive movement there. Otherwise allow manual input movement.
        if (isMovingToTarget)
        {
            MoveToTarget();
        }
        else
        {
            Move();
        }
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryInteract();
        }
        // Handle mouse click -> move to bean resource or coffee machine (using new Input System)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleClickToMove();
        }
        // If we are expecting a brewed cup to appear in the player's cup holder, capture it as heldCup when it spawns
        if (awaitingBrewCup && heldCup == null && cupHoldPoint != null && cupHoldPoint.childCount > 0)
        {
            heldCup = cupHoldPoint.GetChild(0).gameObject;
            awaitingBrewCup = false;
            if (debugClickMove)
            {
                Debug.Log("[PlayerController] Brew complete: cup attached to player's cupHoldPoint and registered as heldCup");
            }
        }
    }
    
    private void Move()
    {
        if (moveAction == null) return;
        Vector2 input = moveAction.ReadValue<Vector2>();
        // For a 3D game we map the 2D input to the XZ plane: input.x -> X, input.y -> Z
        Vector3 dir = new Vector3(input.x, 0f, input.y);
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
        if (rotateOnMove && dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
        // Player rotation is now prevented.
    }
    private void MoveToTarget()
    {
        // Move directly towards the target in world space (we use the hit point stored in moveTargetPosition)
        Vector3 targetPos = moveTargetPosition;
        // Keep player's Y the same so movement happens on the XZ plane for a 3D game
        targetPos.y = transform.position.y;
        if (debugClickMove)
        {
            Debug.DrawLine(transform.position, targetPos, Color.green);
        }
        // Move towards the target position on XZ plane
        Vector3 newPos = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        Vector3 moveDelta = newPos - transform.position;
        transform.position = newPos;
        if (rotateOnMove && moveDelta.sqrMagnitude > 0.0001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDelta.normalized, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist <= clickMoveStopDistance)
        {
            // Arrived
            isMovingToTarget = false;
            ArriveAtTarget();
            moveTargetPosition = Vector3.zero;
            moveTargetObject = null;
            if (debugClickMove)
            {
                Debug.Log($"[PlayerController] Arrived at targetPos {targetPos}");
            }
        }
    }

    private void HandleClickToMove()
    {
    Camera cam = Camera.main;
    if (cam == null) return;
    Vector2 mousePos = Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
    Ray ray = cam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f))
        {
            if (hit.collider.CompareTag("BeanResource") || hit.collider.CompareTag("CoffeeMachine"))
            {
                // Use the exact hit point so the player moves to the clicked location on the object
                moveTargetPosition = hit.point;
                moveTargetObject = hit.collider.gameObject;
                isMovingToTarget = true;
                if (debugClickMove)
                {
                    Debug.Log($"[PlayerController] Clicked {moveTargetObject.name} at hit.point={hit.point}, collider.bounds.center={hit.collider.bounds.center}, collider.transform.position={hit.collider.transform.position}");
                    Debug.DrawLine(cam.transform.position, hit.point, Color.cyan, 2f);
                }
            }
        }
    }

    private void ArriveAtTarget()
    {
        if (moveTargetObject == null) return;

        if (moveTargetObject.CompareTag("BeanResource"))
        {
            PickupBean(moveTargetObject);
        }
        else if (moveTargetObject.CompareTag("CoffeeMachine"))
        {
            CoffeeMachine machine = moveTargetObject.GetComponent<CoffeeMachine>();
            if (machine == null) return;
            
            // First priority: insert bean if player has beans and machine is ready
            if (beanStack.Count > 0 && !machine.HasCup())
            {
                // Brew directly to the player's hand by spawning the cup under cupHoldPoint
                machine.InsertBean(cupHoldPoint);
                RemoveBean();
                awaitingBrewCup = true;
            }
            // Second priority: take cup if machine has one and player doesn't
            else if (machine.HasCup() && heldCup == null)
            {
                GameObject cupFromMachine = machine.TakeCup();
                if (cupFromMachine != null)
                {
                    AttachCupToPlayer(cupFromMachine);
                }
            }
        }
    }

    private void TryInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out hit, 2f))
        {
            if (hit.collider.CompareTag("BeanResource"))
            {
                PickupBean(hit.collider.gameObject);
            }
            else if (hit.collider.CompareTag("CoffeeMachine"))
            {
                CoffeeMachine machine = hit.collider.GetComponent<CoffeeMachine>();
                if (machine == null) return;
                
                // First priority: insert bean if player has beans and machine is ready
                if (beanStack.Count > 0 && !machine.HasCup())
                {
                    // Brew directly to the player's hand by spawning the cup under cupHoldPoint
                    machine.InsertBean(cupHoldPoint);
                    RemoveBean();
                    awaitingBrewCup = true;
                }
                // Second priority: take cup if machine has one and player doesn't
                else if (machine.HasCup() && heldCup == null)
                {
                    GameObject cupFromMachine = machine.TakeCup();
                    if (cupFromMachine != null)
                    {
                        AttachCupToPlayer(cupFromMachine);
                    }
                }
            }
            else if (hit.collider.CompareTag("Customer"))
            {
                CustomerManager customer = hit.collider.GetComponent<CustomerManager>();
                if (heldCup != null && customer != null && customer.IsWaiting())
                {
                    customer.ReceiveCoffee();
                    Destroy(heldCup);
                    heldCup = null;
                }
            }
        }
    }

    private void PickupBean(GameObject beanResource)
    {
        if (beanStack.Count < maxBeans)
        {
            GameObject bean = Instantiate(beanBagPrefab, beanStackParent);
            bean.transform.localPosition = new Vector3(0, 0.3f * beanStack.Count, 0);
            beanStack.Add(bean);
        }
    }

    private void RemoveBean()
    {
        if (beanStack.Count > 0)
        {
            Destroy(beanStack[beanStack.Count - 1]);
            beanStack.RemoveAt(beanStack.Count - 1);
        }
    }

    // Attach a cup GameObject to the player's cup hold point reliably
    private void AttachCupToPlayer(GameObject cup)
    {
        if (cup == null)
        {
            Debug.LogWarning("AttachCupToPlayer: cup is null");
            return;
        }

        if (cupHoldPoint == null)
        {
            Debug.LogError("AttachCupToPlayer: cupHoldPoint is not assigned on PlayerController!");
            return;
        }

        try
        {
            // First, disable any physics components that might interfere
            Rigidbody rb = cup.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            // Store the world position/rotation before changing parent
            Vector3 worldPos = cup.transform.position;
            Quaternion worldRot = cup.transform.rotation;

            // Set the parent directly to cupHoldPoint
            cup.transform.SetParent(cupHoldPoint);

            // Restore world position/rotation temporarily
            cup.transform.position = worldPos;
            cup.transform.rotation = worldRot;

            // Now smoothly move to the hold point
            cup.transform.position = cupHoldPoint.position;
            cup.transform.rotation = cupHoldPoint.rotation;

            // Finally, zero out local transform
            cup.transform.localPosition = Vector3.zero;
            cup.transform.localRotation = Quaternion.identity;

            // Store reference
            heldCup = cup;

            if (debugClickMove)
            {
                Debug.Log($"Cup successfully attached to player at position {cup.transform.position}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to attach cup to player: {e.Message}");
        }
    }
}
