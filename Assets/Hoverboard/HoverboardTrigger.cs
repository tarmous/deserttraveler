using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoverboardTrigger : MonoBehaviour, Mountable
{
    GameObject player;
    [SerializeField] GameObject mVec;
    public void Mount()
    {
        //Makes the GameObject "newParent" the parent of the GameObject "player".
        player.transform.parent = mVec.transform;
        
        player.transform.position = mVec.transform.position;
        //Display the parent's name in the console.
        Debug.Log("Player's Parent: " + player.transform.parent.name);

        // Check if the new parent has a parent GameObject.
        if (mVec.transform.parent != null)
        {
            //Display the name of the grand parent of the player.
            Debug.Log("Player's Grand parent: " + player.transform.parent.parent.name);
        }
        Player._instance.MountedOn = this.gameObject;

        GetComponent<SphereCollider>().enabled = false;

        GetComponentInParent<HoverMotor>().enabled = true;

    }

    public void Dismount()
    {
        // Detaches the transform from its parent.
        GetComponent<SphereCollider>().enabled = true;
        GetComponentInParent<HoverMotor>().enabled = false;
    }
    void OnTriggerEnter(Collider other)
    {   
        if(other.gameObject.tag == "Player")
            player = other.gameObject;

    }

    void OnTriggerStay(Collider other)
    {   
        if(other.gameObject.tag == "Player")
        {   
            Debug.Log("We see Player");
            if (!Player._instance) return;
            Debug.Log(Player._instance.InteractProperty);
            if (Player._instance.InteractProperty)
            {
                Mount();
                Player._instance.Mount();
                
            }
                        
        }

    }
}
