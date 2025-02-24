using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EquipmentController : MonoBehaviour
{
    public enum EquipmentType
    {
       Weapon,
       Tool,
    }

    [Header("Information")]
    [Tooltip("The name that will be displayed in the UI for this Equipment")]
    public string EquipmentName;

    [Tooltip("The image that will be displayed in the UI for this Equipment")]
    public Sprite EquipmentIcon;

    [Tooltip("The root object for the weapon, this is what will be deactivated when the weapon isn't active")]
    public GameObject EquipmentRoot;

    [Tooltip("Tip of the Equipment, where the projectiles are shot (If a weapon")]
    public Transform WeaponMuzzle;

    [Tooltip("The type of Equipment wil affect how it is utilized")]
    public EquipmentType EquipType;

    [Tooltip("Maximum amount of ammo in the gun")]
    public int MaxAmmo = 8;
    float m_CurrentAmmo;

    [Tooltip("The projectile prefab")] public GameObject ProjectilePrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
