using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHitBox : MonoBehaviour
{
    public int damage = 10;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("colide");
        Debug.Log(other.tag);
        if (other.CompareTag("Monster"))
        {

            Debug.Log("monster kena");
            // MonsterHealth monster = other.GetComponent<MonsterHealth>();
            // if (monster != null)
            // {
            //     monster.TakeDamage(damage);
            // }
        }
    }
}
