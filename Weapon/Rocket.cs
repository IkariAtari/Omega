using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public int BaseDamage;
    public float Range;
    public LayerMask Mask;
    public float ExplosionForce;
    public BoltEntity Throwed;  
    public int ExplosionTime;
   
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.up * 10000f);
        GetComponent<Rigidbody>().AddRelativeTorque(Vector3.up * 1000f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            Explode();
            BoltNetwork.Instantiate(BoltPrefabs.FXGrenade, transform.position, Quaternion.FromToRotation(Vector3.up, collision.transform.position) * Quaternion.Euler(Vector3.up * Random.Range(0, 360)));
        }
    }

    private void Explode()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, Range);


        foreach (Collider hit in hits)
        {
            if (hit.GetComponent<Rigidbody>() != null)
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


                    var Event = DamageEvent.Create(hit.GetComponent<BoltEntity>());
                    Event.Damage = BaseDamage / Mathf.RoundToInt(Distance);
                    Event.Player = Throwed;
                    Event.Send();
                    break;
                }
            }
        }

        BoltNetwork.Destroy(gameObject);
    }
}
