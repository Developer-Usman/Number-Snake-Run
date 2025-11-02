using System.Collections;
using UnityEngine;

public class DestroyObjectAfterDelay : MonoBehaviour
{
    public float delay = 2f;
    void Start()
    {
        StartCoroutine(DestroyAfterDelay(delay));
    }
    IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
