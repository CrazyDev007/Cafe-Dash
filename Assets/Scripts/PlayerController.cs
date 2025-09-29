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
    private Transform moveTarget;
    private GameObject moveTargetObject;
    private bool isMovingToTarget = false;
    public Transform beanStackParent;
    public GameObject beanBagPrefab;
    public Transform cupHoldPoint;

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
        if (isMovingToTarget && moveTarget != null)
        {
            MoveToTarget();
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
    }
    private void MoveToTarget()
    {
        if (moveTarget == null)
        {
            isMovingToTarget = false;
            return;
        }

        // Move directly towards the target in world space
        Vector3 targetPos = moveTarget.position;
        // Keep player's z the same if working in 2D/top-down
        targetPos.z = transform.position.z;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        float dist = Vector3.Distance(transform.position, targetPos);
        if (dist <= clickMoveStopDistance)
        {
            // Arrived
            isMovingToTarget = false;
            ArriveAtTarget();
            moveTarget = null;
            moveTargetObject = null;
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
                moveTarget = hit.collider.transform;
                moveTargetObject = hit.collider.gameObject;
                isMovingToTarget = true;
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
            if (beanStack.Count > 0 && machine != null)
            {
                machine.InsertBean();
                RemoveBean();
            }
            else if (machine != null && machine.HasCup() && heldCup == null)
            {
                heldCup = machine.TakeCup();
                heldCup.transform.SetParent(cupHoldPoint);
                heldCup.transform.localPosition = Vector3.zero;
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
                if (beanStack.Count > 0 && machine != null)
                {
                    machine.InsertBean();
                    RemoveBean();
                }
                else if (machine != null && machine.HasCup() && heldCup == null)
                {
                    heldCup = machine.TakeCup();
                    heldCup.transform.SetParent(cupHoldPoint);
                    heldCup.transform.localPosition = Vector3.zero;
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
}
