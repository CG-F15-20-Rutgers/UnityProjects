using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using RootMotion.FinalIK;

public class IKController : MonoBehaviour 
{
    /// <summary>
    /// Controls IK for head look
    /// </summary>
    private class LookAtIKController
    {
        private CrossfadeLookAtIK lookAt;

        private Vector3 oldTarget;
        private Transform curTarget;

        private Interpolator<Vector3> targetInterp;
        private Interpolator<float> weightInterp;
        private Interpolator<float> controlInterp;

        private float weight; // Current weight

        public LookAtIKController(
            CrossfadeLookAtIK lookAt,
            float bodyWeightMax,
            float bodyWeightDelay)
        {
            this.lookAt = lookAt;

            this.targetInterp =
                new Interpolator<Vector3>(
                    Vector3.zero, Vector3.zero, Vector3.Lerp);

            this.weightInterp =
                new Interpolator<float>(
                    0.0f, 1.0f, Mathf.Lerp);

            this.controlInterp =
                new Interpolator<float>(
                    0.0f, bodyWeightMax, Mathf.Lerp);

            this.controlInterp.ForceMax();
        }

        public void LookAt(Vector3 target, float delay)
        {
            this.weightInterp.ToMax(delay);

            this.targetInterp.SetValues(
                this.lookAt.solver.IKPosition,
                target);
            this.targetInterp.ForceMin();
            this.targetInterp.ToMax(delay);
        }

        public void LookStop(float delay)
        {
            this.weightInterp.ToMin(delay);
        }

        public void Update()
        {
            this.lookAt.solver.IKPositionWeight =
                this.weightInterp.Value;
            this.lookAt.solver.IKPosition = 
                this.targetInterp.Value;
            this.lookAt.solver.bodyWeight =
                this.controlInterp.Value;
        }

        public void LateUpdate()
        {
            this.lookAt.GetIKSolver().Update();
        }

        public void FullBody(float delay)
        {
            this.controlInterp.ToMax(delay);
        }

        public void HeadOnly(float delay)
        {
            this.controlInterp.ToMin(delay);
        }

        public bool IsFullBody()
        {
            return this.controlInterp.State == InterpolationState.Max;
        }
    }

    enum PrayingIKState
    {
        INACTIVE,
        STARTING,
        PRAYING,
        RETRACTING
    }

    /// <summary>
    /// Controls IK for praying
    /// </summary>
    private class PrayingIKController
    {
        CrossfadeFBBIK ikSecondary;
        PrayingIKState state;

        float currAnimTime;
        float maxTime;

        public PrayingIKController(CrossfadeFBBIK[] iks)
        {
            ikSecondary = iks[1];
            state = PrayingIKState.INACTIVE;
            maxTime = 0.4f;
        }

        public void StartPrayer(Transform character, Transform egg)
        {
            if (state == PrayingIKState.INACTIVE)
            {
                ObjectController oc = egg.parent.GetComponent<ObjectController>();
                if (oc) oc.StartPrayer();

                state = PrayingIKState.STARTING;
                Quaternion rotation = Quaternion.LookRotation(egg.position - character.position);
                Vector3 target = egg.position - (rotation * new Vector3(0, 0, 1.5f));
                ikSecondary.solver.leftHandEffector.position = target - new Vector3(0.2f, 0, 0);
                ikSecondary.solver.rightHandEffector.position = target + new Vector3(0.2f, 0, 0);
            }
        }

        public void EndPrayer(Transform egg)
        {
            if (state != PrayingIKState.INACTIVE)
            {
                ObjectController oc = egg.parent.GetComponent<ObjectController>();
                if (oc) oc.EndPrayer();

                state = PrayingIKState.RETRACTING;
            }
        }

