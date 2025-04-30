using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class Tray : MonoBehaviour{
    [System.Serializable]
    public class DirectionCastSettings{
        public string name;
        public Vector3 direction;
        public Vector3 offset = Vector3.zero;
        public Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);
        public float castDistance = 1f;
    }

    [System.Serializable]
    public class Slot{
        public Transform transform;
        public bool isFilled;
    }

    [SerializeField] private ColorId color;
    [SerializeField] private DragWithNoPushSnap dragWithNoPush;

    [Header("Layer and Debug")] public LayerMask layerMask;
    public bool drawGizmos = true;
    public UnityEngine.Color gizmoColor = UnityEngine.Color.green;

    [Header("BoxCast Settings Per Direction")]
    public DirectionCastSettings[] casts = new DirectionCastSettings[]{
        new DirectionCastSettings{ name = "Forward", direction = Vector3.forward },
        new DirectionCastSettings{ name = "Back", direction = Vector3.back },
        new DirectionCastSettings{ name = "Right", direction = Vector3.right },
        new DirectionCastSettings{ name = "Left", direction = Vector3.left }
    };

    [Header("Slots")] public List<Slot> slots;

    public bool CheckForWalls(){
        foreach (var cast in casts){
            Vector3 origin = transform.position + cast.offset;
            RaycastHit[] hits = Physics.BoxCastAll(origin, cast.halfExtents,
                transform.TransformDirection(cast.direction), transform.rotation, cast.castDistance, layerMask);
            bool canPass = false;
            foreach (var hit in hits){
                if (!hit.transform.IsChildOf(transform)){
                    Debug.Log($"[{cast.name}] Hit {hit.collider.name}");
                    if (hit.transform.TryGetComponent(out ColorIdentifier wall)){
                        canPass = wall.GetColor() == color;
                    }
                    else{
                        return false;
                    }
                }
            }

            if (canPass){
                if (Counter.Instance.TryAddTray(this, out Vector3 slotPos)){
                    transform.position = slotPos;
                    dragWithNoPush.enabled = false;
                }

                return canPass;
            }
        }

        return false;
    }

    private void OnDrawGizmos(){
        if (!drawGizmos || casts == null) return;

        Gizmos.color = gizmoColor;

        foreach (var cast in casts){
            Vector3 origin = transform.position + cast.offset;
            Vector3 center =
                origin + transform.TransformDirection(cast.direction.normalized) * (cast.castDistance / 2f);
            Quaternion rot = transform.rotation;

            Matrix4x4 matrix = Matrix4x4.TRS(center, rot, Vector3.one);
            Gizmos.matrix = matrix;
            Gizmos.DrawWireCube(Vector3.zero, cast.halfExtents * 2f);
        }
    }

    public ColorId GetColor(){
        return color;
    }

    public void AddMover(SplineItemMover mover){
        foreach (Slot slot in slots){
            if (!slot.isFilled){
                mover.transform.position = slot.transform.position;
                mover.transform.parent = slot.transform;
                slot.isFilled = true;
                break;
            }
        }

        if (slots.All(slot => slot.isFilled)){
            Destroy(gameObject);
        }
    }
}