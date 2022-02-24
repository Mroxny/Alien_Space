using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private bool fallowMouse = false;
    private bool fallowTouch = false;
    private bool tapBlock = false;



    void Start()
    {
        if (Application.isEditor) print("Editor");
        else print("Device");
    }

    void Update()
    {

        


        //Mobile controlls
        if (Input.touchCount > 0)
        {

            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);


            if (Vector2.Distance(transform.position, touchPos) < .2f && !fallowTouch)
            {
                fallowTouch = true;
            }
            
            if (fallowTouch)
            {
                transform.position = new Vector3(touchPos.x, transform.position.y, transform.position.z);
            }
            else if (touch.tapCount >= 1 && !tapBlock)
            {
                gameObject.GetComponent<PlayerManager>().Shoot();
                tapBlock = true;
            }
        }
        else fallowTouch = tapBlock = false;





        //Controlls in editor
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector2.Distance(transform.position, mousePos) < .2f && !fallowMouse)
            {
                fallowMouse = true;
            }
            if (fallowMouse)
            {
                transform.position = new Vector3(mousePos.x, transform.position.y, transform.position.y);
            }
            else if (Input.GetMouseButtonDown(0)) {
                
                //print("Mouse tap");
            }
        }
        else fallowMouse = false;
    }
}
