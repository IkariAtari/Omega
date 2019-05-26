using System.Collections;
using System.Collections.Generic;
using Bolt;
using UnityEngine;
using System.Linq;

[BoltGlobalBehaviour(BoltNetworkModes.Server)]
public class GameManager : Bolt.GlobalEventListener {

    public GameObject[] Calibers;
    public GameObject[] SpawnPoints;

    private GameManager instance;

    public MatchManager matchManager;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.Log("More than on GameManager!");
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        matchManager = GetComponent<MatchManager>();
        GetSpawnPoints();
        GameObject[] NoRendObjects = GameObject.FindGameObjectsWithTag("NoRend");
        GameObject[] NoRendObjects2 = GameObject.FindGameObjectsWithTag("Trigger");
        GameObject[] NoRendObjects3 = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (GameObject _object in NoRendObjects)
        {
            _object.GetComponent<Renderer>().enabled = false;
        }

        foreach (GameObject _object in NoRendObjects2)
        {
            _object.GetComponent<Renderer>().enabled = false;
        }

        foreach (GameObject _object in NoRendObjects3)
        {
            _object.GetComponent<Renderer>().enabled = false;
        }
    }

    private void GetSpawnPoints()
    {
        SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
    }

    public Transform GetRandomSpawnpoint()
    {
        int i = Random.Range(0, SpawnPoints.GetLength(0));

        return SpawnPoints[i].transform;
    }

    public override void OnEvent(PlayerKilled evnt)
    {
        Player[] players = GetAllPlayers();       

        foreach (Player Player in players)
        {
            Player.GetComponent<GUImanager>().KilFeedAdd(evnt.Killed.GetComponent<Player>().state.Username, evnt.Killer.GetComponent<Player>().state.Username);        
        }      
    }

    public override void OnEvent(PlayerRegistered evnt)
    {      
        matchManager.RegisterPlayer(evnt.Entity);
    }

    public Player[] GetAllPlayers()
    {
        Debug.Log(matchManager.state.Players.Length);
        Player[] _players = new Player[matchManager.state.Players.Length];

        for (int i = 0; i < matchManager.state.Players.Length; i++)
        {
            if (matchManager.state.Players[i] != null)
            {
                _players[i] = matchManager.state.Players[i].GetComponent<Player>();
            }
        }

        return _players;
    }
}
 