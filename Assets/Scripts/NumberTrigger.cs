using TMPro;
using UnityEngine;

public class NumberTrigger : MonoBehaviour
{
    public TriggerType triggerType;
    private TMP_Text numberText;
    private BoxCollider boxCollider;
    public int number = 1;
    void Awake()
    {
        numberText = GetComponentInChildren<TMP_Text>();
        boxCollider = GetComponent<BoxCollider>();
        boxCollider.isTrigger = true;

        if (number <= 0)
            number = Random.Range(1, 10);
        UpdateNumber(number);

        UpdateTextColor();
        
    }
    public void UpdateNumber(int _number)
    {
        numberText.text = _number.ToString();
        gameObject.name = _number.ToString();
    }
    void UpdateTextColor()
    {
        if (triggerType == TriggerType.Grow)
        {
            numberText.color = Color.blue;
        }
        else
        {
            numberText.color = Color.red;
        }
    }
}
public enum TriggerType
{
    Grow,
    Reduce
}