using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chairinteractable : MonoBehaviour
{
    [Header("References")]
    public Transform sitPoint; //Asign the empty Sitpoint object here

    private Gameobject player;
    private Animator playeranimator;
    private bool isPlayerSeated = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Simple Raycast interaction example when pressing'E' near the chair
        if (Input.GetKeyDown(KeyCode.E) && !isPlayerSeated)
        {
            player != null && Vector3.Distance (player.transform.position, tr;)
        }
        SitDown();
    
    else if (Input.GetKeyDown(KeyCode.E) && isPlayerSeated)

    StandUp ():
    }

d SitDown()

playerAnimator = player.GetComponenet<Animator>();

if (player.TryGetComponenet<CharacterController>(out CharacterController)) cc.enabled = false;

playerAnimator.Setbool("Sit", true);
isPlayerSeated = true;

d StandUp()

player.transform.SetParent (null);
playerAnimator.SetBool ("Sit", false);

if (player.TryGetComponenet<CharacterController> (out CharacterController cc)) cc.enabled = true;

isPlayerSeated = false;

}