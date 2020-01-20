using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlatformView : MonoBehaviour
{
    public LayerMask passengerMask;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    public Vector3 move;

    public Collider collider;

    private RaycastController _raycastController;
    private HashSet<Transform> _movedPassengers = new HashSet<Transform>();
    
    // Start is called before the first frame update
    void Awake()
    {
        _raycastController = new RaycastController(horizontalRayCount, verticalRayCount, collider, passengerMask);   
    }

    // Update is called once per frame
    void Update()
    {
        _raycastController.Step();
        Vector3 velocity = move * Time.deltaTime;
        
        _movePassengers(velocity);
        transform.Translate(velocity);

    }

    private void _movePassengers(Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);
        RaycastHit hit;

        // Moving vertically
        if (velocity.y != 0)
        {
            float rayLength = Mathf.Abs(velocity.y) + _raycastController.skinWidth;
            
            for (int i = 0; i < verticalRayCount; ++i)
            {
                Vector3 rayOrigin = (directionY == -1) ? _raycastController.origins.bottomLeft : _raycastController.origins.topLeft;
                rayOrigin += Vector3.right * (_raycastController.verticalRaySpacing * i);
                if (Physics.Raycast(rayOrigin, Vector3.up * directionY, out hit, rayLength, passengerMask))
                {
                    if (!_movedPassengers.Contains(hit.transform))
                    {
                        float pushX = (directionY == 1) ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - _raycastController.skinWidth) * directionY;
                        IPlatformPassenger passenger = hit.transform.gameObject.GetComponent<IPlatformPassenger>();
                        if (passenger != null)
                        {
//                            passenger.requestVelocity = new Vector3(pushX, pushY);
                            hit.transform.Translate(new Vector3(pushX, pushY));
                        }

                        _movedPassengers.Add(hit.transform);
                    }
                }
            }
        }

        //Moving Horizontally
        if (velocity.x != 0)
        {
            float rayLength = Mathf.Abs(velocity.x) + _raycastController.skinWidth;
            for (int i = 0; i < horizontalRayCount; ++i)
            {
                Vector3 rayOrigin = (directionX == -1) ? _raycastController.origins.bottomLeft : _raycastController.origins.bottomRight;
                rayOrigin += Vector3.up * (_raycastController.horizontalRaySpacing * i);
                if (Physics.Raycast(rayOrigin, Vector3.right * directionX, out hit, rayLength, passengerMask))
                {
                    if (!_movedPassengers.Contains(hit.transform))
                    {
                        float pushX = velocity.x - (hit.distance - _raycastController.skinWidth) * directionX;
                        float pushY = 0;
                        IPlatformPassenger passenger = hit.transform.gameObject.GetComponent<IPlatformPassenger>();
                        if (passenger != null)
                        {
//                            passenger.requestVelocity = new Vector3(pushX, pushY);
                            hit.transform.Translate(new Vector3(pushX, pushY));
                        }

                        _movedPassengers.Add(hit.transform);
                    }
                }
            }
        }
        
        if(directionY == -1 || (velocity.y == 0 && velocity.x !=0))
        {
            float rayLength = _raycastController.skinWidth * 2.0f;
            
            for (int i = 0; i < verticalRayCount; ++i)
            {
                Vector3 rayOrigin = _raycastController.origins.topLeft + (_raycastController.verticalRaySpacing * i * Vector3.right);
                if (Physics.Raycast(rayOrigin, Vector3.up, out hit, rayLength, passengerMask))
                {
                    if (!_movedPassengers.Contains(hit.transform))
                    {
                        float pushX = velocity.x;
                        float pushY = velocity.y;
                        IPlatformPassenger passenger = hit.transform.gameObject.GetComponent<IPlatformPassenger>();
                        if (passenger != null)
                        {
//                            passenger.requestVelocity = new Vector3(pushX, pushY);
                            hit.transform.Translate(new Vector3(pushX, pushY));
                        }

                        _movedPassengers.Add(hit.transform);
                    }
                }
            }
        }
        _movedPassengers.Clear();
    }
}