        public void Update()
        {
            switch (state)
            {
                case PrayingIKState.INACTIVE:
                    break;
                case PrayingIKState.STARTING:
                    currAnimTime += Time.deltaTime;
                    float weight;
                    if (currAnimTime > maxTime)
                    {
                        currAnimTime = maxTime;
                        weight = 1;
                        state = PrayingIKState.PRAYING;
                    }
                    else
                    {
                        float normTime = currAnimTime / maxTime;
                        weight = Mathf.Sin(normTime * Mathf.PI / 2);
                    }
                    ikSecondary.solver.leftHandEffector.positionWeight = weight;
                    ikSecondary.solver.rightHandEffector.positionWeight = weight;
                    break;
                case PrayingIKState.PRAYING:
                    break;
                case PrayingIKState.RETRACTING:
                    currAnimTime -= Time.deltaTime;
                    if (currAnimTime < 0)
                    {
                        currAnimTime = 0;
                        weight = 0;
                        state = PrayingIKState.INACTIVE;
                    }
                    else
                    {
                        float normTime = currAnimTime / maxTime;
                        weight = Mathf.Sin(normTime * Mathf.PI / 2);
                    }
                    ikSecondary.solver.leftHandEffector.positionWeight = weight;
                    ikSecondary.solver.rightHandEffector.positionWeight = weight;
                    break;
            }
        }

        public bool IsActive()
        {
            return state != PrayingIKState.INACTIVE;
        }
    }

    enum LifeState
    {
        ALIVE,
        DYING,
        DYING2,
        DEAD
    }
    private class DeathIKController
    {
        CrossfadeFBBIK ikSecondary;
        LifeState state;

        float currAnimTime;
        float maxTime;

        Transform player;

        public DeathIKController(CrossfadeFBBIK[] iks) {
            this.ikSecondary = iks[1];
            state = LifeState.ALIVE;
            currAnimTime = 0f;
            maxTime = 1.2f;
        }

        public void Die(Transform player)
        {
            if (state == LifeState.ALIVE)
            {
                Vector3 location = player.transform.position;
                location.y = 0;
                Quaternion forward = Quaternion.LookRotation(player.forward);
                Vector3 leftHandLocation = location + (forward * new Vector3(-0.3f, 0, 3));
                Vector3 rightHandLocation = location + (forward * new Vector3(0.3f, 0, 3));
                ikSecondary.solver.leftHandEffector.position = leftHandLocation;
                ikSecondary.solver.rightHandEffector.position = rightHandLocation;
                state = LifeState.DYING;
                this.player = player;
            }
        }

        public void Update()
        {
            float weight;
            switch (state)
            {
                case LifeState.ALIVE:
                    break;
                case LifeState.DYING:
                    currAnimTime += Time.deltaTime;
                    if (currAnimTime > maxTime)
                    {
                        currAnimTime = maxTime;
                        weight = 1;
                        state = LifeState.DYING2;
                    }
                    else
                    {
                        float normTime = currAnimTime / maxTime;
                        weight = Mathf.Sin(normTime * Mathf.PI / 2);
                    }
                    ikSecondary.solver.leftHandEffector.positionWeight = weight;
                    ikSecondary.solver.rightHandEffector.positionWeight = weight;
                    break;
                case LifeState.DYING2:
                    currAnimTime -= Time.deltaTime;
                    if (currAnimTime < 0)
                    {
                        currAnimTime = 0;
                        weight = 0;
                        state = LifeState.DEAD;
                        player.gameObject.SetActive(false);
                    }
                    break;
                case LifeState.DEAD:
                    break;
            }
        }

        public bool IsAlive()
        {
            return state == LifeState.ALIVE;
        }

        public bool IsDying()
        {
            return state == LifeState.DYING || state == LifeState.DYING2;
        }

        public bool IsDead()
        {
            return state == LifeState.DEAD;
        }
    }

    enum PointingState {
        INACTIVE,
        RAISING_ARM,
        HOLDING,
        LOWERING_ARM
    }

    private class PointIKController
    {
        CrossfadeFBBIK ikSecondary;
        PointingState state;

        IKEffector effector;
        Transform target;

        float currAnimTime;
        float maxTime;

        float holdTime;
        float maxHoldTime;

        public PointIKController(CrossfadeFBBIK[] iks)
        {
            this.ikSecondary = iks[1];
            this.state = PointingState.INACTIVE;
            this.currAnimTime = 0.0f;
            this.maxTime = 0.6f;
            this.holdTime = 0.0f;
            this.maxHoldTime = 1.0f;
        }

