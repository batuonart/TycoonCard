using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerTestMove : NetworkBehaviour
{
    // Start is called before the first frame update




    public override void OnNetworkSpawn()
    {

    }


    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            transform.Translate(Vector3.right * 3f * Time.deltaTime);
        }
    }
}
