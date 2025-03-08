using UnityEngine;

[CreateAssetMenu(menuName = "Chess/Lookup Save")]
public class LookupSave : ScriptableObject
{
    public Lookup[] rookLookups;
    public Lookup[] bishopLookups;
}
