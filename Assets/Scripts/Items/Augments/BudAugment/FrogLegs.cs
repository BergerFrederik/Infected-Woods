using System;
using UnityEngine;

public class FrogLegs : MonoBehaviour
{
    private const float MinDashCooldown = 0.5f;

    [SerializeField] private float cooldownReductionOnDash; //in seconds e.g: 0.05
    private Transform Player;
    private DashAbility dashAbility;
    private StackCounter _stacks;
    private float cooldownReducedBySeconds;

    private void Start()
    {
        Player = this.transform.root;
        dashAbility = Player.GetComponentInChildren<DashAbility>();
        _stacks = StackCounter.Uncapped(Player.GetComponent<PlayerStats>());

        dashAbility.OnDashUsed += ReduceDashCooldown;
        GameManager.OnRoundOver += ResetDashCooldownOnRoundEnd;
    }

    private void OnDestroy()
    {
        dashAbility.OnDashUsed -= ReduceDashCooldown;
        GameManager.OnRoundOver -= ResetDashCooldownOnRoundEnd;
    }

    private void ReduceDashCooldown()
    {
        // The floor stops the reduction, not the stacks - once it is reached nothing is gained or counted
        if (dashAbility.dash_base_cooldown <= MinDashCooldown) return;

        _stacks.TryAdd(out int wholeStacksGained);

        // A gain can be worth several stacks, so the reduction itself must not undercut the floor
        float reduction = Mathf.Min(cooldownReductionOnDash * wholeStacksGained,
            dashAbility.dash_base_cooldown - MinDashCooldown);
        dashAbility.dash_base_cooldown -= reduction;
        cooldownReducedBySeconds += reduction;
    }

    private void ResetDashCooldownOnRoundEnd()
    {
        dashAbility.dash_base_cooldown += cooldownReducedBySeconds;
        cooldownReducedBySeconds = 0f;
        _stacks.Reset();
    }
}
