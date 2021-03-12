using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    // editor
    public Camera playerCamera;
    public AudioSource playerAudioSource;

    public float lookSensitivity = 200f;
    public float gravity = 20f;

    [Header("Movement")]
    public float speed = 4f;
    public float maxSpeed = 10f;
    public float speedCrouching = 3f;
    public float jumpForce = 9f;

    [Header("Stance")]
    public float heightStanding = 2f;
    public float heightCrouching = 1f;


    // private
    CharacterController m_characterController;
    float m_rotationX = 0f;
    float m_heightPlayer;


    void Start()
    {
        m_characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        /* look */
        float mouseX = Input.GetAxis(GameConstants.MOUSE_X) * lookSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(GameConstants.MOUSE_Y) * lookSensitivity * Time.deltaTime;
        m_rotationX -= mouseY;
        m_rotationX = Mathf.Clamp(m_rotationX, -90f, 90f); // limit -90~90

        playerCamera.transform.localRotation = Quaternion.Euler(m_rotationX, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        /* move */
        Vector3 move = transform.right * Input.GetAxis(GameConstants.HORIZONTAL) +
                       transform.forward * Input.GetAxis(GameConstants.VERTICAL);
        m_characterController.Move(move * speed * Time.deltaTime);
    }
}
