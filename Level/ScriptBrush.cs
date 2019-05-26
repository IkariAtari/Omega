using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ScriptBrush : MonoBehaviour {

    [SerializeField]
    private GameObject Target;
    
    [SerializeField]
    private float Speed;

    [SerializeField]
    private string Action;

    private bool isActivated = false;

    public void Activate()
    {
        isActivated = true;
    }

    private void Update()
    {
        Debug.DrawLine(GetComponent<Renderer>().bounds.center, Target.GetComponent<Renderer>().bounds.center, Color.green);

        if (isActivated == true)
        {
            switch (Action)
            {
                case "MoveToTarget":
                    MoveToTarget();
                    break;
            }
        }
    }

    private void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, Speed * Time.deltaTime);
    }
}
