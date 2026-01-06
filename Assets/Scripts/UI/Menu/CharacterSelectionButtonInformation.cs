using UnityEngine;

public class CharacterSelectionButtonInformation : MonoBehaviour
{
    public string CharacterName
    {
        get => _characterName;
        set => _characterName = value;
    }

    public string CharacterPassive
    {
        get => _characterPassive;
        set => _characterPassive = value;
    }

    public string CharacterActiveCooldown
    {
        get => _characterActiveCooldown;
        set => _characterActiveCooldown = value;
    }

    public string CharacterActiveDuration
    {
        get => _characterActiveDuration;
        set => _characterActiveDuration = value;
    }

    public string CharacterActiveManaCost
    {
        get => _characterActiveManaCost;
        set => _characterActiveManaCost = value;
    }

    public string CharacterActiveDescription
    {
        get => _characterActiveDescription;
        set => _characterActiveDescription = value;
    }

    public string WeaponName
    {
        get => _weaponName;
        set => _weaponName = value;
    }

    public string WeaponDamage
    {
        get => _weaponDamage;
        set => _weaponDamage = value;
    }

    public string WeaponAttackSpeed
    {
        get => _weaponAttackSpeed;
        set => _weaponAttackSpeed = value;
    }

    public string WeaponSpecialAbility
    {
        get => _weaponSpecialAbility;
        set => _weaponSpecialAbility = value;
    }

    private string _characterName;
    private string _characterPassive;
    
    private string _characterActiveCooldown;
    private string _characterActiveDuration;
    private string _characterActiveManaCost;
    private string _characterActiveDescription;
    
    private string _weaponName;
    private string _weaponDamage;
    private string _weaponAttackSpeed;
    private string _weaponSpecialAbility;


    
}
