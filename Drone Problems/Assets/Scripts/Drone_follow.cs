using UnityEngine;

public class Drone_follow : MonoBehaviour
{
    public Transform target;
    public Transform forwardFocus;

    public float moveSpeed = 4f;
    public float turnSpeed = 720f;
    public float shootingDistance = 8f;

    private Quaternion modelOffset;

    void Start()
    {
        if (target == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                target = playerObject.transform;
            }
        }

        if (forwardFocus != null)
        {
            Vector3 localForward = forwardFocus.localPosition.normalized;

            if (localForward != Vector3.zero)
            {
                modelOffset = Quaternion.Inverse(Quaternion.LookRotation(localForward));
            }
        }
    }

    void Update()
    {
        if (target == null || forwardFocus == null) return;

        Vector3 toTarget = target.position - transform.position;
        float distance = toTarget.magnitude;

        if (distance <= shootingDistance)
        {
            Debug.Log("Shooting at the target!");
            return;
        }

        Quaternion targetRotation =
            Quaternion.LookRotation(toTarget.normalized) * modelOffset;

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            turnSpeed * Time.deltaTime
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            moveSpeed * Time.deltaTime
        );
    }
}