        public void Point(Transform player, Transform target, bool useRightHand)
        {
            if(!isActive()) {
                this.target = target;
                if (useRightHand) this.effector = ikSecondary.solver.rightHandEffector;
                else this.effector = ikSecondary.solver.leftHandEffector;
                this.effector.position = calculateHandPosition(player, target, useRightHand);
                state = PointingState.RAISING_ARM;
                ikSecondary.solver.spineStiffness = 10f;
            }
        }

        public void Update()
        {
            float normTime;
            float weight;
            switch (state)
            {
                case PointingState.INACTIVE:
                    break;
                case PointingState.RAISING_ARM:
                    currAnimTime += Time.deltaTime;
                    if (currAnimTime > maxTime)
                    {
                        currAnimTime = maxTime;
                        weight = 1;
                        state = PointingState.HOLDING;
                    }
                    else
                    {
                        normTime = currAnimTime / maxTime;
                        weight = Mathf.Sin(normTime * Mathf.PI / 2);
                    }
                    this.effector.positionWeight = weight;
                    break;
                case PointingState.HOLDING:
                    holdTime += Time.deltaTime;
                    if (holdTime > maxHoldTime)
                    {
                        holdTime = 0;
                        state = PointingState.LOWERING_ARM;
                    }
                    break;
                case PointingState.LOWERING_ARM:
                    currAnimTime -= Time.deltaTime;
                    if (currAnimTime < 0)
                    {
                        currAnimTime = 0;
                        weight = 0;
                        state = PointingState.INACTIVE;
                    }
                    else
                    {
                        normTime = currAnimTime / maxTime;
                        weight = Mathf.Sin(normTime * Mathf.PI / 2);
                    }
                    this.effector.positionWeight = weight;
                    break;
            }
        }

        public bool isActive()
        {
            return state != PointingState.INACTIVE;
        }

        public Vector3 calculateHandPosition(Transform player, Transform target, bool useRightHand)
        {
            Vector3 tg = target.position - player.position;
            tg.Normalize();
            tg = 0.3f * tg + player.position;
            tg.y = 2f;
            if (useRightHand && target.position.x > player.position.x)
            {
                Quaternion rotation = Quaternion.LookRotation(player.forward);
                tg = tg + rotation * new Vector3(0.4f, 0, 0);
            }
            else if (!useRightHand && target.position.x < player.position.x)
            {
                Quaternion rotation = Quaternion.LookRotation(player.forward);
                tg = tg + rotation * new Vector3(-0.4f, 0, 0);
            }
            return tg;
        }

    }

    enum ButtonPressState
    {
        INACTIVE,
        PRESSING,
        RETURNING
    }

    /// <summary>
    /// Controls IK for pressing a button
    /// </summary>
    private class ButtonIKController
    {
        CrossfadeFBBIK ikSecondary;
        ButtonPressState state;

        Transform currentPinPad;

        float currAnimTime;
        float maxTime;

        public ButtonIKController(CrossfadeFBBIK[] iks)
        {
            this.ikSecondary = iks[1];
            this.state = ButtonPressState.INACTIVE;
            this.currAnimTime = 0.0f;
            this.maxTime = 0.6f;
        }

        public void PressButton(Transform pinPad)
        {
            if (!IsActive())
            {
                currentPinPad = pinPad;
                ikSecondary.solver.leftHandEffector.position = pinPad.position;
                state = ButtonPressState.PRESSING;
            }
        }

        public void Update()
        {
            switch (state)
            {
                case ButtonPressState.PRESSING:
                    currAnimTime += Time.deltaTime;
                    float weight;
                    if (currAnimTime > maxTime)
                    {
                        currAnimTime = maxTime;
                        weight = 1;
                        state = ButtonPressState.RETURNING;
                        DoorController dc = currentPinPad.parent.gameObject.GetComponent<DoorController>();
                        if (dc != null)
                        {
                            dc.ToggleDoor();
                        }
                    }
                    else
                    {
                        float normTime = currAnimTime / maxTime;
                        weight = Mathf.Sin(normTime * Mathf.PI / 2);
                    }
                    ikSecondary.solver.leftHandEffector.positionWeight = weight;
                    break;
                case ButtonPressState.RETURNING:
                    currAnimTime -= Time.deltaTime;
                    if (currAnimTime < 0)
                    {
                        currAnimTime = 0;
                        weight = 0;
                        state = ButtonPressState.INACTIVE;
                    }
                    else
                    {
                        float normTime = currAnimTime / maxTime;
                        weight = Mathf.Sin(normTime * Mathf.PI / 2);
                    }
                    ikSecondary.solver.leftHandEffector.positionWeight = weight;
                    break;
                case ButtonPressState.INACTIVE:
                default:
                    break;
            }
        }

