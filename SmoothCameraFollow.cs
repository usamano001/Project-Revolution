using UnityEngine;

public class CloseFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target; // The player to follow
    [SerializeField] private Vector3 offset = new Vector3(0, 2, -3); // Closer offset behind the player
    [SerializeField] private float positionSmoothTime = 0.2f; // Smooth time for position
    [SerializeField] private float rotationSmoothTime = 0.05f; // Smooth time for rotation (tighter)

    private Vector3 _currentVelocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogError("Target not set for CloseFollowCamera.");
            return;
        }

        // Calculate the desired position relative to the player's rotation
        Vector3 targetPosition = target.position + target.TransformDirection(offset);

        // Smoothly move the camera to the target position
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, positionSmoothTime);

        // Smoothly rotate the camera to look in the same direction as the player
        Quaternion targetRotation = Quaternion.LookRotation(target.forward, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothTime);
    }
}
