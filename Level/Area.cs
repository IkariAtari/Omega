using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Area : MonoBehaviour {

    public GameObject[] LinkedSpawners;
    private GameObject GameManager;

    [SerializeField]
    private int AreaNumber;

    private void Start()
    {
        GameManager = GameObject.Find("GameManager");
    }

    private void Update()
    {
        foreach (GameObject LinkedObject in LinkedSpawners)
        {
            Debug.DrawLine(GetComponent<Renderer>().bounds.center, end: LinkedObject.GetComponent<Renderer>().bounds.center, color: Color.red);
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if(col.transform.tag == "Player")
        {
            GameManager.GetComponent<SpawnManagement>().CurrentArea = AreaNumber;
        }
    }

    private void OnTriggerLeave(Collider col)
    {
        if (col.transform.tag == "Player")
        {
            GameManager.GetComponent<SpawnManagement>().CurrentArea = 0;
        }
    }
}
