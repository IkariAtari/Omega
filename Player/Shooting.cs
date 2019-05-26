using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : Bolt.EntityEventListener<IPlayer>
{
    private WeaponManager weaponManager;

    private GameObject GameManager;

    [SerializeField]
    private LayerMask Mask;

    [SerializeField]
    private Camera Cam;

    [SerializeField]
    private GameObject weaponGFX;

    public Weapon CurrentWeapon;

    [SerializeField]
    private int TimerConstant;

    public bool CanShoot;

    [SerializeField]
    private bool HasToReload;

    public bool IsReloading;

    private bool isADS;

    public override void Attached () {
        GameManager = GameObject.Find("GameManager");
        weaponManager = GetComponent<WeaponManager>();
        CanShoot = true;
        HasToReload = false;
        isADS = false;
        IsReloading = false;
    }

    private void Update()
    {
        if (!state.IsDead)
        {
            if (entity.IsOwner)
            {
                CurrentWeapon = weaponManager.CurrentWeapon;
                weaponGFX = CurrentWeapon.WeaponGFX;
                Sway();

                if (CanShoot == false)
                {
                    TimerConstant--;

                    if (TimerConstant <= 0)
                    {
                        CanShoot = true;
                        TimerConstant = CurrentWeapon.ResetTime * 2;
                    }
                }

                if (IsReloading == false)
                {
                    switch (CurrentWeapon.Type)
                    {
                        case "single_shot":
                            if (Input.GetMouseButtonDown(0))
                            {
                                if (CanShoot == true)
                                {
                                    Shoot(CurrentWeapon.Type);
                                    CanShoot = false;
                                }
                            }
                            break;
                        case "fully_automatic":
                            if (Input.GetMouseButton(0))
                            {
                                if (CanShoot == true)
                                {
                                    Shoot(CurrentWeapon.Type);
                                    CanShoot = false;
                                }
                            }
                            break;
                        case "shotgun":
                            if (Input.GetMouseButtonDown(0))
                            {
                                if (CanShoot == true)
                                {
                                    Shoot(CurrentWeapon.Type, CurrentWeapon.ProjectileAmount, true);
                                    CanShoot = false;
                                }
                            }
                            break;
                        case "bolt_action":
                            if (Input.GetMouseButtonDown(0))
                            {
                                if (CanShoot == true)
                                {
                                    Shoot(CurrentWeapon.Type, CurrentWeapon.ProjectileAmount, true);
                                    CanShoot = false;
                                }
                            }
                            break;
                        case "projectile":
                            if (Input.GetMouseButtonDown(0))
                            {
                                if (CanShoot == true)
                                {
                                    ShootProjectile(CurrentWeapon.Projectile);
                                    CanShoot = false;
                                }
                            }
                            break;
                    }
                }

                if (CurrentWeapon.HasCrosshair == false)
                {
                    GetComponent<GUImanager>().UnsetCrosshair();
                }

                if (CurrentWeapon.AmmunitionInMagazine <= 0)
                {
                    HasToReload = true;
                }

                if (CurrentWeapon.AmmunitionInMagazine > 0)
                {
                    HasToReload = false;
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    if (CurrentWeapon.AmmunitionInMagazine != CurrentWeapon.MagazineCapacity)
                    {
                        StartCoroutine(Reload());
                    }
                }

                if (Input.GetMouseButton(1))
                {
                    Aim(1);
                }

                if (!Input.GetMouseButton(1))
                {
                    Aim(0);
                }
            }
        }
    }

    private IEnumerator FlashActivate(GameObject Flash)
    {
        Flash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Flash.SetActive(false);
    }
    /*if (H.transform.tag == "Enemy")
      {
            Transform root = H.transform.root;
            Zombie zombie = root.gameObject.GetComponent<Zombie>();
            zombie.DealDamage(CurrentWeapon.Damage, H.transform.name);
      }*/

   
    private void Shoot(string WeaponType, int ProjectileAmount = 1, bool Spread = false)
    {
        if (entity.IsOwner)
        {
            if (HasToReload != true)
            {
                for (int i = 0; i < ProjectileAmount; i++)
                {
                    RaycastHit H;
                    if (Spread)
                    {
                        Physics.Raycast(Cam.ScreenPointToRay(new Vector3(Screen.width / 2 + Random.Range(-CurrentWeapon.Spread, CurrentWeapon.Spread), Screen.height / 2 + Random.Range(-CurrentWeapon.Spread, CurrentWeapon.Spread), 0f)), out H, Mathf.Infinity, Mask);
                    }
                    else
                    {
                        Physics.Raycast(Cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0f)), out H, Mathf.Infinity, Mask);
                    }                    
                    if (H.transform != null)
                    {
                        Debug.Log(H.transform);
                        if (H.transform.tag == "Player")
                        {
                            
                            SendEvent(H.transform);
                        }
                        else
                        {
                            PlaceBulletHoleDecal(H.point, H.normal);
                        }
                    }
                }

                if ((WeaponType == "bolt_action") || (WeaponType == "shotgun"))
                {
                    CurrentWeapon.Animator.SetTrigger("Bolt");
                }

                CurrentWeapon.Animator.SetTrigger("Shoot");

                if (CurrentWeapon.HasMuzzleflash)
                {
                    Transform Flash = weaponGFX.transform.GetChild(0);
                    if (CurrentWeapon.TurnMuzzleflash == true)
                    {
                        Flash.Rotate(new Vector3(Random.Range(0, 360), 0, 0));
                    }
                    StartCoroutine(FlashActivate(Flash.gameObject));
                }
                if (CurrentWeapon.EjectsShell)
                {
                    Transform EjectionPort = weaponGFX.transform.GetChild(1);
                    EjectShell();
                }
                                   
                if (isADS == true)
                {
                    Cam.transform.Rotate(new Vector3(-Random.Range(CurrentWeapon.minRecoilADS, CurrentWeapon.maxRecoilADS), Random.Range(-CurrentWeapon.RecoilHor, CurrentWeapon.RecoilHor), 0));
                }
                else
                {
                    Cam.transform.Rotate(new Vector3(-Random.Range(CurrentWeapon.minRecoil, CurrentWeapon.maxRecoil), Random.Range(-CurrentWeapon.RecoilHor, CurrentWeapon.RecoilHor), 0));
                }

                CurrentWeapon.AmmunitionInMagazine -= 1;

                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in players)
                {
                    var EventSound = ShootSound.Create(player.GetComponent<BoltEntity>());
                    EventSound.WeaponID = weaponManager.CurrentWeaponNumber;
                    EventSound.Shooter = entity;
                    EventSound.Send();
                }

                weaponManager.WeaponAudioSource.clip = weaponManager.CurrentWeapon.ShootSound;
                weaponManager.WeaponAudioSource.Play();
            }
        }  
    }

    private void ShootProjectile(GameObject Projectile)
    {
        if (entity.IsOwner)
        {
            GameObject _instance = new GameObject();
            switch(CurrentWeapon.Name)
            {
                case "Frag":
                    _instance = BoltNetwork.Instantiate(BoltPrefabs.FragThrowed, weaponManager.ThrowStruct.transform.position, weaponManager.ThrowStruct.transform.rotation);
                    break;
                case "FlashBang":
                    _instance = BoltNetwork.Instantiate(BoltPrefabs.FlashBangThrowed, weaponManager.ThrowStruct.transform.position, weaponManager.ThrowStruct.transform.rotation);
                    break;
            }          
            _instance.GetComponent<ThrowableExplosive>().Throwed = entity;
            CurrentWeapon.AmmunitionCount -= 1;
        }
    }
    private void SendEvent(Transform Hit)
    {
        BoltEntity enemyEntity = Hit.transform.GetComponent<BoltEntity>();
        var Event = DamageEvent.Create(enemyEntity);
        Event.Damage = CurrentWeapon.Damage;
        Event.Player = entity;
        Event.Send();
    }

    private IEnumerator Reload()
    { 
        IsReloading = true;

        CurrentWeapon.Animator.SetBool("Reload", true);

        yield return new WaitForSeconds(CurrentWeapon.ReloadTime);

        if (CurrentWeapon.AmmunitionCount > 0)
        {
            if(CurrentWeapon.AmmunitionInMagazine < CurrentWeapon.MagazineCapacity)
            {
                if(CurrentWeapon.AmmunitionCount < CurrentWeapon.MagazineCapacity)
                {
                    CurrentWeapon.AmmunitionCount = 0;
                    CurrentWeapon.AmmunitionInMagazine = CurrentWeapon.AmmunitionCount;
                }
                else
                {
                    CurrentWeapon.AmmunitionCount -= (CurrentWeapon.MagazineCapacity - CurrentWeapon.AmmunitionInMagazine);
                    CurrentWeapon.AmmunitionInMagazine = CurrentWeapon.MagazineCapacity;
                }              
            }
            
            HasToReload = false;
        }

        if(CurrentWeapon.AmmunitionCount < 0)
        {
            CurrentWeapon.AmmunitionCount = 0;
        }

        CurrentWeapon.Animator.SetBool("Reload", false);

        IsReloading = false;

        yield break;
    }

    private void Aim(int State)
    {
        if(State == 1)
        {
            CurrentWeapon.WeaponGFX.transform.localPosition = Vector3.Slerp(CurrentWeapon.WeaponGFX.transform.localPosition, CurrentWeapon.SightsPosition, Time.deltaTime * CurrentWeapon.ADSspeed);
            GetComponent<GUImanager>().UnsetCrosshair();
            isADS = true;
        }
        else
        {
            CurrentWeapon.WeaponGFX.transform.localPosition = Vector3.Slerp(CurrentWeapon.WeaponGFX.transform.localPosition, CurrentWeapon.HipPosition, Time.deltaTime * CurrentWeapon.ADSspeed);

            if (CurrentWeapon.HasCrosshair == true)
            {
                GetComponent<GUImanager>().SetCrosshair();
            }

            isADS = false;
        }
    }

    private void PlaceBulletHoleDecal(Vector3 Point, Vector3 Hit)
    {
        Debug.Log("Cheeaase!");
        BoltEntity Decal = BoltNetwork.Instantiate(BoltPrefabs.Bullethole1, Point, Quaternion.FromToRotation(Vector3.up, Hit) * Quaternion.Euler(Vector3.up * Random.Range(0, 360))) as BoltEntity;
        Decal.transform.Translate(Vector3.up * 0.01f, Space.Self);
    }

    private void Sway()
    {
        float TiltX = Input.GetAxis("Mouse X") * CurrentWeapon.tiltangle + CurrentWeapon.OffsetX;
        float TiltY = Input.GetAxis("Mouse Y") * CurrentWeapon.tiltangle + CurrentWeapon.OffsetY;

        Quaternion target = Quaternion.Euler(0, TiltX, TiltY);
        weaponManager.WeaponStruct.transform.localRotation = Quaternion.Slerp(weaponManager.WeaponStruct.transform.localRotation, target, Time.deltaTime * CurrentWeapon.Smoothing);
    }
    
    private void EjectShell()
    {
        GameObject EjectedShell = new GameObject();
        switch(CurrentWeapon.Caliber)
        {
            case 0:
                EjectedShell = BoltNetwork.Instantiate(BoltPrefabs._9mm, CurrentWeapon.transform.GetChild(1).transform.position, CurrentWeapon.transform.GetChild(1).transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0)));
                break;
            case 1:
                EjectedShell = BoltNetwork.Instantiate(BoltPrefabs._5_56, CurrentWeapon.transform.GetChild(1).transform.position, CurrentWeapon.transform.GetChild(1).transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0)));
                break;
            case 2:
                EjectedShell = BoltNetwork.Instantiate(BoltPrefabs._12_Gauge, CurrentWeapon.transform.GetChild(1).transform.position, CurrentWeapon.transform.GetChild(1).transform.rotation * Quaternion.Euler(new Vector3(90, 0, 0)));
                break;
        }
       
        Rigidbody EjectedShellRB = EjectedShell.GetComponent<Rigidbody>();
        EjectedShellRB.AddRelativeForce(new Vector3(300, 250, 0));
        EjectedShellRB.AddRelativeTorque(new Vector3(Random.Range(10000, 50000), 0, Random.Range(10000, 50000)));
    }

    public override void OnEvent(ShootSound evnt)
    {
        if (evnt.Shooter != entity)
        {
            Debug.Log("Shooter :"+evnt.Shooter);         
            evnt.Shooter.GetComponent<WeaponManager>().WeaponAudioSource.clip = weaponManager.Weapons[evnt.WeaponID].GetComponent<Weapon>().ShootSound;
            evnt.Shooter.GetComponent<WeaponManager>().WeaponAudioSource.Play();
        }
    }
}