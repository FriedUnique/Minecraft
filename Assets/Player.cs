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
    private float vLook = 90;
    private Vector3 moveAmount;
    private Vector3 smoothVelocity;



    public Vector3 GetPos() {
        return gameObject.transform.position;
    }

    public Vector2 GetCurrentChunk() {
        return new Vector2(Mathf.FloorToInt(GetPos().x / Chunk.chunkWidth), Mathf.FloorToInt(GetPos().z / Chunk.chunkWidth));
    }

    private void Start() {
        rb = GetComponentInParent<Rigidbody>();

        // works only for the editor, because of the editor protection
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate() {
        Vector3 direction = Vector3.zero;
        direction += transform.forward * Input.GetAxisRaw("Vertical");
        direction += transform.right * Input.GetAxisRaw("Horizontal");

        if (isGrounded()) {
            if (Input.GetKey(KeyCode.Space)) {
                //Debug.Log("Space pressed");
                rb.AddForce(Vector3.up * jumpForce);
            }
        }

        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * getSpeed(), ref smoothVelocity, smoothTime); // smooth out the movement so it isnt so edgy

        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.deltaTime);

        Turn();
    }

    void Turn() {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSens);

        vLook += Input.GetAxisRaw("Mouse Y") * mouseSens;
        vLook = Mathf.Clamp(vLook, -90f, 90f);

        cam.transform.localEulerAngles = Vector3.left * vLook;
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
