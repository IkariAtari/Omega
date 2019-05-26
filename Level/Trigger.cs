using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Trigger : MonoBehaviour {

    private GameObject GameManager;
    private GUImanager GuiManager;
    
    [SerializeField]
    private string TipString;

    [SerializeField]
    private int Cost;

    [SerializeField]
    private bool RemoveOnTriggered;

    [SerializeField]
    private GameObject LinkedObject;


    private void Update()
    {
        Debug.DrawLine(GetComponent<Renderer>().bounds.center, LinkedObject.GetComponent<Renderer>().bounds.center, Color.green);
    }

    private void Start()
    {
        GameManager = GameObject.Find("GameManager");
        GuiManager = GameManager.GetComponent<GUImanager>();
    }

    // If use trigger
    public int Activate(int PlayerPoints)
    {
        if(PlayerPoints >= Cost)
        {
            LinkedObject.GetComponent<ScriptBrush>().Activate();
            if (RemoveOnTriggered == true)
            {
                TipString = "";
            }
            return Cost;       
        }
        else
        {
            return -1;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
       if(col.transform.tag == "Player")
       {
            GuiManager.SetToolTip(TipString);
       }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.transform.tag == "Player")
        {
            GuiManager.UnsetToolTip();
        }
    }
}
