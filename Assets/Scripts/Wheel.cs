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
        if (Physics.Linecast(suspensionJoint.position, suspensionJoint.position -(transform.up * ((radius*2)+(displacementRange))), out hit))
        {
            // Debug.DrawLine(suspensionJoint.position, hit.point, Color.red);
            hitPoint = hit.point.y;
            // transform.position = new Vector3(suspensionJoint.position.x, hitPoint + (radius), suspensionJoint.position.z);
            
            rayDist = suspensionJoint.position.y - hitPoint;
            transform.position = suspensionJoint.position - (suspensionJoint.up *(rayDist - (radius)));
            CurDisplacement = restPositionOffset - rayDist;
            velocity = Vector3.Dot(transform.up, myCar.rb.GetPointVelocity(transform.position));
            force = (CurDisplacement * springConstant) - (velocity * dampningFactor);
            if(force < 0){force=0;}
            myCar.rb.AddForceAtPosition(transform.up * force, transform.position);
            Debug.DrawLine(suspensionJoint.position, transform.position + (transform.up * force), Color.red);

            grounded = true;
            Friction();
        }
        else
        {
            Vector3 newPosition = suspensionJoint.position - (suspensionJoint.up * ((radius)+(displacementRange)));
            transform.position = Vector3.Lerp(transform.position, newPosition, 0.2f);
            // new Vector3(transform.localPosition.x, botMaxDisplacement, transform.localPosition.z),0.2f);
        }

        // transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + velocity, transform.localPosition.z);        
    }
    public void Rotate(float input){
        if(grounded ){
            Vector3 accelDir = transform.forward;            
            if(input ==0){
                //Decelerate
                Vector3 force = accelDir * -myCar.curSpeed/myCar.Deceleration;
                myCar.rb.AddForceAtPosition(force, suspensionJoint.position);
                Debug.DrawLine(suspensionJoint.position, suspensionJoint.position + force, Color.red);
            }else{
                float normalizedSpeed= Mathf.Clamp01(Mathf.Abs(myCar.curSpeed) / myCar.TopSpeed);
                float availableTorque = myCar.PowerCurve.Evaluate(normalizedSpeed) * input * myCar.EnginePower;
                Vector3 force = accelDir * availableTorque;
                myCar.rb.AddForceAtPosition(force, suspensionJoint.position);
                Debug.DrawLine(suspensionJoint.position, suspensionJoint.position + force, Color.blue);
            }
        }
    }

    public void Brake(){
        if(!grounded){return;}
            Vector3 accelDir = transform.forward;            

        Vector3 force = accelDir * -myCar.curSpeed/myCar.BrakePower;
                myCar.rb.AddForceAtPosition(force, suspensionJoint.position);
                Debug.DrawLine(suspensionJoint.position, suspensionJoint.position + force, Color.red);
    }

    void Friction(){
        if(grounded){
            Vector3 steeringDir = transform.right;
            Vector3 tireWorldVel = myCar.rb.GetPointVelocity(suspensionJoint.position);
            float steeringVel = Vector3.Dot(steeringDir, tireWorldVel);
            float desiredVelChange = -steeringVel * tireGripFactor;
            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
            Vector3 friction = steeringDir * tireMass * desiredAccel;
            // friction = new Vector3(friction.x, 0, friction.z);
            Debug.DrawLine(suspensionJoint.position, suspensionJoint.position + friction, Color.blue);

            myCar.rb.AddForceAtPosition(friction, suspensionJoint.position);
        }
    }

    public void Steer(float input){
        float carSpeed = Mathf.Clamp01(Vector3.Dot(myCar.transform.forward, myCar.rb.velocity) / myCar.TopSpeed);
        
        float newSteerAngle = myCar.maxSteerAngle - ((myCar.maxSteerAngle - myCar.minSteerAngle) * carSpeed);
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, input * newSteerAngle, transform.localEulerAngles.z);
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
