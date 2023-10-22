using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerRotation : MonoBehaviour
{
    public GameObject player;
    public Vector3 cameraOffset = new Vector3(0f, 6f, -6f); // Offset càmera
    public float speedCamera; // Velocitat de la càmera

    void LateUpdate()
    // Per evitar la tremolor o jittering de la camera, es millor utilitzar LateUpdate() en comptes de Update()
    // Això és degut a que si ho fem amb Update s'encadenen els dos moviments player i càmera i es produeix el jittering
    {
        if (player != null) // Per assegurar que el player no és null
        {
            // // == OPCIÓ 0 ==
            // // la càmera segueix al cotxe però al canviar de sentit la càmera no gira
            // transform.position = player.transform.position + cameraOffset;

            // // == OPCIÓ 1 ==
            // // Per tal de que al canviar de sentit la càmera giri, mourem la rotació de la càmera segons el player
            // // i s'aplica l'offset i un espai darrera per corregir la posició segons el sentit del player
            // transform.position = player.transform.position + player.transform.TransformVector(cameraOffset) - player.transform.forward * 7f;
            // transform.rotation = player.transform.rotation;

            // == OPCIÓ 2 ==
            // Per tal de que suavitzi el moviment de la càmera, utilitzarem el mètode Lerp()
            // Per fer que la càmera miri al player, utilitzarem el mètode LookAt()
            transform.position = Vector3.Lerp(transform.position,
                player.transform.position + player.transform.TransformVector(cameraOffset),
                speedCamera * Time.deltaTime);
            // Mirarem la posició del player i hi afegim una distància davant perquè no quedi al centre de la càmera
            transform.LookAt(player.transform.position + player.transform.forward * 4f);
        }
        else
        {
            Debug.Log("Player not linked to camera");
        }
    }
}
