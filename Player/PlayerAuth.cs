using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAuth : MonoBehaviour
{
    public string Name;

    public void CreateSession(string _username)
    {
        name = _username;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
