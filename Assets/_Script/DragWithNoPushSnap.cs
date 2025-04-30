using System;
using UnityEngine;
using UnityEngine.Serialization;


public class DragWithNoPushSnap : MonoBehaviour{
    private Camera mainCamera;
    private Rigidbody rb;
    private Collider objectCollider;
    private Vector3 dragOffset;
    private bool isDragging = false;
    private Vector3 targetPosition;
    
    [Header("Drag Settings")]
    [Tooltip("How strongly the object follows the cursor")]
    [SerializeField] private float dragForce = 20f;
    [Tooltip("Maximum drag distance from camera")]
    [SerializeField] private float maxDragDistance = 10f;
    [Tooltip("Distance maintained from camera during drag")]
    [SerializeField] private float dragPlaneDistance = 2f;
    [Tooltip("How quickly the object snaps to grid after release")]
    [SerializeField] private float snapSpeed = 10f;

    public Tray tray;

    void Start()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();
        
        if (rb == null)
        {
            Debug.LogError("No rigidbody attached");
        }
        
        Vector3 startPos = transform.position;
        transform.position = new Vector3(startPos.x, 0f, startPos.z);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isDragging)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit) && (hit.collider == objectCollider || hit.transform.IsChildOf(transform)))
            {
                StartDrag(hit);
            }
        }
        
        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDrag();
        }
        
        
    }

    void StartDrag(RaycastHit hit)
    {
        StopAllCoroutines();
        rb.isKinematic = false;
        isDragging = true;
        
        dragOffset = transform.position - hit.point;
        
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    private void FixedUpdate(){
        if (isDragging)
        {
            UpdateDrag();
        }
    }

    void UpdateDrag()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        
        Plane horizontalPlane = new Plane(Vector3.up, Vector3.zero);

        if (horizontalPlane.Raycast(ray, out var distance))
        {
            Vector3 dragPoint = ray.GetPoint(distance);
            
            targetPosition = new Vector3(
                dragPoint.x + dragOffset.x,
                0f,  
                dragPoint.z + dragOffset.z
            );
            
            Vector3 force = new Vector3(
                (targetPosition.x - rb.position.x) * dragForce,
                0f,  
                (targetPosition.z - rb.position.z) * dragForce
            );
            
            rb.linearVelocity = force;
            
        }
    }

    void EndDrag()
    {
        isDragging = false;
        
        gameObject.layer = 0;
        
        rb.linearVelocity = Vector3.zero;
        
        Vector3 snappedPosition = new Vector3(
            Mathf.Round(transform.position.x),
            0f,  
            Mathf.Round(transform.position.z)
        );
        
        StartCoroutine(SnapToGrid(snappedPosition));
    }

    System.Collections.IEnumerator SnapToGrid(Vector3 snappedPosition)
    {
        
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;
        
        rb.isKinematic = true;
        
        while (elapsedTime < 0.2f)
        {
            Vector3 newPos = Vector3.Lerp(startingPosition, snappedPosition, elapsedTime * snapSpeed);
            transform.position = new Vector3(newPos.x, 0f, newPos.z);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = snappedPosition;
       
    }

    public Tray GetTray(){
        return tray;
    }
  
}