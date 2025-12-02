using System;
using UnityEngine;

public class FrogLegs : MonoBehaviour
{
    [SerializeField] private float cooldownReductionOnDash; //in seconds e.g: 0.05
    private Transform Player;
    private DashAbility dashAbility;
    private float cooldownReducedBySeconds;

    private void Awake()
    {
        Player = this.transform.root;
        dashAbility = Player.GetComponentInChildren<DashAbility>();
    }

    private void OnEnable()
    {
        dashAbility.OnDashUsed += ReduceDashCooldown;
        GameManager.OnRoundOver += ResetDashCooldownOnRoundEnd;
    }

    private void OnDisable()
    {
        dashAbility.OnDashUsed -= ReduceDashCooldown;
        GameManager.OnRoundOver -= ResetDashCooldownOnRoundEnd;
    }

    private void ReduceDashCooldown()
    {
        if (dashAbility.dash_base_cooldown <= 0.5)
        {
            dashAbility.dash_base_cooldown -= cooldownReductionOnDash;
            cooldownReducedBySeconds += cooldownReductionOnDash;
        }
    }

    private void ResetDashCooldownOnRoundEnd()
    {
        dashAbility.dash_base_cooldown -= cooldownReducedBySeconds;
    }
}
