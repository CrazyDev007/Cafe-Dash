using UnityEngine;
using System.Collections;
using Game.Domain.Interfaces;

public class CustomerManager : MonoBehaviour
{
    public Transform customerSpawnPoint;
    public GameObject customerPrefab;
    public UIManager uiManager;
    private ICustomerService customerService;
    private GameObject currentCustomer;

    public void Initialize(ICustomerService service)
    {
        customerService = service;
    }

    void Start()
    {
        SpawnCustomer();
    }

    public void SpawnCustomer()
    {
        currentCustomer = Instantiate(customerPrefab, customerSpawnPoint.position, Quaternion.identity);
        currentCustomer.tag = "Customer";
        customerService.OnCustomerSpawned();
    }

    public bool IsWaiting()
    {
        return customerService.IsWaiting;
    }

    public void ReceiveCoffee()
    {
        if (customerService.IsWaiting)
        {
            customerService.OnCustomerServed();
            StartCoroutine(HandleDelivery());
        }
    }

    IEnumerator HandleDelivery()
    {
        uiManager.AnimateMoney(currentCustomer.transform.position);
        Destroy(currentCustomer);
        yield return new WaitForSeconds(1f);
        SpawnCustomer();
    }
}
