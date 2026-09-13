using UnityEngine;

public abstract class WeaponAbility : MonoBehaviour
{
    [Header("Ability")]
    [SerializeField] protected float abilityCooldown = 5f;
    [SerializeField] protected WeaponStats weaponStats;

    protected Transform playerTransform;
    protected PlayerStats playerStats;

    private GameInput gameInput;
    private float remainingCooldown;
    private bool abilityReady = true;

    protected virtual void Awake()
    {
        gameInput = FindAnyObjectByType<GameInput>();
    }

    protected virtual void OnEnable()
    {
        gameInput.OnWeaponAbilitySlotStarted += OnWeaponAbilitySlotStarted;
    }

    protected virtual void OnDisable()
    {
        if (gameInput != null)
        {
            gameInput.OnWeaponAbilitySlotStarted -= OnWeaponAbilitySlotStarted;
        }
    }

    protected virtual void Update()
    {
        if (playerStats == null)
        {
            playerTransform = transform.root;
            playerStats = playerTransform.GetComponentInChildren<PlayerStats>();
            return;
        }

        if (!abilityReady)
        {
            remainingCooldown -= Time.deltaTime;
            if (remainingCooldown <= 0f)
            {
                abilityReady = true;
            }
        }
    }

    private void OnWeaponAbilitySlotStarted(int slotIndex)
    {
        if (slotIndex != GetWeaponSlotIndex()) return;
        TryActivate();
    }

    private int GetWeaponSlotIndex()
    {
        // The weapon prefab is instantiated as a child of its "WeaponAnkerN" slot object
        // under PlayerWeaponSlots, so the parent's sibling index is the weapon's slot number.
        return transform.parent != null ? transform.parent.GetSiblingIndex() : -1;
    }

    private void TryActivate()
    {
        if (!abilityReady || playerStats == null) return;

        float reducedCooldown = abilityCooldown * (1f - playerStats.playerCooldown / 100f);
        remainingCooldown = Mathf.Clamp(reducedCooldown, 0.01f, abilityCooldown);
        abilityReady = false;

        Activate();
    }

    protected abstract void Activate();
}
