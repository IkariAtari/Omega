using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScoreBoard : Bolt.EntityEventListener
{
    [SerializeField]
    private GameObject Row;

    private GameObject GameManager;

    private void OnEnable()
    {      
        GameManager = GameObject.Find("GameManager");

        Player[] players = GameManager.GetComponent<GameManager>().GetAllPlayers();
 
        var playersUnsorted = new Dictionary<Player, int>();
        
        for (int i = 0; i < players.GetLength(0); i++)
        {
            if (players[i] != null)
            {
                playersUnsorted.Add(players[i], players[i].state.Score);
            }
        }

        var playersSorted = from pair in playersUnsorted orderby pair.Value descending select pair;

        foreach (KeyValuePair<Player, int> pair in playersSorted)
        {
            if (pair.Key != null)
            {
                Debug.Log("Scoreboard player: " + pair.Key.state.Username);
                GameObject _row = BoltNetwork.Instantiate(BoltPrefabs.Row, transform.position, Quaternion.identity);
                _row.transform.SetParent(transform);
                Text Name = _row.transform.GetChild(0).GetComponent<Text>();
                Text Kills = _row.transform.GetChild(1).GetComponent<Text>();
                Text Deaths = _row.transform.GetChild(2).GetComponent<Text>();
                Text Score = _row.transform.GetChild(3).GetComponent<Text>();

                Name.text = pair.Key.state.Username;
                Kills.text = pair.Key.state.Kills.ToString();
                Deaths.text = pair.Key.state.Deaths.ToString();
                Score.text = pair.Key.state.Score.ToString();
            }
        }
    }

    private void OnDisable()
    {
        for(int i = 1; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
