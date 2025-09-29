using UnityEngine;
using System.Collections;

public class CustomerManager : MonoBehaviour
{
    public Transform customerSpawnPoint;
    public GameObject customerPrefab;
    public UIManager uiManager;
    private GameObject currentCustomer;
    private bool waiting = false;

    void Start()
    {
        SpawnCustomer();
    }

    public void SpawnCustomer()
    {
        currentCustomer = Instantiate(customerPrefab, customerSpawnPoint.position, Quaternion.identity);
        currentCustomer.tag = "Customer";
        waiting = true;
    }

    public bool IsWaiting()
    {
        return waiting;
    }

    public void ReceiveCoffee()
    {
        if (waiting)
        {
            StartCoroutine(HandleDelivery());
        }
    }

    IEnumerator HandleDelivery()
    {
        waiting = false;
        uiManager.AnimateMoney(currentCustomer.transform.position);
        Destroy(currentCustomer);
        yield return new WaitForSeconds(1f);
        SpawnCustomer();
    }
}
