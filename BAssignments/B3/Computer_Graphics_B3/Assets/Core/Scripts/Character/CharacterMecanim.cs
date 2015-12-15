using UnityEngine;
using TreeSharpPlus;
using System;
using System.Collections;
using System.Collections.Generic;

using RootMotion.FinalIK;

public class CharacterMecanim : MonoBehaviour
{
    public const float MAX_REACHING_DISTANCE = 1.1f;
    public const float MAX_REACHING_HEIGHT = 2.0f;
    public const float MAX_REACHING_ANGLE = 100;

    private Dictionary<FullBodyBipedEffector, bool> triggers;
    private Dictionary<FullBodyBipedEffector, bool> finish;

    [HideInInspector]
    public BodyMecanim Body = null;

    void Awake() { this.Initialize(); }

    /// <summary>
    /// Searches for and binds a reference to the Body interface
    /// </summary>
    public void Initialize()
    {
        this.Body = this.GetComponent<BodyMecanim>();
        this.Body.InteractionTrigger += this.OnInteractionTrigger;
        this.Body.InteractionStop += this.OnInteractionFinish;
    }

    private void OnInteractionTrigger(
        FullBodyBipedEffector effector, 
        InteractionObject obj)
    {
        if (this.triggers == null)
            this.triggers = new Dictionary<FullBodyBipedEffector, bool>();
        if (this.triggers.ContainsKey(effector))
            this.triggers[effector] = true;
    }

    private void OnInteractionFinish(
        FullBodyBipedEffector effector,
        InteractionObject obj)
    {
        if (this.finish == null)
            this.finish = new Dictionary<FullBodyBipedEffector, bool>();
        if (this.finish.ContainsKey(effector))
            this.finish[effector] = true;
    }

    #region Smart Object Specific Commands
    public virtual RunStatus WithinDistance(Vector3 target, float distance)
    {
        if ((transform.position - target).magnitude < distance)
            return RunStatus.Success;
        return RunStatus.Failure;
    }

    public virtual RunStatus Approach(Vector3 target, float distance)
    {
        Vector3 delta = target - transform.position;
        Vector3 offset = delta.normalized * distance;
        return this.NavGoTo(target - offset);
    }
    #endregion

    #region Navigation Commands

    public virtual RunStatus NavTurnShop()
    {
        ShopperMeta sm = this.gameObject.GetComponent<ShopperMeta>();
        return NavTurn(Val.V(() => sm.salesman.transform.position));
    }

    /// <summary>
    /// Turns to face a desired target point
    /// </summary>
    public virtual RunStatus NavTurn(Val<Vector3> target)
    {
        this.Body.NavSetOrientationBehavior(OrientationBehavior.None);
        this.Body.NavSetDesiredOrientation(target.Value);
        if (this.Body.NavIsFacingDesired() == true)
        {
            this.Body.NavSetOrientationBehavior(
                OrientationBehavior.LookForward);
            return RunStatus.Success;
        }
        return RunStatus.Running;
    }

    /// <summary>
    /// Turns to face a desired orientation
    /// </summary>
    public virtual RunStatus NavTurn(Val<Quaternion> target)
    {
        this.Body.NavSetOrientationBehavior(OrientationBehavior.None);
        this.Body.NavSetDesiredOrientation(target.Value);
        if (this.Body.NavIsFacingDesired() == true)
        {
            this.Body.NavFacingSnap();
            this.Body.NavSetOrientationBehavior(
                OrientationBehavior.LookForward);
            return RunStatus.Success;
        }
        return RunStatus.Running;
    }

    /// <summary>
    /// Sets a custom orientation behavior
    /// </summary>
    public virtual RunStatus NavOrientBehavior(
        Val<OrientationBehavior> behavior)
    {
        this.Body.NavSetOrientationBehavior(behavior.Value);
        return RunStatus.Success;
    }

