using UnityEngine;

public class CupInventoryService : ICupInventoryService
{
    public bool HasCup => CurrentCup != null;
    public GameObject CurrentCup { get; private set; }
    
    public event System.Action OnCupAttached;
    public event System.Action OnCupRemoved;
    
    private readonly Transform _cupHoldPoint;
    
    public CupInventoryService(Transform cupHoldPoint)
    {
        _cupHoldPoint = cupHoldPoint;
    }
    
    public void AttachCup(GameObject cup)
    {
        if (cup == null)
        {
            Debug.LogWarning("CupInventoryService: cup is null");
            return;
        }

        if (_cupHoldPoint == null)
        {
            Debug.LogError("CupInventoryService: cupHoldPoint is not assigned!");
            return;
        }

        try
        {
            Rigidbody rb = cup.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }

            Vector3 worldPos = cup.transform.position;
            Quaternion worldRot = cup.transform.rotation;

            cup.transform.SetParent(_cupHoldPoint);
            cup.transform.position = worldPos;
            cup.transform.rotation = worldRot;

            cup.transform.position = _cupHoldPoint.position;
            cup.transform.rotation = _cupHoldPoint.rotation;

            cup.transform.localPosition = Vector3.zero;
            cup.transform.localRotation = Quaternion.identity;

            CurrentCup = cup;
            OnCupAttached?.Invoke();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to attach cup: {e.Message}");
        }
    }
    
    public void RemoveCup()
    {
        if (CurrentCup != null)
        {
            Object.Destroy(CurrentCup);
            CurrentCup = null;
            OnCupRemoved?.Invoke();
        }
    }
}