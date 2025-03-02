using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileGun : MonoBehaviour
{
    public string gunName;
    public int ammoCount;
    public float fireRate;

    public void Shoot()
    {
        // Shooting logic
        Debug.Log("Shooting with " + gunName);
    }
}
