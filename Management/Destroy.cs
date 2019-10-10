using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : Bolt.EntityBehaviour
{
    public override void Attached()
    {
        StartCoroutine(DestroySelf());
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(30);

        BoltNetwork.Destroy(gameObject);
    }
}
