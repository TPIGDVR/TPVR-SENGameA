using UnityEngine;

[CreateAssetMenu(fileName = "positionOffset placement", menuName = "placement/Position Placement")]
public class PositionOffsetPlacement : PlacementStrategy { 
    public Vector3 offset;
    public override Vector3 Setposition(Vector3 position) => position + offset;
}