using KBCore.Refs;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using Utilities;

namespace CroakCreek
{
    public class PlayerController : ValidatedMonoBehaviour
    {
        [Header("References")]
        [SerializeField, Self] Rigidbody rb;
        [SerializeField, Self] GroundChecker groundChecker;
        [SerializeField, Self] Animator animator;
        [SerializeField, Anywhere] InputReader input;

        [Header("Movement Settings")]
        [SerializeField] float walkSpeed = 400f;
        [SerializeField] float runSpeed = 500f;
        [SerializeField] float rotationSpeed = 15f;
        [SerializeField] float smoothTime = 0.2f;

        private bool isRunning = false;
        private float moveSpeed => isRunning ? runSpeed : walkSpeed;

        [Header("Jump Settings")]
        [SerializeField] float jumpForce = 10f;
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float jumpMaxHeight = 2f;
        [SerializeField] float gravityMultiplier = 8f;
        [SerializeField] float ascentGravityMultiplier = 2f; // or 2.5f — tweakable

        const float ZeroF = 0f;

        float currentSpeed;
        float velocity;
        float jumpVelocity;

        Vector3 movement;

        List<Utilities.Timer> timers;
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;

        private void Awake()
        {
            rb.freezeRotation = true;

            // setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            timers = new List<Utilities.Timer>(capacity: 2) { jumpTimer, jumpCooldownTimer };

            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        }

        private void OnEnable()
        {
            input.Jump += OnJump;
            input.Run += OnRun;
        }

        private void OnDisable()
        {
            input.Jump -= OnJump;
            input.Run -= OnRun;
        }

        void OnJump(bool performed)
        {
            if (performed && !jumpTimer.IsRunning && !jumpCooldownTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpTimer.Start();
            }
            else if (!performed && jumpTimer.IsRunning)
            {
                jumpTimer.Stop();
            }
        }

        private void OnRun(bool runState)
        {
            isRunning = runState;
        }

        private void Update()
        {
            movement = new Vector3(input.Direction.x, 0f, input.Direction.y);

            HandleTimers();
            //UpdateAnimator();
        }

        private void FixedUpdate()
        {
            HandleJump();
            HandleMovement();
        }

        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        void HandleJump()
        {
            // If not jumping and grounded, keep jump velocity at 0
            if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpVelocity = ZeroF;
                jumpTimer.Stop();
                return;
            }

            // If jumping or falling calculate velocity
            if (jumpTimer.IsRunning)
            {
                if (jumpTimer.Progress > 0f)
                {
                    // Use stronger gravity to make ascent faster
                    float effectiveGravity = Mathf.Abs(Physics.gravity.y) * ascentGravityMultiplier;
                    jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * effectiveGravity);
                }
            }
            else
            {
                // Gravity Takes over
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            // Apply Velocity
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
        }

        private void HandleMovement()
        {
            var adjustedDirection = movement.normalized;
            if (adjustedDirection.magnitude > ZeroF)
            {
                HandleHorizontalMovement(adjustedDirection);
                SmoothSpeed(adjustedDirection.magnitude);

            }
            else
            {
                SmoothSpeed(ZeroF);
                rb.linearVelocity = new Vector3(ZeroF, rb.linearVelocity.y, ZeroF);
            }
        }

        void HandleHorizontalMovement(Vector3 adjustedDirection)
        {
            // Move the player
            Vector3 velocity = adjustedDirection * moveSpeed * Time.fixedDeltaTime;
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }

        void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }
}



// Might Re-use

//  HandleRotation(adjustedDirection);
//  void HandleRotation(Vector3 adjustedDirection)
//  {
//      var targetRotation = Quaternion.LookRotation(adjustedDirection);
//      transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
//      transform.LookAt(transform.position + adjustedDirection);
//  }