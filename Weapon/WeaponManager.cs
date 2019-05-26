using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Bolt.EntityBehaviour<IPlayer>
{
    public GameObject[] Weapons;
    public Weapon CurrentWeapon;

    public GameObject WeaponStruct;
    public GameObject ThrowStruct;

    [SerializeField]
    private int WeaponsAvailable;

    public AudioSource WeaponAudioSource;
    public int CurrentWeaponNumber;

    private Shooting shooting;

    private LoadoutManager loadoutManager;

    public override void Attached()
    {
        loadoutManager = GameObject.Find("PlayerManager").GetComponent<LoadoutManager>();
        InstantiateWeapons();
        shooting = GetComponent<Shooting>();
    }

    public void ReinstantiateWeapons()
    {
        if (entity.IsOwner)
        {         
                for (int i = 0; i < Weapons.GetLength(0); i++)
                {
                    Destroy(Weapons[i].gameObject);
                }         
                InstantiateWeapons();
        }         
    }

    public void InstantiateWeapons()
    {
        if (entity.IsOwner)
        {
            WeaponsAvailable = 4;
           
            CurrentWeaponNumber = 0;

            GameObject[] WeaponsToInstantiate = new GameObject[4];
            WeaponsToInstantiate[0] = loadoutManager.Loadouts[loadoutManager.LoadoutChosen].Primary;
            WeaponsToInstantiate[1] = loadoutManager.Loadouts[loadoutManager.LoadoutChosen].Secondary;
            WeaponsToInstantiate[2] = loadoutManager.Loadouts[loadoutManager.LoadoutChosen].Lethal;
            WeaponsToInstantiate[3] = loadoutManager.Loadouts[loadoutManager.LoadoutChosen].NonLethal;

            for (int i = 0; i < WeaponsToInstantiate.GetLength(0); i++)
            {
                Weapons[i] = Instantiate(WeaponsToInstantiate[i], WeaponStruct.transform.position, Quaternion.identity);
                Weapons[i].transform.SetParent(WeaponStruct.transform);
                Weapons[i].transform.rotation = Quaternion.Euler(WeaponsToInstantiate[i].GetComponent<Weapon>().CorrectRotation);
                Weapons[i].transform.localRotation = Quaternion.Euler(WeaponsToInstantiate[i].GetComponent<Weapon>().CorrectRotation);
                Weapons[i].layer = LayerMask.NameToLayer("Weapon");
                for (int j = 0; j < Weapons[i].transform.childCount; j++)
                {
                    Transform Child = Weapons[i].transform.GetChild(j);
                    Child.gameObject.layer = LayerMask.NameToLayer("Weapon");
                    for (int k = 0; k < Child.childCount; k++)
                    {
                        Child.GetChild(k).gameObject.layer = LayerMask.NameToLayer("Weapon");
                    }
                }
                Weapons[i].transform.localPosition = Weapons[i].GetComponent<Weapon>().HipPosition;
                Weapons[i].SetActive(false);
            }

            Weapons[0].SetActive(true);
            CurrentWeapon = Weapons[0].GetComponent<Weapon>();
        }
    }

    public void SwitchLoadout(int _loadout)
    {
        Debug.Log(_loadout);
        loadoutManager.SetLoadout(_loadout);
    }

    private void Update()
    {
        if (entity.IsOwner)
        {
            if (shooting.IsReloading == false && shooting.CanShoot == true)
            {
                if (Input.GetKeyDown(KeyCode.G))
                {
                    SwitchWeapon();
                }
            }
        }

        if(CurrentWeapon.Type == "projectile")
        {
            if(CurrentWeapon.AmmunitionCount <= 0)
            {
                CurrentWeapon.gameObject.SetActive(false);
                SwitchWeapon();
            }
        }
    }

    private void SwitchWeapon()
    {
        if (entity.IsOwner)
        {
            if (CurrentWeaponNumber < (WeaponsAvailable - 1))
            {
                Weapons[CurrentWeaponNumber].SetActive(false);
                Weapons[CurrentWeaponNumber + 1].SetActive(true);
                CurrentWeapon = Weapons[CurrentWeaponNumber + 1].GetComponent<Weapon>();
                CurrentWeaponNumber += 1;
            }
            else
            {
                Weapons[CurrentWeaponNumber].SetActive(false);
                Weapons[0].SetActive(true);
                CurrentWeapon = Weapons[0].GetComponent<Weapon>();
                CurrentWeaponNumber = 0;
            }
        }
    }   
}
