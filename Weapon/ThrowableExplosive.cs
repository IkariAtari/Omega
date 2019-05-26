using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableExplosive : Bolt.EntityBehaviour<IExplosive>
{
    public int BaseDamage;
    public GameObject FX;
    public float Range;
    public LayerMask Mask;
    public float ExplosionForce;
    public BoltEntity Throwed;
    public string Type;
    public int ExplosionTime;

    public override void Attached()
    {
        gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 1000f);
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        yield return new WaitForSeconds(ExplosionTime);

        Collider[] hits = Physics.OverlapSphere(transform.position, Range);
        

        foreach(Collider hit in hits)
        {   
            if(hit.GetComponent<Rigidbody>() != null)
            {
                hit.GetComponent<Rigidbody>().AddExplosionForce(ExplosionForce, transform.position, Range, 0f);
            }
            if (hit.transform.tag == "Player")
            {
                RaycastHit Hit;
                Physics.Linecast(transform.position, hit.transform.position, out Hit, Mask);
                Debug.Log(Hit.transform.tag);
                if (Hit.transform.tag == "Player")
                {
                    Debug.Log(Vector3.Distance(transform.position, hit.transform.position));
                    float Distance = Vector3.Distance(transform.position, hit.transform.position);               
                    switch(Type)
                    {
                        case "frag":
                            var Event = DamageEvent.Create(hit.GetComponent<BoltEntity>());
                            Event.Damage = BaseDamage / Mathf.RoundToInt(Distance);
                            Event.Player = Throwed;
                            Event.Send();
                            break;
                        case "flash":
                            var EventFlashed = PlayerFlashed.Create(hit.GetComponent<BoltEntity>());
                            EventFlashed.Send();
                            break;
                    }
                    
                }
            }
        }

        SpawnFX();
    }

    private void SpawnFX()
    {
        switch(Type)
        {
            case "frag":
                BoltNetwork.Instantiate(BoltPrefabs.FXGrenade, transform.position, Quaternion.identity);
                break;
            case "flash":
                BoltNetwork.Instantiate(BoltPrefabs.FXFlashBang, transform.position, Quaternion.identity);
                break;
        }    
        BoltNetwork.Destroy(gameObject);
    }
}
