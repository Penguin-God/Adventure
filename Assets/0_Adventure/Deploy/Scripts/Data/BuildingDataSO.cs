using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Deploy/BuildingData")]
public class BuildingDataSO : ScriptableObject
{
    public BuildingType buildingType;
    public string buildingName;
    public Sprite sprite;
    public Color color = Color.white;
    
    public float attackDamage;
    public float attackSpeed;
    public float attackRange;
    public int maxAmmo;
    public AmmoType ammoType;
    
    public float ammoProductionTime; 
    public AmmoType producedAmmoType;
    public int connectionRange; 
    
    public float buffAmount;
    public int buffRange;
}
