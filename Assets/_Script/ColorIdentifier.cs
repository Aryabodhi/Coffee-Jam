using UnityEngine;
using UnityEngine.Serialization;

public class ColorIdentifier : MonoBehaviour
{
    [SerializeField] private ColorId wallColorId;

    public ColorId GetColor(){
        return wallColorId;
    }
}
