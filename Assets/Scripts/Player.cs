using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Camera cam;

    public float mouseSens;
    public float walkSpeed, inAirSpeed, sneekSpeed, sprintSpeed; // should be expressed in blocks per second
    public float jumpForce; // calculate
    public float smoothTime;

    private Rigidbody rb;
    private float vLook = 90f;
    private float hLook = 90f;
    private Vector3 moveAmount;
    private Vector3 smoothVelocity;

    private float walkTime;
    private PlayerSkinHandler skinHandler;


    public Vector3 GetPos() {
        return gameObject.transform.position;
    }
    public Vector2 GetCurrentChunk() {
        return new Vector2(Mathf.FloorToInt(GetPos().x / Chunk.width), Mathf.FloorToInt(GetPos().z / Chunk.width));
    }


    private void Start() {
        rb = GetComponentInParent<Rigidbody>();

        skinHandler = transform.GetChild(0).GetComponent<PlayerSkinHandler>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void FixedUpdate() { 
        if (isGrounded()) {
            if (Input.GetKey(KeyCode.Space)) {
                //Debug.Log("Space pressed");
                rb.AddForce(Vector3.up * jumpForce);
            }
        }

        Move();
        Turn();

    }

    private void Turn() {
        hLook += Input.GetAxisRaw("Mouse X") * mouseSens;
        float a = hLook;
        hLook = Mathf.Clamp(hLook, -45f, 45f);
        float difference = a - hLook;

        vLook += -Input.GetAxisRaw("Mouse Y") * mouseSens;
        vLook = Mathf.Clamp(vLook, -179f, 0f);

        PlayerSkinHandler.getHead().localEulerAngles = new Vector3(vLook, hLook, 0);
        transform.Rotate(Vector3.up * difference);
    }

    private void Move() {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        // move animation
        if(moveDir != Vector3.zero) { skinHandler.WalkAnimation(ref walkTime, 5f);  }
        else { 
            walkTime = 0f;
            skinHandler.ResetAnimation();
        }

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * getSpeed(), ref smoothVelocity, smoothTime); // smooth out the movement so it isnt so edgy

        // switching rotation for calculation
        Vector3 rot = transform.rotation.eulerAngles;
        transform.eulerAngles += Vector3.up * hLook;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.deltaTime);
        transform.eulerAngles = rot;
    }



    void changeGravity(bool isGround) {
        if (isGround) {
            Physics.gravity = new Vector3(0, -9.81f, 0);
        }else if (!isGround) {
            Physics.gravity = new Vector3(0, -12f, 0);
        }
    }

    float getSpeed() {
        if (!isGrounded()) return inAirSpeed;

        else if (Input.GetKey(KeyCode.LeftShift)) return sprintSpeed;

        else return walkSpeed;
    }

    bool isGrounded() {
        if(Mathf.Abs(rb.velocity.y) > 0.001f) {
            changeGravity(false);
            return false;
        }
        changeGravity(true);
        return true;
    }

}
