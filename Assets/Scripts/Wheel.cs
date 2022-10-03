using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wheel : MonoBehaviour
{
    public Transform suspensionJoint;
    public float radius = 0.35f;
    public float restPosition;
    public float restPositionOffset;
    public float springConstant = 1f;
    public float tireGripFactor =1f;
    public float tireMass = 1f;
    public float steerAngle = 25;
    public float dampningFactor = 1f;
    public float velocity;
    // public ForceMode springForceMode;
    public float displacementRange = 0.1f;
    public float hitPoint = 0;
    public float CurDisplacement;
    public float curDisplacement => restPosition - transform.localPosition.y;
    public float topMaxDisplacement => restPosition + displacementRange;
    public float botMaxDisplacement => restPosition - displacementRange;
    public float bottomEnd => transform.position.y - radius;
    LayerMask groundMask;

    public CarController myCar;

    void Awake()
    {
        restPosition = transform.localPosition.y;
        // restPositionOffset = suspensionJoint.position.y - transform.position.y;
        groundMask = LayerMask.GetMask("Ground");
    }
    public float rayDist ;
    // Update is called once per frame
    bool grounded = false;
    void FixedUpdate()
    {
        if (myCar == null) { return; }

        RaycastHit hit = new RaycastHit();
        float newDisplacement = transform.localPosition.y;
        
        Vector3 topEnd = transform.position + new Vector3(0, radius, 0);

        grounded=false;
        if (Physics.Linecast(suspensionJoint.position, suspensionJoint.position -(transform.up * ((radius*2)+(displacementRange))), out hit, groundMask))
        {
            Debug.DrawLine(suspensionJoint.position, hit.point, Color.red);
            hitPoint = hit.point.y;
            transform.position = new Vector3(suspensionJoint.position.x, hitPoint + (radius), suspensionJoint.position.z);
            rayDist = suspensionJoint.position.y - hitPoint;
            CurDisplacement = restPositionOffset - rayDist;
            velocity = Vector3.Dot(transform.up, myCar.rb.GetPointVelocity(transform.position));
            force = (CurDisplacement * springConstant) - (velocity * dampningFactor);
            myCar.rb.AddForceAtPosition(transform.up * force, transform.position);
            grounded = true;
            Friction();
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, 
            new Vector3(transform.localPosition.x, botMaxDisplacement, transform.localPosition.z),0.2f);
        }

        // transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + velocity, transform.localPosition.z);        
    }

    public void Rotate(float input){
        if(grounded ){
            if(input >0){

            }
            Vector3 accelDir = transform.forward;

            float carSpeed = Vector3.Dot(myCar.transform.forward, myCar.rb.velocity);
            float normalizedSpeed= Mathf.Clamp01(Mathf.Abs(carSpeed) / myCar.TopSpeed);
            float availableTorque = myCar.PowerCurve.Evaluate(normalizedSpeed) * input * myCar.EnginePower;
            
            myCar.rb.AddForceAtPosition(accelDir * availableTorque, transform.position);
        }
    }

    void Friction(){
        if(grounded){
            Vector3 steeringDir = transform.right;
            Vector3 tireWorldVel = myCar.rb.GetPointVelocity(transform.position);
            float steeringVel = Vector3.Dot(steeringDir, tireWorldVel);
            float desiredVelChange = -steeringVel * tireGripFactor;
            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
            Vector3 friction = steeringDir * tireMass * desiredAccel;
            // friction = new Vector3(friction.x, 0, friction.z);
            myCar.rb.AddForceAtPosition(friction, transform.position);
        }
    }

    public void Steer(float input){
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, input * steerAngle, transform.localEulerAngles.z);
    }

    public float force;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        int segmants = 12;

        Vector3 lastPoint = Vector3.zero;
        for (int i = 0; i < segmants; i++)
        {
            float theta = ((Mathf.PI * 2) / segmants) * i;
            float z = transform.position.z + (radius * Mathf.Sin(theta));
            float y = transform.position.y + (radius * Mathf.Cos(theta));
            Vector3 thisPoint = new Vector3(transform.position.x, y, z);


            if (i > 0) { Gizmos.DrawLine(lastPoint, thisPoint); }
            lastPoint = thisPoint;
        }

        Gizmos.DrawWireSphere(new Vector3(transform.position.x, hitPoint, transform.position.z), 0.2f);
    }
}
