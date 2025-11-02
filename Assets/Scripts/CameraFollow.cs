using UnityEditor.Rendering;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private Player player;
	public Transform target;

	public float smoothSpeed = 0.125f;
	public float boosterSensitivity = 1f;
    public Vector3 offset;
    [SerializeField]private Vector3 initialOffset;

    void Start()
    {
        player = target.GetComponent<Player>();
        initialOffset = offset;
    }
    void FixedUpdate()
    {
        YouTube();
        // Brackey();
    }
    void Brackey()
    {
        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;

        transform.LookAt(target);
    }
    void YouTube()
    {
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, target.position.x, smoothSpeed), transform.position.y, target.position.z + offset.z);

        if (player.isSpeedBoosted)
        {
            offset = new Vector3(offset.x, offset.y, offset.z - boosterSensitivity * Time.fixedDeltaTime);
        }
        else if (player.isSpeedDecreased)
        {
            offset = new Vector3(offset.x, offset.y, offset.z + boosterSensitivity * Time.fixedDeltaTime);
        }
        else
        {
            offset = Vector3.Lerp(offset, initialOffset, boosterSensitivity * Time.fixedDeltaTime);
        }
    }

}