using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "ScriptableObjects/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Allgemeine Infos")]
    public string weaponName;
    public string weaponSubtitle;
    public float weaponPrice;
    public int weaponTier; 
    public string weaponSpecialAbility;
    public string weaponLore;
    public enum weaponTypeOptions
    {
        Melee,
        Ranged,
        Ability
    }
    public weaponTypeOptions weaponWeaponType;
    public string weaponAttackType;
    public float weaponClass = 0f;

    [Header("Level-Spezifische Stats")]
    public LevelStats[] levels; 

    [System.Serializable]
    public class LevelStats
    {
        public float weaponProjectileSpeed = 0f;
        public float weaponBaseDamage = 0f;
        public float weaponMeeleDamageScale = 0f;
        public float weaponRangedDamageScale = 0f;
        public float weaponMysticDamageScale = 0f;
        public float weaponAttackSpeedCooldown = 0f; // in seconds
        public float weaponCritChance = 0f;
        public float weaponCritDamage = 0f;
        public float weaponRange = 0f;
        public float weaponKnockback = 0f;
        public float weaponLifesteal = 0f;
    }
}