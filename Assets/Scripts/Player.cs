using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    private Rigidbody rb;
    private Vector2 minMaxX = new Vector2(-2.3f, 2.3f);

    private List<GameObject> numberParts = new List<GameObject>();
    private List<Vector3> positionHistory = new List<Vector3>();
    [SerializeField] private Joystick joystick;
    [SerializeField] private GameObject numberPrefab;
    [SerializeField] private float forwardSpeed = 5f;
    [SerializeField] private float horizontalSpeed = 5f;
    [SerializeField] private int gap = 10;
    [SerializeField] private float maxYRotation = 30f;
    [SerializeField] private float rotationSpeed = 8f;

    [Space(10)]
    [Header("Particles")]
    [SerializeField] private GameObject GrowtriggerParticle;
    [SerializeField] private GameObject reduceTriggerParticle;
    [SerializeField] private GameObject boostUp;
    [SerializeField] private GameObject boostDown;
    [SerializeField] private GameObject explosionParticle;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        GrowSnake(1);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) // reduce testing
        {
            ReduceSnake(5);
        }
    }
    void FixedUpdate()
    {
        // limiting player movement
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minMaxX.x, minMaxX.y),
            transform.position.y,
            transform.position.z
        );
        // control movement
        rb.linearVelocity = new Vector3(joystick.Horizontal * horizontalSpeed, rb.linearVelocity.y, forwardSpeed) * Time.fixedDeltaTime;

        // rotate player around Y based on horizontal input
        float targetY = joystick.Horizontal * maxYRotation;
        float currentY = Mathf.LerpAngle(transform.eulerAngles.y, targetY, rotationSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Euler(0f, currentY, 0f);

        // store position historoy
        positionHistory.Insert(0, transform.position);

        // move number in childs
        int index = 0;
        foreach (var number in numberParts)
        {
            int minimum;
            minimum = Mathf.Min(index * gap, positionHistory.Count - 1);
            Vector3 point = positionHistory[minimum];
            Vector3 moveDirection = point - number.transform.position;
            number.transform.position += moveDirection * 5f * Time.fixedDeltaTime;
            Vector3 lookPoint = new Vector3(point.x, number.transform.position.y, point.z);
            number.transform.LookAt(lookPoint);
            index++;
        }
    }
    private void GrowSnake(int number = 1) // increase number snake length
    {

        for (int i = 0; i < number; i++)
        {
            GameObject spawnedNumber = Instantiate(numberPrefab);
            UpdateSpawnedNumbers(spawnedNumber);
        }
        UpdateNumberSequentially();
    }
    void UpdateSpawnedNumbers(GameObject _number) // 
    {
        // _number.transform.SetParent(this.transform);
        numberParts.Add(_number);

        // _number.GetComponent<BoxCollider>().enabled = false;
        // _number.layer = LayerMask.NameToLayer("Default");
    }
    void UpdateNumberSequentially() // update numbers in sequentially
    {
        if (numberParts.Count == 0)
        {
            return;
        }
        numberParts[0].transform.localScale = Vector3.one * 2f; // head scale
        for (int i = 0; i < numberParts.Count; i++)
        {
            numberParts[i].GetComponent<SpawnedNumber>().UpdateNumber(numberParts.Count - i);
        }
    }
    public void ReduceSnake(int number = 1) // decrease number snake length
    {
        for (int i = 0; i < number; i++)
        {
            if (numberParts.Count == 0)
            {
                UIManager.Instance.GameOver();
                return;
            }
            GameObject lastNumber = numberParts[numberParts.Count - 1];
            numberParts.RemoveAt(numberParts.Count - 1);
            Destroy(lastNumber);
        }
        UpdateNumberSequentially();
    }

    #region Triggers

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            if (numberParts.Count <= 0) return;
            numberParts[0].transform.DOScale(2.5f, 0.4f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                numberParts[0].transform.DOScale(2f, 0.2f);
            });
            NumberTrigger numberScript = other.GetComponent<NumberTrigger>();
            if (numberScript.triggerType == TriggerType.Grow)
            {
                GrowSnake(numberScript.number);
                TriggerParticles(GrowtriggerParticle, other.transform.position);
                AudioManager.Instance.PlayNumberClipPositive();
            }
            else
            {
                ReduceSnake(numberScript.number);
                TriggerParticles(reduceTriggerParticle, other.transform.position);
                AudioManager.Instance.PlayNumberClipNegative();
            }

            Destroy(other.gameObject);
        }
        if (other.gameObject.layer == 7)
        {
            //Speed Booster
            ActivateSpeedBoost();
            TriggerParticles(boostUp, other.transform.position + new Vector3(0, 1.5f, 0));
            AudioManager.Instance.PlayPowerUp();
            Destroy(other.gameObject);
        }
        if (other.gameObject.layer == 8)
        {
            //Speed Reducer
            ActivateSpeedDecrease();
            AudioManager.Instance.PlayPowerDown();
            TriggerParticles(boostDown, other.transform.position + new Vector3(0, 1.5f, 0));
            Destroy(other.gameObject);
        }
        if (other.gameObject.layer == 10)
        {
            UIManager.Instance.GameComplete();
        }
    }
    void TriggerParticles(GameObject _particle, Vector3 position)
    {
        Instantiate(_particle, position, Quaternion.identity);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 9) // obstacle layer
        {
            TriggerParticles(explosionParticle, collision.transform.position);
            AudioManager.Instance.PlayExplosion();
            UIManager.Instance.GameOver();
        }
    }
    #endregion


    #region Booster

    [SerializeField] private float speedBoostMultiplier = 2f; // how much faster during boost
    [SerializeField] private float speedDecreaseMultiplier = 0.5f; // how much slower during decrease
    private float originalForwardSpeed;
    public bool isSpeedBoosted = false;
    public bool isSpeedDecreased = false;

    // Speed Boost Method
    private void ActivateSpeedBoost()
    {
        if (!isSpeedBoosted && !isSpeedDecreased)
        {
            isSpeedBoosted = true;
            originalForwardSpeed = forwardSpeed;
            forwardSpeed *= speedBoostMultiplier;
            StartCoroutine(ResetSpeedAfterDelay(3f, true));
        }
    }
    // Speed Decrease Method
    private void ActivateSpeedDecrease()
    {
        if (!isSpeedDecreased && !isSpeedBoosted)
        {
            isSpeedDecreased = true;
            originalForwardSpeed = forwardSpeed;
            forwardSpeed *= speedDecreaseMultiplier;
            StartCoroutine(ResetSpeedAfterDelay(3f, false));
        }
    }
    private IEnumerator ResetSpeedAfterDelay(float delay, bool wasSpeedBoost)
    {
        yield return new WaitForSeconds(delay);
        forwardSpeed = originalForwardSpeed;

        if (wasSpeedBoost)
            isSpeedBoosted = false;
        else
            isSpeedDecreased = false;
    }

    #endregion
}
