using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Bolt.EntityEventListener<IPlayer>
{
    Collider triggerColliding;
    private bool canUse = false;

    public int Points;

    [SerializeField]
    Behaviour[] DisabledComponents;

    [SerializeField]
    GameObject[] DisabledObjects;

    GameObject PlayerManager;

    [SerializeField]
    private BoltEntity LastDamaged;

    private GUImanager GuiManager;

    private const string REMOTE_PLAYER_LAYER = "remotePlayer";

    public override void Attached()
    {
        PlayerManager = GameObject.Find("PlayerManager");
        GuiManager = GetComponent<GUImanager>();
       
        state.Health = 100;

        if (!entity.IsOwner)
        {
            DisableRemoteBehaviour();
                      
            gameObject.layer = LayerMask.NameToLayer(REMOTE_PLAYER_LAYER);

            transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer(REMOTE_PLAYER_LAYER);

            for(int i = 0; i < 2; i++)
            {
                transform.GetChild(0).GetChild(i).gameObject.layer = LayerMask.NameToLayer(REMOTE_PLAYER_LAYER);
            }
        }
        else
        {
            state.Username = PlayerManager.GetComponent<PlayerAuth>().Name;
            state.XP = PlayerManager.GetComponent<PlayerAuth>().XP;
            state.Kills = 0;
            state.Deaths = 0;
            state.Score = 0;

            var Event = PlayerRegistered.Create();
            Event.Name = PlayerManager.GetComponent<PlayerAuth>().Name;
            Event.Entity = entity;
            Event.Send();
        }
    }

    private void DisableRemoteBehaviour()
    {
        for (int i = 0; i < DisabledComponents.GetLength(0); i++)
        {
            DisabledComponents[i].enabled = false;
        }

        for(int i = 0; i < DisabledObjects.GetLength(0); i++)
        {
            DisabledObjects[i].SetActive(false);
        }
    }

    public override void OnEvent(DamageEvent evnt)
    {
        if(state.IsDead)
        {
            Debug.Log("Player is alread dead!");
        }
        else
        {
            state.Health -= evnt.Damage;         
            LastDamaged = evnt.Player;
            GetComponent<AudioSource>().Play();
            if(state.Health <= 0)
            {
                state.IsDead = true;
                Die();
            }             
        }      
    }

    public override void OnEvent(ClaimKill evnt)
    {
        state.Kills += 1;
        state.XP += 5;
        PlayerManager.GetComponent<PlayerAuth>().XP += 5;
    }

    public override void OnEvent(PlayerFlashed evnt)
    {
        if(entity.IsOwner)
        {
            GuiManager.Flashed();
        }
    }

    public void AddScore(int Amount)
    {
        state.Score += Amount;
    }

    private void Die()
    {
        if (entity.IsOwner)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            var Event = PlayerKilled.Create();
            Event.Killed = entity;
            Event.Killer = LastDamaged;
            Event.Send();

            var Event2 = ClaimKill.Create(LastDamaged);
            Event2.Send();

            var Event3 = ClaimScore.Create(LastDamaged);
            Event3.Score = 100;
            Event3.Send();

            if (LastDamaged != entity)
            {
                LastDamaged.GetComponent<Player>().AddScore(100);
            }

            state.Deaths += 1;
            StartCoroutine(Respawn());
            if(LastDamaged == entity)
            {
                GuiManager.ShowDeathScreen("You've commited suicide...");
                if(state.Score > 0)
                {
                    state.Score -= 100;
                }
                if(state.Score < 0)
                {
                    state.Score = 0;
                }
            }
            else
            {
                GuiManager.ShowDeathScreen("You've been killed by: " + LastDamaged.GetComponent<Player>().state.Username);
            }       
            GuiManager.ShowScoreBoard();        
            gameObject.GetComponent<Renderer>().enabled = false;
        }
    }

    private IEnumerator Respawn()
    {
        transform.position = new Vector3(0, 0, 0); 
        yield return new WaitForSeconds(10);
        GuiManager.HideDeathScreen();
        GuiManager.HideScoreBoard();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.GetComponent<Renderer>().enabled = true;
        transform.rotation = Quaternion.identity;
        GetComponent<WeaponManager>().ReinstantiateWeapons();
        transform.position = GameObject.Find("GameManager").GetComponent<GameManager>().GetRandomSpawnpoint().position;    
        state.Health = 100;
        state.IsDead = false;
    }

    private void Update()
    {
        if(canUse == true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                int ReturnedValue = triggerColliding.gameObject.GetComponent<Trigger>().Activate(Points);

                if(ReturnedValue >= 0)
                {
                    Points -= ReturnedValue;
                }
            }
        }      
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "Trigger")
        {
            canUse = true;
            triggerColliding = col;
        }
    }

    private void OnTriggerLeave(Collider col)
    {
        if (col.transform.tag == "Trigger")
        {
            canUse = false;
            triggerColliding = null;
        }
    }
}
