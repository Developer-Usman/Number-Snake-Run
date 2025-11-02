using TMPro;
using UnityEngine;

public class SpawnedNumber : MonoBehaviour
{
    private TMP_Text numberText;
    void Awake()
    {
        numberText = GetComponentInChildren<TMP_Text>();
    }
    public void UpdateNumber(int _number)
    {
        numberText.text = _number.ToString();
        gameObject.name = _number.ToString();
    }
}
