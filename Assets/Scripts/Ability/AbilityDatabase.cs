using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AbilityDatabase", menuName = "Abilities/Database")]
public class AbilityDatabase : ScriptableObject
{
    public List<Ability> abilitiesList;

    public Ability GetAbility(string name)
    {
        return abilitiesList.Find(a => a.abilityName == name);
    }

    public Ability GetByType(AbilityType type)
    {
        int index = (int)type - 1;
        return (index >= 0 && index < abilitiesList.Count) ? abilitiesList[index] : null;
    }
}
