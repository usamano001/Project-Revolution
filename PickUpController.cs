using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    public ProjectileGun projectileGun;
    public Rigidbody rb;
    public BoxCollider coll;  // Ensure this collider is a child of the GunContainer
    public Transform player, gunContainer, fpsCam;

    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;

    public bool equipped;
    public static bool slotFull;

    private void Start()
    {
        // Set up the initial state of the gun
        if (!equipped)
        {
            projectileGun.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
        }

        if (equipped)
        {
            projectileGun.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            slotFull = true;
        }
    }

    private void Update()
    {
        // Check if player is in range and presses "E" to pick up the weapon
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull)
        {
            PickUp();
        }

        // If equipped and "Q" is pressed, drop the weapon
        if (equipped && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;

        // Make the gun a child of the gun container (which should have the collider)
        transform.SetParent(gunContainer);

        // Reset local position and rotation to ensure it aligns with the container
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Set collider and Rigidbody properties
        rb.isKinematic = true;
        coll.isTrigger = true;

        // Enable gun functionality
        projectileGun.enabled = true;
    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;

        // Detach the gun from the player
        transform.SetParent(null);

        // Make Rigidbody dynamic and enable the collider
        rb.isKinematic = false;
        coll.isTrigger = false;

        // Apply forces to simulate the gun being dropped
        rb.velocity = player.GetComponent<Rigidbody>().velocity;
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

        // Add random rotation to simulate a natural drop
        float randomValue = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(randomValue, randomValue, randomValue) * 10);

        // Disable gun functionality
        projectileGun.enabled = false;
    }
}