    public virtual RunStatus ChooseNextShop() 
    {
        
        GameObject[] openStalls = GameObject.FindGameObjectsWithTag("PurchaseEmpty");
        if (openStalls.Length > 0)
        {
            int index = UnityEngine.Random.Range(0, openStalls.Length - 1);
            ShopperMeta sm = this.gameObject.GetComponent<ShopperMeta>();
            sm.stallSpot = openStalls[index];
            sm.stallSpot.tag = "PurchaseOccupied";
            sm.salesman = openStalls[index].transform.parent.FindChild("Tom").gameObject;
            return RunStatus.Success;
        }
        else
        {
            return RunStatus.Running;
        }
    }

    public virtual RunStatus ChooseTheftTarget()
    {
        Transform stall = this.gameObject.GetComponent<ShopperMeta>().stallSpot.transform.parent;
        int lampIndex = UnityEngine.Random.Range(1, 3);
        GameObject lamp = stall.FindChild("Lamps").FindChild("L" + lampIndex).gameObject;
        this.gameObject.GetComponent<ThiefMeta>().lamp = lamp;
        return RunStatus.Success;
    }

    public virtual RunStatus NavGoToNextShop()
    {
            ShopperMeta sm = this.gameObject.GetComponent<ShopperMeta>();
            return NavGoTo(Val.V(() => sm.stallSpot.transform.position));
    }


    public virtual RunStatus NavLeaveShop()
    {
        ShopperMeta sm = this.gameObject.GetComponent<ShopperMeta>();
        sm.stallSpot.tag = "PurchaseEmpty";
        Vector3 pos = sm.stallSpot.transform.position;
        pos.z -= 2;
        return NavTurn(Val.V(() => pos));
    }

    public virtual RunStatus NavEscape()
    {
        GameObject[] escapePoints = GameObject.FindGameObjectsWithTag("ThiefEscapePoint");
        int minInd = 0;
        float min = 0;
        for (int i = 0; i < escapePoints.Length; i++)
        {
            float dist = (escapePoints[i].transform.position - transform.position).sqrMagnitude;
            if(dist < min || i == 0)
            {
                minInd = i;
                min = dist;
            }
        }
        return NavGoTo(escapePoints[minInd].transform.position);
    }

    public virtual RunStatus NavChase(Val<GameObject> target)
    {
        this.gameObject.GetComponent<UnitySteeringController>().maxSpeed = 6.0f;
        if(this.Body.NavCanReach(target.Value.transform.position) == false)
        {
            return RunStatus.Failure;
        }
        this.Body.NavGoTo(target.Value.transform.position);

        float sqdist = (target.Value.transform.position - this.transform.position).sqrMagnitude;

        if (sqdist < 16.0f)
        {
            this.Body.NavStop();
            return RunStatus.Success;
        }
        return RunStatus.Running;
    }


    /// <summary>
    /// Sets a new navigation target. Will fail immediately if the
    /// point is unreachable. Blocks until the agent arrives.
    /// </summary>
    public virtual RunStatus NavGoTo(Val<Vector3> target)
    {
        if (this.Body.NavCanReach(target.Value) == false)
        {
            Debug.LogWarning("NavGoTo failed -- can't reach target");
            return RunStatus.Failure;
        }
        // TODO: I previously had this if statement here to prevent spam:
        //     if (this.Interface.NavTarget() != target)
        // It's good for limiting the amount of SetDestination() calls we
        // make internally, but sometimes it causes the character1 to stand
        // still when we re-activate a tree after it's been terminated. Look
        // into a better way to make this smarter without false positives. - AS
        this.Body.NavGoTo(target.Value);
        if (this.Body.NavHasArrived() == true)
        {
            this.Body.NavStop();
            return RunStatus.Success;
        }
        return RunStatus.Running;
        // TODO: Timeout? - AS
    }

    /// <summary>
    /// Lerps the character towards a target. Use for precise adjustments.
    /// </summary>
    public virtual RunStatus NavNudgeTo(Val<Vector3> target)
    {
        bool? result = this.Body.NavDoneNudge();
        if (result == null)
        {
            this.Body.NavNudge(target.Value, 0.3f);
        }
        else if (result == true)
        {
            this.Body.NavNudgeStop();
            return RunStatus.Success;
        }
        return RunStatus.Running;
    }

