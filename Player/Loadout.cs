using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Loadout : MonoBehaviour {

    public int Points;

    public GameObject Primary;
    public GameObject Secondary;
    public GameObject Lethal;
    public GameObject NonLethal;

    public string Type;
    
    public int[] AvaibleItems;
}