        public bool IsActive()
        {
            return state != ButtonPressState.INACTIVE;
        }
    }
    
    private enum BodyIKState
    {
        Online,
        Swapping,
        Stopping,
        Offline,
    }

    /// <summary>
    /// Controlls IK for the body
    /// </summary>
    private class BodyIKController
    {
        /// <summary>
        /// Time to take when swapping controllers
        /// </summary>
        public float SwapTime = 0.5f;

        /// <summary>
        /// Called when an InteractionEvent has been started
        /// </summary>
        public event InteractionSystem.InteractionEvent BodyStart;
        /// <summary>
        /// Called when an Interaction has been paused
        /// </summary>
        public event InteractionSystem.InteractionEvent BodyPause;
        /// <summary>
        /// Called when an Interaction has been triggered
        /// </summary>
        public event InteractionSystem.InteractionEvent BodyTrigger;
        /// <summary>
        /// Called when an Interaction has been released
        /// </summary>
        public event InteractionSystem.InteractionEvent BodyRelease;
        /// <summary>
        /// Called when an InteractionObject has been picked up.
        /// </summary>
        public event InteractionSystem.InteractionEvent BodyPickUp;
        /// <summary>
        /// Called when a paused Interaction has been resumed
        /// </summary>
        public event InteractionSystem.InteractionEvent BodyResume;
        /// <summary>
        /// Called when an Interaction has been stopped
        /// </summary>
        public event InteractionSystem.InteractionEvent BodyStop;

        private CrossfadeFBBIK ikPrimary = null;
        private CrossfadeFBBIK ikSecondary = null;

        private CrossfadeInteractionHandler handlerPrimary = null;
        private CrossfadeInteractionHandler handlerSecondary = null;

        private BodyIKState state;
        public BodyIKState State { get { return this.state; } }

        private float swapTimeFinish;

        public Dictionary<FullBodyBipedEffector, InteractionObject> primaryEffectors;
        public Dictionary<FullBodyBipedEffector, InteractionObject> secondaryEffectors;

        public BodyIKController(CrossfadeFBBIK[] iks, float swapTime)
        {
            this.primaryEffectors =
                new Dictionary<FullBodyBipedEffector, InteractionObject>();
            this.secondaryEffectors =
                new Dictionary<FullBodyBipedEffector, InteractionObject>();

            this.ikPrimary = iks[0];
            this.ikSecondary = iks[1];
            this.SwapTime = swapTime;

            this.handlerPrimary = new CrossfadeInteractionHandler(ikPrimary);
            this.handlerSecondary = new CrossfadeInteractionHandler(ikSecondary);

            this.RegisterWithHandler(this.handlerPrimary);
            this.RegisterWithHandler(this.handlerSecondary);

            this.state = BodyIKState.Offline;
        }

        public void Update()
        {
            if (this.state == BodyIKState.Swapping)
            {
                if (Time.time > this.swapTimeFinish)
                {
                    foreach (var kv in this.secondaryEffectors)
                        this.handlerSecondary.StopInteraction(kv.Key);
                    this.secondaryEffectors.Clear();
                    this.state = BodyIKState.Online;
                }
            }
            else if (this.state == BodyIKState.Stopping)
            {
                foreach (var kv in this.secondaryEffectors)
                    this.handlerSecondary.StopInteraction(kv.Key);
                this.secondaryEffectors.Clear();

                foreach (var kv in this.primaryEffectors)
                    this.handlerSecondary.StopInteraction(kv.Key);
                this.secondaryEffectors.Clear();

                if (this.IsActive(this.ikPrimary) == false)
                    this.state = BodyIKState.Offline;
            }
        }

        public void LateUpdate()
        {
            this.handlerPrimary.LateUpdate();
            this.handlerSecondary.LateUpdate();

            this.ikPrimary.GetIKSolver().Update();
            this.ikSecondary.GetIKSolver().Update();
        }