    private IEnumerator<RunStatus> snapper;

    private RunStatus TickSnap(
        Vector3 position,
        Vector3 target,
        float time = 1.0f)
    {
        if (this.snapper == null)
            this.snapper = 
                SnapToTarget(position, target, time).GetEnumerator();
        if (this.snapper.MoveNext() == false)
        {
            this.snapper = null;
            return RunStatus.Success;
        }
        return snapper.Current;
    }

    private IEnumerable<RunStatus> SnapToTarget(
        Vector3 position,
        Vector3 target,
        float time)
    {
        Interpolator<Vector3> interp =
            new Interpolator<Vector3>(
                position,
                target,
                Vector3.Lerp);
        interp.ForceMin();
        interp.ToMax(time);

        while (interp.State != InterpolationState.Max)
        {
            transform.position = interp.Value;
            yield return RunStatus.Running;
        }
        yield return RunStatus.Success;
        yield break;
    }

	/// <summary>
	/// Stops the Navigation system. Blocks until the agent is stopped.
	/// </summary>
	public virtual RunStatus NavStop()
    {
        this.Body.NavStop();
        if (this.Body.NavIsStopped() == true)
            return RunStatus.Success;
        return RunStatus.Running;
        // TODO: Timeout? - AS
    }
    #endregion

    #region Interaction Commands
    public virtual RunStatus WaitForTrigger(
        Val<FullBodyBipedEffector> effector)
    {
        if (this.triggers == null)
            this.triggers = new Dictionary<FullBodyBipedEffector, bool>();
        if (this.triggers.ContainsKey(effector.Value) == false)
            this.triggers.Add(effector.Value, false);
        if (this.triggers[effector.Value] == true)
        {
            this.triggers.Remove(effector.Value);
            return RunStatus.Success;
        }
        return RunStatus.Running;
    }

    public virtual RunStatus WaitForFinish(
        Val<FullBodyBipedEffector> effector)
    {
        if (this.finish == null)
            this.finish = new Dictionary<FullBodyBipedEffector, bool>();
        if (this.finish.ContainsKey(effector.Value) == false)
            this.finish.Add(effector.Value, false);
        if (this.finish[effector.Value] == true)
        {
            this.finish.Remove(effector.Value);
            return RunStatus.Success;
        }
        return RunStatus.Running;
    }

    public virtual RunStatus StartInteraction(
        Val<FullBodyBipedEffector> effector, 
        Val<InteractionObject> obj)
    {
        this.Body.StartInteraction(effector, obj);
        return RunStatus.Success;
    }

    public virtual RunStatus ResumeInteraction(
        Val<FullBodyBipedEffector> effector)
    {
        this.Body.ResumeInteraction(effector);
        return RunStatus.Success;
    }

    public virtual RunStatus StopInteraction(Val<FullBodyBipedEffector> effector)
    {
        this.Body.StopInteraction(effector);
        return RunStatus.Success;
    }	
    #endregion

    #region HeadLook Commands
    public virtual RunStatus HeadLookAt(Val<Vector3> target)
    {
        this.Body.HeadLookAt(target);
        return RunStatus.Success;
    }

    public virtual RunStatus HeadLookAtBoth(Val<Vector3> eyeHeight)
    {
        GameObject salesman = this.gameObject.GetComponent<ShopperMeta>().salesman;
        BodyMecanim other = salesman.GetComponent<BodyMecanim>();
        Val<Vector3> targA = Val.V(() => salesman.transform.position + eyeHeight.Value);
        Val<Vector3> targB = Val.V(() => this.transform.position + eyeHeight.Value);
        this.Body.HeadLookAt(targA);
        other.HeadLookAt(targB);
        return RunStatus.Success;
    }

    public virtual RunStatus HeadLookStop()
    {
        this.Body.HeadLookStop();
		return RunStatus.Success;
	}

