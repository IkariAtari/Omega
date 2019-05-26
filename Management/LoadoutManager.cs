using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadoutManager : MonoBehaviour {

    [SerializeField]
    public Loadout[] Loadouts = new Loadout[5];

    public GameObject[] weapons;

    public int LoadoutChosen;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetLoadout(int _loadout)
    {
        LoadoutChosen = _loadout;
    }
}
