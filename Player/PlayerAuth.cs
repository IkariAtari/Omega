using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAuth : MonoBehaviour
{
    public string Name;
    public int Rank;
    public int XP;

    public void CreateSession(string _username)
    {
        Name = _username;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
