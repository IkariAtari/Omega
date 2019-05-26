using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon : Bolt.EntityBehaviour {

    // General
    public int ID;
    public string Name;
    public GameObject WeaponGFX;
    public int Damage;
    public float minRecoil;
    public float maxRecoil;
    public float minRecoilADS;
    public float maxRecoilADS;
    public float RecoilHor;
    public float RecoilHorADS;
    public float ReloadTime;
    public int AmmunitionCount;
    public int MagazineCapacity;
    public int AmmunitionInMagazine;
    public int ResetTime;
    public string Type;

    // For Shotguns
    public float Spread;
    public int ProjectileAmount;

    // Aiming
    public Vector3 HipPosition;
    public Vector3 SightsPosition;
    public Vector3 CorrectRotation;
    public float ADSspeed;

    public int Caliber;

    // Sway
    public float tiltangle;
    public float tiltAmount;
    public float Smoothing;
    public float OffsetX;
    public float OffsetY;

    public Animator Animator;
    public bool TurnMuzzleflash;
    public bool HasCrosshair;
    public bool HasMuzzleflash;
    public bool EjectsShell;
    public GameObject Projectile;

    public AudioClip ShootSound;
}
