using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[BoltGlobalBehaviour]
public class NetworkCallbacks : Bolt.GlobalEventListener
{
    private GameManager Manager;
    
    public override void SceneLoadLocalDone(string map)
    {
        Manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        BoltNetwork.Instantiate(BoltPrefabs.Player, Manager.GetRandomSpawnpoint().position, Quaternion.identity);
    }  
}
