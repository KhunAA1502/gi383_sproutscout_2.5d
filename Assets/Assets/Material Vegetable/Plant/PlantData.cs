using UnityEngine;

[CreateAssetMenu(fileName = "NewPlantData", menuName = "Farming/PlantData")]
public class PlantData : ScriptableObject 
{
    public string plantName;
    public GameObject seedModel;     // สำหรับเก็บ Model ขั้นที่ 1
    public GameObject seedlingModel; // สำหรับเก็บ Model ขั้นที่ 2
    public GameObject matureModel;   // สำหรับเก็บ Model ขั้นที่ 3
    public ItemData harvestItem;     // ไอเทมที่จะได้รับเมื่อเก็บเกี่ยว
}