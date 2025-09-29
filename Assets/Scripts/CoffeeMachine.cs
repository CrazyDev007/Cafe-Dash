using UnityEngine;
using System.Collections;

public class CoffeeMachine : MonoBehaviour
{
    public GameObject cupPrefab;
    public Transform cupSpawnPoint;
    public float processTime = 2f;
    private bool isProcessing = false;
    private GameObject readyCup;

    public void InsertBean(Transform cupHolder = null)
    {
        if (!isProcessing && readyCup == null)
        {
            if (cupPrefab != null)
            {
                StartCoroutine(ProcessCoffee(cupHolder));
            }
            else
            {
                Debug.LogWarning("CoffeeMachine: Missing cupPrefab reference!");
            }
        }
    }

    IEnumerator ProcessCoffee(Transform cupHolder)
    {
        isProcessing = true;
        yield return new WaitForSeconds(processTime);
        
        if (cupPrefab != null)
        {
            // If we have a cup holder, instantiate directly there, otherwise use machine's position
            Vector3 spawnPos = cupHolder != null ? cupHolder.position : transform.position;
            Transform parent = cupHolder != null ? cupHolder : transform;
            
            readyCup = Instantiate(cupPrefab, spawnPos, Quaternion.identity, parent);
            
            if (cupHolder != null)
            {
                // If spawned at cup holder, ensure clean local transform
                readyCup.transform.localPosition = Vector3.zero;
                readyCup.transform.localRotation = Quaternion.identity;
                
                // Disable any physics components
                if (readyCup.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.isKinematic = true;
                }
            }
        }
        isProcessing = false;
    }

    public bool HasCup()
    {
        return readyCup != null;
    }

    public GameObject TakeCup()
    {
        if (readyCup != null)
        {
            GameObject cup = readyCup;
            readyCup = null;
            return cup;
        }
        return null;
    }
}