    public virtual RunStatus HeadLookStopBoth()
    {
        this.Body.HeadLookStop();
        this.gameObject.GetComponent<ShopperMeta>().salesman.GetComponent<BodyMecanim>().HeadLookStop();
        return RunStatus.Success;
    }

    public virtual RunStatus DisplaySpeechBubble(string message, long duration)
    {
        GetComponent<SpeechBubbleController>().DisplaySpeechBubble(message, duration);
        return RunStatus.Success;
    }
    public virtual RunStatus DisplaySpeechBubbleOther(string message, long duration)
    {
        GameObject salesman = this.gameObject.GetComponent<ShopperMeta>().salesman;
        salesman.GetComponent<SpeechBubbleController>().DisplaySpeechBubble(message, duration);
        return RunStatus.Success;
    }

    public virtual RunStatus PointAt(Transform pointTargetTransform, bool useRightHand)
    {
        GetComponent<IKController>().PointAt(transform, pointTargetTransform, false);
        return RunStatus.Success;
    }

    public virtual RunStatus FaceLamp()
    {
        return NavTurn(Val.V(() => GetComponent<ThiefMeta>().lamp.transform.position));
    }

    public virtual RunStatus PointAtLamp()
    {
        return PointAt(gameObject.GetComponent<ThiefMeta>().lamp.transform, true);
    }

    public virtual RunStatus PullLamp()
    {
        GameObject lamp = this.gameObject.GetComponent<ThiefMeta>().lamp.gameObject;
        LampController lc = lamp.AddComponent<LampController>();
        lc.thief = this.gameObject;
        lc.primary = lamp.transform.position;
        this.gameObject.GetComponent<UnitySteeringController>().maxSpeed = 8.0f;
        return RunStatus.Success;
    }

    public virtual RunStatus DropAll()
    {
        GameObject lamp = this.gameObject.GetComponent<ThiefMeta>().lamp.gameObject;
        LampController lc = lamp.GetComponent<LampController>();
        lc.enabled = false;

        // Make the lamp fall down
        Rigidbody rb = lamp.AddComponent<Rigidbody>();
        rb.useGravity = true;
        BoxCollider bc = lamp.AddComponent<BoxCollider>();

        this.gameObject.GetComponent<UnitySteeringController>().maxSpeed = 8.5f;
        return RunStatus.Success;
    }

    #endregion

    #region Animation Commands
    public virtual RunStatus FaceAnimation(
        Val<string> gestureName, Val<bool> isActive)
    {
        this.Body.FaceAnimation(gestureName.Value, isActive.Value);
		return RunStatus.Success;
	}
	
	public virtual RunStatus HandAnimation(
        Val<string> gestureName, Val<bool> isActive)
    {
        this.Body.HandAnimation(gestureName.Value, isActive.Value);
		return RunStatus.Success;
	}
    public virtual RunStatus HandAnimationOther(
        Val<string> gestureName, Val<bool> isActive)
    {
        GameObject salesman = this.gameObject.GetComponent<ShopperMeta>().salesman;
        salesman.GetComponent<BodyMecanim>().HandAnimation(gestureName.Value, isActive.Value);
        return RunStatus.Success;
    }

	public virtual RunStatus BodyAnimation(Val<string> gestureName, Val<bool> isActive)
	{
		this.Body.BodyAnimation(gestureName.Value, isActive.Value);
		return RunStatus.Success;
	}

	public RunStatus ResetAnimation()
    {
        this.Body.ResetAnimation();
        return RunStatus.Success;
    }
    #endregion

    #region Sitting Commands
    /// <summary>
    /// Sits the character down
    /// </summary>
    public virtual RunStatus SitDown()
    {
        if (this.Body.IsSitting() == true)
            return RunStatus.Success;
        this.Body.SitDown();
        return RunStatus.Running;
    }

    /// <summary>
    /// Stands the character up
    /// </summary>
    public virtual RunStatus StandUp()
    {
        if (this.Body.IsStanding() == true)
            return RunStatus.Success;
        this.Body.StandUp();
        return RunStatus.Running;
    }
    #endregion
}
