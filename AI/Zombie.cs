using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour {

    private GameObject Player;
    private GameObject GameManager;
    private NavMeshAgent Agent;

    private Animator Anim;

    public double Health;
    public float Speed;

	private void Start ()
    {
        Agent = GetComponent<NavMeshAgent>();
        Anim = GetComponent<Animator>();

        Anim.Play("run");
	} 

    private void Update()
    {
        GameManager = GameObject.Find("GameManager");
        Player = GameObject.Find("Player");

        Agent.destination = Player.transform.position;
        Agent.speed = Speed;

        if(Health <= 0)
        {
            Die();
        }
    }

    public void DealDamage(int Damage, string Place)
    {
        double FinalDamage = 0;
        double Bonus = 0;

        switch(Place)
        {
            case "Head":
                Bonus = 1.5;
                break;
            case "Torso":
                Bonus = 1.1;
                break;
            case "Arm":
                Bonus = 0.7;
                break;
            case "UpperLeg":
                Bonus = 0.9;
                break;
            case "LowerLeg":
                Bonus = 0.5;
                break;
        }

        FinalDamage = Damage * Bonus;
        Debug.Log(FinalDamage);
        Health -= FinalDamage;
    }

    private void Die()
    {
        // Send points
         Destroy(gameObject);
        Player player = Player.GetComponent<Player>();
        player.Points += 50;
        GameManager.GetComponent<SpawnManagement>().ZombiesKilled += 1;
    }
}