        public void StartInteraction(FullBodyBipedEffector effector, InteractionObject obj)
        {
            if (this.state == BodyIKState.Offline)
            {
                this.primaryEffectors.Add(effector, obj);
                this.handlerPrimary.StartInteraction(effector, obj, true);
                this.state = BodyIKState.Online;
            }
            else if (this.state == BodyIKState.Online
                || this.state == BodyIKState.Swapping)
            {
                // Is this effector already being used?
                if (this.primaryEffectors.ContainsKey(effector))
                {
                    this.PerformSwap(effector, obj);
                }
                else
                {
                    this.primaryEffectors.Add(effector, obj);
                    this.handlerPrimary.StartInteraction(effector, obj, true);
                }
            }
            else if (this.state == BodyIKState.Stopping)
            {
                this.primaryEffectors.Add(effector, obj);
                this.handlerPrimary.StartInteraction(effector, obj, true);
                this.state = BodyIKState.Online;
            }
        }

        public void ResumeInteraction(FullBodyBipedEffector effector)
        {
            if (this.state == BodyIKState.Online
                || this.state == BodyIKState.Swapping)
            {
                // TODO: What if we swap immediately after? The interaction
                //       will get stuck at the trigger again.
                this.handlerPrimary.ResumeInteraction(effector);
            }
        }

        private void PerformSwap(FullBodyBipedEffector effector, InteractionObject obj)
        {
            // Move all the effectors to the secondary IK solver
            foreach (var kv in this.primaryEffectors)
            {
                if (kv.Key != effector)
                {
                    this.handlerSecondary.StartInteraction(kv.Key, kv.Value, true);
                    this.secondaryEffectors[kv.Key] = kv.Value;
                }
            }
            this.handlerSecondary.StartInteraction(effector, obj, true);
            this.secondaryEffectors[effector] = obj;

            // Swap solvers
            this.Swap();

            // Store the intermediate state
            float time = Time.time;
            this.swapTimeFinish = time + this.SwapTime;
            this.state = BodyIKState.Swapping;
        }

        public void StopInteraction(FullBodyBipedEffector effector)
        {
            if (this.primaryEffectors.ContainsKey(effector) == true)
            {
                this.primaryEffectors.Remove(effector);
                this.handlerPrimary.StopInteraction(effector);

                // If this is our last active effector, shut down
                if (this.primaryEffectors.Count == 0)
                    this.state = BodyIKState.Stopping;
            }
        }

        private bool IsActive(CrossfadeFBBIK fbbik)
        {
            foreach (IKEffector eff in fbbik.solver.effectors)
                if (eff.positionWeight > 0.05f || eff.rotationWeight > 0.05f)
                    return true;
            return false;
        }

        private void RegisterWithHandler(CrossfadeInteractionHandler handler)
        {
            handler.InteractionStart += this.OnInteractionStart;
            handler.InteractionTrigger += this.OnInteractionTrigger;
            handler.InteractionRelease += this.OnInteractionRelease;
            handler.InteractionPause += this.OnInteractionPause;
            handler.InteractionPickUp += this.OnInteractionPickUp;
            handler.InteractionResume += this.OnInteractionResume;
            handler.InteractionStop += this.OnInteractionStop;
        }

        private void Swap()
        {
            CrossfadeFBBIK ikTemp = this.ikPrimary;
            this.ikPrimary = this.ikSecondary;
            this.ikSecondary = ikTemp;

            CrossfadeInteractionHandler handlerTemp = this.handlerPrimary;
            this.handlerPrimary = this.handlerSecondary;
            this.handlerSecondary = handlerTemp;

            Dictionary<FullBodyBipedEffector, InteractionObject> temp =
                this.primaryEffectors;
            this.primaryEffectors = this.secondaryEffectors;
            this.secondaryEffectors = temp;
        }

        #region Event Bounce
        private void OnInteractionStart(
            CrossfadeInteractionHandler handler,
            FullBodyBipedEffector effectorType,
            InteractionObject interactionObject)
        {
            if (this.BodyStart != null)
                this.BodyStart(effectorType, interactionObject);
        }

