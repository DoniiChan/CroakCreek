using CroakCreek;
using KBCore.Refs;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.Mathematics;
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
        [SerializeField] StaminaManager staminaManager;
        [SerializeField, Anywhere] InputReader input;
        [SerializeField] private LockOn lockOn;

        [Header("Movement Settings")]
        [SerializeField] float walkSpeed = 400f;
        [SerializeField] float runSpeed = 500f;
        [SerializeField] float smoothTime = 0.2f;

        [Header("Dash Settings")]
        [SerializeField] float dashForce = 20f;
        [SerializeField] float dashDuration = 0.2f;
        [SerializeField] float dashCooldown = 1f;
        [SerializeField] float runHoldDuration = 0.5f;
        [SerializeField] int dashCost = 5;

        [Header("Jump Settings")]
        [SerializeField] float jumpDuration = 0.5f;
        [SerializeField] float jumpCooldown = 0f;
        [SerializeField] float jumpMaxHeight = 2f;
        [SerializeField] float gravityMultiplier = 8f;
        [SerializeField] float ascentGravityMultiplier = 2f;
        [SerializeField] float coyoteTime = 0.2f;
        [SerializeField] float jumpBufferTime = 0.2f;

        [Header("Fire Settings")]
        [SerializeField] private Transform netObject;
        [SerializeField] private PlayerAim playerAim;
        [SerializeField] private Transform netSpawnPoint;

        private NetScript netScript;

        private bool isRunning;
        private bool runInputHeld;
        private bool jumpInputHeld;
        private bool isDashing;
        private bool outOfSta;
        private bool canRun;
        private bool wasGrounded;

        const float ZeroF = 0f;
        float moveSpeed => isRunning ? runSpeed : walkSpeed;
        float currentSpeed;
        float velocity;
        float jumpVelocity;

        Vector3 movement;
        Vector3 dashDirection;

        [Header("Timers")]
        List<Utilities.Timer> timers;
        // Jump Timers
        CountdownTimer jumpTimer;
        CountdownTimer jumpCooldownTimer;
        CountdownTimer coyoteTimer;
        CountdownTimer jumpBufferTimer;

        // Dash Timers
        CountdownTimer dashTimer;
        CountdownTimer dashCooldownTimer;

        // Run Timers
        StopwatchTimer runTimer;
        StopwatchTimer runHoldTimer;

        private void Awake()
        {
            isRunning = false;
            runInputHeld = false;
            isDashing = false;
            outOfSta = false;
            canRun = true;

            rb.freezeRotation = true;

            netScript = netObject.GetComponent<NetScript>();
            
            // setup timers
            jumpTimer = new CountdownTimer(jumpDuration);
            jumpCooldownTimer = new CountdownTimer(jumpCooldown);
            coyoteTimer = new CountdownTimer(coyoteTime);
            jumpBufferTimer = new CountdownTimer(jumpBufferTime);

            dashTimer = new CountdownTimer(dashDuration);
            dashCooldownTimer = new CountdownTimer(dashCooldown);

            runTimer = new StopwatchTimer();
            runHoldTimer = new StopwatchTimer();

            timers = new List<Utilities.Timer>(capacity: 8) { jumpTimer, jumpCooldownTimer, coyoteTimer, jumpBufferTimer, dashTimer, dashCooldownTimer, runTimer, runHoldTimer };

            jumpTimer.OnTimerStop += () => jumpCooldownTimer.Start();
        }

        private void OnEnable()
        {
            input.Jump += OnJump;
            input.Run += OnRun;
            input.Fire += OnFire;
            input.Lock += OnLock;
            input.Look += playerAim.OnLookInput;
        }

        private void OnDisable()
        {
            input.Jump -= OnJump;
            input.Run -= OnRun;
            input.Fire -= OnFire;
            input.Lock -= OnLock;
            input.Look -= playerAim.OnLookInput;
        }

        void OnJump(bool performed)
        {
            jumpInputHeld = performed;

            if (performed)
            {
                jumpBufferTimer.Start(); // store jump input
            }
            else if (!performed && jumpTimer.IsRunning)
            {
                jumpTimer.Stop(); // stop early jump for variable height
            }
        }

        private void OnFire(bool performed)
        {
            if (performed)
            {
                ThrowNet();
            }
        }

        public void OnLock(bool performed)
        {
            if (performed)
                lockOn.ToggleLock();
        }

        private void OnRun(bool runState)
        {
            if (runState && staminaManager.currentSta > 0)
            {
                runInputHeld = true;
                runHoldTimer.Reset();
                runHoldTimer.Start();
            }
            else if (staminaManager.currentSta <= 0)
            {
                outOfSta = true;
            }
            else
            {
                runInputHeld = false;
                runHoldTimer.Stop();
                outOfSta = false;

                // If released before threshold, it's a tap (dash)
                if (runHoldTimer.GetTime() < runHoldDuration &&
                    !isRunning &&
                    !dashTimer.IsRunning &&
                    !dashCooldownTimer.IsRunning &&
                    staminaManager.currentSta >= dashCost &&
                    groundChecker.IsGrounded)
                {
                    isDashing = true;
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, ZeroF, rb.linearVelocity.z);
                    dashTimer.Start();
                    dashCooldownTimer.Start();

                    staminaManager.DecreaseStamina(dashCost);
                }
            }
        }

        private void Update()
        {
            // Movement
            movement = new Vector3(input.Direction.x, ZeroF, input.Direction.y);

            // HandleDash direction
            if (movement.magnitude == ZeroF)
            {
                dashDirection.x = ZeroF;
                dashDirection.y = ZeroF;
                dashDirection.z = -1f;
            }
            else
            {
                dashDirection = movement.normalized;
            }

            HandleTimers();

            // Handle Run input
            if (runInputHeld && runHoldTimer.GetTime() >= runHoldDuration && staminaManager.currentSta > 0 && canRun)
                isRunning = true;
            else
                isRunning = false;

            // Handle Stamina
            if (isRunning && movement.magnitude > 0f)
                HandleStaminaDrain();
            else
                HandleStaminaRegen();

            // Handle Coyote Time
            if (!groundChecker.IsGrounded && wasGrounded)
            {
                coyoteTimer.Start();
            }

            wasGrounded = groundChecker.IsGrounded;
        }

        private void FixedUpdate()
        {
            if (isDashing)
                HandleDash();
            else
            {
                HandleJump();
                HandleMovement();
            }
        }

        void HandleTimers()
        {
            foreach (var timer in timers)
            {
                timer.Tick(Time.deltaTime);
            }
        }

        private void HandleMovement()
        {
            var adjustedDirection = movement.normalized;
            if (adjustedDirection.magnitude > 0.01f)
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
            Vector3 velocity = adjustedDirection * moveSpeed * Time.deltaTime;
            rb.linearVelocity = new Vector3(velocity.x, rb.linearVelocity.y, velocity.z);
        }

        void HandleDash()
        {
            Vector3 dashVelocity = dashDirection * dashForce;

            rb.linearVelocity = new Vector3(dashVelocity.x, rb.linearVelocity.y, dashVelocity.z);

            if (!dashTimer.IsRunning)
            {
                isDashing = false;
            }
        }

        void HandleJump()
        {
            bool canJump = (groundChecker.IsGrounded || coyoteTimer.IsRunning) &&
                           !jumpCooldownTimer.IsRunning &&
                           !jumpTimer.IsRunning &&
                           jumpBufferTimer.IsRunning;

            if (canJump)
            {
                jumpTimer.Start();
                jumpBufferTimer.Stop();
                coyoteTimer.Stop();
                //jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * Mathf.Abs(Physics.gravity.y) * ascentGravityMultiplier);
            }

            if (!jumpTimer.IsRunning && groundChecker.IsGrounded)
            {
                jumpVelocity = ZeroF;
                return;
            }

            if (jumpTimer.IsRunning)
            {
                float effectiveGravity = Mathf.Abs(Physics.gravity.y) * ascentGravityMultiplier;
                jumpVelocity = Mathf.Sqrt(2 * jumpMaxHeight * effectiveGravity);
            }
            else
            {
                jumpVelocity += Physics.gravity.y * gravityMultiplier * Time.fixedDeltaTime;
            }

            if (!jumpInputHeld && jumpTimer.IsRunning)
            {
                jumpTimer.Stop();
            }

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);

        }


        private void HandleStaminaDrain()
        {
            if (!outOfSta && isRunning && movement.magnitude > 0f)
            {
                if (!runTimer.IsRunning)
                    runTimer.Start();

                if (runTimer.GetTime() >= 0.25f)
                {
                    staminaManager.DecreaseStamina(1);
                    runTimer.Reset();

                    if (staminaManager.currentSta <= 0)
                    {
                        isRunning = false;
                        outOfSta = true;
                        canRun = false;
                        Debug.Log("Out of Stamina!");
                    }
                }
            }
            else
            {
                runTimer.Stop();
            }
        }

        void HandleStaminaRegen()
        {
            if (!isRunning && staminaManager.currentSta < staminaManager.maxSta)
            {
                if (!runTimer.IsRunning)
                    runTimer.Start();

                if (runTimer.GetTime() >= 0.135f)
                {
                    staminaManager.RestoreStamina(1);
                    runTimer.Reset();

                    if (!canRun && staminaManager.currentSta >= staminaManager.maxSta)
                    {
                        canRun = true;
                        outOfSta = false;
                        Debug.Log("Can run again!");
                    }
                }
            }
            else
            {
                runTimer.Stop();
            }
        }

        private void ThrowNet()
        {
            if (lockOn.target == null)
            {
                netScript.Throw(playerAim.AimDirection);
            }
            //if (!netScript.IsCoolingDown())
            //{
            //    if (lockOn.target == null)
            //    {
            //        netScript.Throw(playerAim.AimDirection);
            //    }
            //    else
            //    {
            //        Vector3 directionToTarget = (lockOn.target.position - transform.position).normalized;
            //        netScript.Throw(directionToTarget);
            //    }

            //}
        }

        void SmoothSpeed(float value)
        {
            currentSpeed = Mathf.SmoothDamp(currentSpeed, value, ref velocity, smoothTime);
        }
    }
}
