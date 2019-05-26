using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    [SerializeField]
    private GameObject Enemy;

    public void Spawn(double Health, float Speed)
    {
        GameObject EnemyInstance = (GameObject)Instantiate(Enemy, transform.position, Quaternion.identity);
        Zombie zombie = EnemyInstance.GetComponent<Zombie>();
        zombie.Health = Health;
        zombie.Speed = Speed;
    }
}