        private void OnInteractionTrigger(
            CrossfadeInteractionHandler handler,
            FullBodyBipedEffector effectorType,
            InteractionObject interactionObject)
        {
            if (this.BodyTrigger != null)
                this.BodyTrigger(effectorType, interactionObject);
        }

        private void OnInteractionRelease(
            CrossfadeInteractionHandler handler,
            FullBodyBipedEffector effectorType,
            InteractionObject interactionObject)
        {
            if (this.BodyRelease != null)
                this.BodyRelease(effectorType, interactionObject);
        }

        private void OnInteractionPause(
            CrossfadeInteractionHandler handler,
            FullBodyBipedEffector effectorType,
            InteractionObject interactionObject)
        {
            if (this.BodyPause != null)
                this.BodyPause(effectorType, interactionObject);
        }

        private void OnInteractionPickUp(
            CrossfadeInteractionHandler handler,
            FullBodyBipedEffector effectorType,
            InteractionObject interactionObject)
        {
            if (this.BodyPickUp != null)
                this.BodyPickUp(effectorType, interactionObject);
        }

        private void OnInteractionResume(
            CrossfadeInteractionHandler handler,
            FullBodyBipedEffector effectorType,
            InteractionObject interactionObject)
        {
            if (this.BodyResume != null)
                this.BodyResume(effectorType, interactionObject);
        }

        private void OnInteractionStop(
            CrossfadeInteractionHandler handler,
            FullBodyBipedEffector effectorType,
            InteractionObject interactionObject)
        {
            if (this.BodyStop != null)
                this.BodyStop(effectorType, interactionObject);
        }
        #endregion
    }

    /// <summary>
    /// Called when an InteractionEvent has been started
    /// </summary>
    public event InteractionSystem.InteractionEvent InteractionStart;
    /// <summary>
    /// Called when an Interaction has been paused
    /// </summary>
    public event InteractionSystem.InteractionEvent InteractionPause;
    /// <summary>
    /// Called when an Interaction has been triggered
    /// </summary>
    public event InteractionSystem.InteractionEvent InteractionTrigger;
    /// <summary>
    /// Called when an Interaction has been released
    /// </summary>
    public event InteractionSystem.InteractionEvent InteractionRelease;
    /// <summary>
    /// Called when an InteractionObject has been picked up.
    /// </summary>
    public event InteractionSystem.InteractionEvent InteractionPickUp;
    /// <summary>
    /// Called when a paused Interaction has been resumed
    /// </summary>
    public event InteractionSystem.InteractionEvent InteractionResume;
    /// <summary>
    /// Called when an Interaction has been stopped
    /// </summary>
    public event InteractionSystem.InteractionEvent InteractionStop;

    /// <summary>
    /// Default delay time
    /// </summary>
    public float DefaultDelay = 0.5f;

    /// <summary>
    /// Maximum weight for body control
    /// </summary>
    public float BodyWeightMax = 0.5f;

    private LookAtIKController lookController;
    private BodyIKController bodyController;
    private PointIKController pointController;
    private ButtonIKController buttonController;
    private PrayingIKController prayerController;
    private DeathIKController deathController;

    void Awake()
    {
        this.lookController = new LookAtIKController(
            this.GetComponent<CrossfadeLookAtIK>(),
            this.BodyWeightMax,
            this.DefaultDelay);
        this.bodyController = new BodyIKController(
            this.GetComponents<CrossfadeFBBIK>(),
            this.DefaultDelay);
        this.pointController = new PointIKController(
            this.GetComponents<CrossfadeFBBIK>());
        this.buttonController = new ButtonIKController(
            this.GetComponents<CrossfadeFBBIK>());
        this.prayerController = new PrayingIKController(
            this.GetComponents<CrossfadeFBBIK>());
        this.deathController = new DeathIKController(
            this.GetComponents<CrossfadeFBBIK>());
        this.RegisterWithBodyController();
    }

    void Update()
    {
        this.bodyController.Update();
        this.lookController.Update();
        this.pointController.Update();
        this.buttonController.Update();
        this.prayerController.Update();
        this.deathController.Update();

        if (this.bodyController.State == BodyIKState.Offline
            && this.lookController.IsFullBody() == false)
            this.lookController.FullBody(this.DefaultDelay);
    }

    void LateUpdate()
    {
        this.bodyController.LateUpdate();
        this.lookController.LateUpdate();
    }

