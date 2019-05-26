using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManagement : MonoBehaviour {

    private GameManager GameManager;

    public GameObject[] Areas;
    public int CurrentArea;

    //Zombies killed in current round
    [HideInInspector]
    public int ZombiesKilled;

    //Zombies spawned
    private int ZombiesCounter;

    //Zombies to be spawned in a round
    private int ZombiesPerRound;

    [SerializeField]
    private int[] SpawnRate = new int[2];
    [SerializeField]
    private double ZombiesExponent;
    [SerializeField]
    private double HealthExponent;
    [SerializeField]
    private float SpeedExponent;
    [SerializeField]
    private float SpeedLimit;

    [SerializeField]
    private double ZombiesBaseHealth;
    [SerializeField]
    private float ZombiesBaseSpeed;
    [SerializeField]
    private int ZombiesBaseAmount;

    private double ZombiesHealth;
    private float ZombiesSpeed;

    //Interger user for timing
    private int SpawnTimer;

    //Which round the game is in
    [HideInInspector]
    public int RoundCounter;

    //Boolean that indicates if spawning is allowed(not in between rounds)
    private bool Spawning;

    private void Start()
    {
        SpawnTimer = 0;
        RoundCounter = 1;
        ZombiesPerRound = ZombiesBaseAmount;
        ZombiesKilled = 0;
        Spawning = true;

        ZombiesHealth = ZombiesBaseHealth;
        ZombiesSpeed = ZombiesBaseSpeed;
    }

    private void Update()
    {
        if (Spawning)
        {
            if (ZombiesCounter != ZombiesPerRound)
            {
                if (SpawnTimer > 0)
                {
                    SpawnTimer--;
                }
                else
                {
                    if (Areas != null)
                    {
                        for (int i = 0; i < Areas[CurrentArea].GetComponent<Area>().LinkedSpawners.Length; i++)
                        {
                            if (ZombiesCounter != ZombiesPerRound) { 
                                Areas[CurrentArea].GetComponent<Area>().LinkedSpawners[i].GetComponent<Spawner>().Spawn(ZombiesHealth, ZombiesSpeed);
                                ZombiesCounter += 1;
                            }
                        }

                        SpawnTimer = Random.Range(SpawnRate[0], SpawnRate[1]);                       
                    }
                }
            }
            if (ZombiesKilled == ZombiesPerRound)
            {
                StartCoroutine(NewRound());
            }
        }
    }

    private IEnumerator NewRound()
    {
        Spawning = false;

        yield return new WaitForSeconds(10);
        
        //Add the zombies for the next round
        double z = System.Math.Round(ZombiesPerRound * ZombiesExponent);
        ZombiesPerRound = (int)z;

        //Add health to the zombies for the next round
        ZombiesHealth *= HealthExponent;

        //Add speed to the zombies for the next round
        if(!(ZombiesSpeed >= SpeedLimit))
        {
            ZombiesSpeed *= SpeedExponent;
        }

        RoundCounter += 1;
        ZombiesCounter = 0;
        ZombiesKilled = 0;

        Spawning = true;

        yield break;
    }
}