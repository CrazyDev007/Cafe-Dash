using UnityEngine;
using System.Collections;

public class CoffeeMachine : MonoBehaviour
{
    public GameObject cupPrefab;
    public Transform cupSpawnPoint;
    public float processTime = 2f;
    private bool isProcessing = false;
    private GameObject readyCup;

    public void InsertBean()
    {
        if (!isProcessing && readyCup == null)
            StartCoroutine(ProcessCoffee());
    }

    IEnumerator ProcessCoffee()
    {
        isProcessing = true;
        yield return new WaitForSeconds(processTime);
        readyCup = Instantiate(cupPrefab, cupSpawnPoint.position, Quaternion.identity, cupSpawnPoint);
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