    public void Die(Transform player)
    {
        this.deathController.Die(player);
    }

    public bool IsAlive()
    {
        return this.deathController.IsAlive();
    }

    public bool IsDying()
    {
        return this.deathController.IsDying();
    }

    public bool IsDead()
    {
        return this.deathController.IsDead();
    }

    public void PointAt(Transform player, Transform target, bool useRightHand)
    {
        this.pointController.Point(player, target, useRightHand);
    }

    public bool IsPointing()
    {
        return this.pointController.isActive();
    }

    public void StartPrayer(Transform player, Transform egg)
    {
        this.prayerController.StartPrayer(player, egg);
    }

    public void EndPrayer(Transform egg)
    {
        this.prayerController.EndPrayer(egg);
    }

    public bool IsPraying()
    {
        return this.prayerController.IsActive();
    }

    public void PressButton(Transform pinPad)
    {
        this.buttonController.PressButton(pinPad);
    }

    public bool IsPressingButton()
    {
        return this.buttonController.IsActive();
    }

    public void LookAt(Vector3 target, float delay)
    {
        this.lookController.LookAt(target, delay);
    }

    public void LookStop(float delay)
    {
        this.lookController.LookStop(delay);
    }

    public void LookAt(Vector3 target)
    {
        this.lookController.LookAt(target, this.DefaultDelay);
    }

    public void LookStop()
    {
        this.lookController.LookStop(this.DefaultDelay);
    }

    public void StartInteraction(
        FullBodyBipedEffector effector, 
        InteractionObject obj)
    {
        this.bodyController.StartInteraction(effector, obj);
        this.lookController.HeadOnly(this.DefaultDelay);
    }

    public void ResumeInteraction(
        FullBodyBipedEffector effector)
    {
        this.bodyController.ResumeInteraction(effector);
        this.lookController.HeadOnly(this.DefaultDelay);
    }

    public void StopInteraction(FullBodyBipedEffector effector)
    {
        this.bodyController.StopInteraction(effector);
    }

    private void RegisterWithBodyController()
    {
        this.bodyController.BodyStart += this.OnInteractionStart;
        this.bodyController.BodyTrigger += this.OnInteractionTrigger;
        this.bodyController.BodyRelease += this.OnInteractionRelease;
        this.bodyController.BodyPause += this.OnInteractionPause;
        this.bodyController.BodyPickUp += this.OnInteractionPickUp;
        this.bodyController.BodyResume += this.OnInteractionResume;
        this.bodyController.BodyStop += this.OnInteractionStop;
    }

    #region Event Bounce
    private void OnInteractionStart(
        FullBodyBipedEffector effectorType,
        InteractionObject interactionObject)
    {
        if (this.InteractionStart != null)
            this.InteractionStart(effectorType, interactionObject);
    }

    private void OnInteractionTrigger(
        FullBodyBipedEffector effectorType,
        InteractionObject interactionObject)
    {
        if (this.InteractionTrigger != null)
            this.InteractionTrigger(effectorType, interactionObject);
    }

    private void OnInteractionRelease(
        FullBodyBipedEffector effectorType,
        InteractionObject interactionObject)
    {
        if (this.InteractionRelease != null)
            this.InteractionRelease(effectorType, interactionObject);
    }

    private void OnInteractionPause(
        FullBodyBipedEffector effectorType,
        InteractionObject interactionObject)
    {
        if (this.InteractionPause != null)
            this.InteractionPause(effectorType, interactionObject);
    }

    private void OnInteractionPickUp(
        FullBodyBipedEffector effectorType,
        InteractionObject interactionObject)
    {
        if (this.InteractionPickUp != null)
            this.InteractionPickUp(effectorType, interactionObject);
    }

    private void OnInteractionResume(
        FullBodyBipedEffector effectorType,
        InteractionObject interactionObject)
    {
        if (this.InteractionResume != null)
            this.InteractionResume(effectorType, interactionObject);
    }

    private void OnInteractionStop(
        FullBodyBipedEffector effectorType,
        InteractionObject interactionObject)
    {
        if (this.InteractionStop != null)
            this.InteractionStop(effectorType, interactionObject);
    }
    #endregion
}
