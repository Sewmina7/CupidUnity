using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    [SerializeField]private float movingSpeed;
    [SyncVar]
    private Vector2 input;
    [SyncVar(hook = nameof(OnColorChanged))]
    private Color color;

    void Start()
    {
        if(isServer){
            SetPlayer();
        }  
        if(isLocalPlayer){
            FindObjectOfType<CameraFollower>().SetTarget(transform);
        }
    }

    void SetPlayer(){
        color = new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
    }

    void OnColorChanged(Color oldValue, Color newValue){
        GetComponent<MeshRenderer>().material.color = newValue;
    }
    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer){
            Vector2 m_input = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
            if(isServer){
                input = m_input;
            }else{
                CmdSetInput(m_input);
            }
        }

        if(isServer){
            transform.Translate(new Vector3(input.x,0,input.y)*movingSpeed);
        }
    }

    [Command]
    void CmdSetInput(Vector2 _input){
        input = _input;
    }
}
