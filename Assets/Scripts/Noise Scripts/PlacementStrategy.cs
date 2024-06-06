using UnityEngine;

//Err just me trying to do goofy pattern. U might as well just put vec3 offset directly to the noise handler

[CreateAssetMenu(fileName = "Default placement", menuName = "placement/DefaultPlacement")]
public class PlacementStrategy : ScriptableObject
{
    public virtual Vector3 Setposition(Vector3 worldPosition) => worldPosition;
}