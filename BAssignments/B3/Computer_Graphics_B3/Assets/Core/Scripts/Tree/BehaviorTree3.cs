using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using System.Collections.Generic;

public class BehaviorTree3 : MonoBehaviour
{

    public Vector3 eyeHeight;
    public long gestureDuration;

    private BehaviorAgent behaviorAgent;

    // Use this for initialization
    void Start()
    {
        behaviorAgent = new BehaviorAgent(BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private BehaviorMecanim mec(GameObject g)
    {
        return g.GetComponent<BehaviorMecanim>();
    }

    protected Node Gesture(GameObject g, string gestureName)
    {
        return mec(g).ST_PlayHandGesture(gestureName, gestureDuration);
    }

    protected Node Speak(GameObject g, string message, string gestureName)
    {
        return new Sequence(
            mec(g).Node_HeadLookBoth(eyeHeight),
            mec(g).Node_SpeechBubble(message, gestureDuration),
            mec(g).ST_PlayHandGesture(gestureName, gestureDuration)
            );
    }

    protected Node SpeakOther(GameObject g, string message, string gestureName)
    {
        return new Sequence(
            mec(g).Node_HeadLookBoth(eyeHeight),
            mec(g).Node_SpeechBubbleOther(message, gestureDuration),
            mec(g).ST_PlayHandGestureOther(gestureName, gestureDuration)
            );
    }

    protected Node PointAt(GameObject g, GameObject target, bool useRightHand)
    {
        Val<Transform> transform = Val.V<Transform>(target.transform);
        Val<Vector3> position = Val.V<Vector3>(target.transform.position);
        return new Sequence(mec(g).Node_OrientTowards(position), mec(g).Node_PointAt(transform, Val.V<bool>(useRightHand)));
    }

    protected Node BuildTreeRoot()
    {
        DesignateThief();
        return new Sequence(DirectShoppersNode());
    }


    protected Node MaintainEyeContact(GameObject a, GameObject b, Vector3 eyeHeight)
    {
        Val<Vector3> p1Head = Val.V(() => a.transform.position + eyeHeight);
        Val<Vector3> p2Head = Val.V(() => b.transform.position + eyeHeight);
        return new DecoratorLoop(new SequenceParallel(mec(a).Node_HeadLook(p2Head), mec(b).Node_HeadLook(p1Head)));
    }

    protected Node MaintainEyeContactBoth(GameObject a, Vector3 eyeHeight)
    {
        return new DecoratorLoop(mec(a).Node_HeadLookBoth(eyeHeight));
    }

    protected Node Converse(GameObject a, GameObject b)
    {
        return new Sequence(Speak(a, "", "WAVE"),
                            Speak(b, "", "WAVE"),
                            Speak(a, "", "THINK"),
                            Speak(b, "", "THINK"),
                            Speak(b, "", "HANDSUP"),
                            Speak(a, "", "WAVE"),
                            Speak(b, "", "WAVE"));
    }

    protected Node ConverseBoth(GameObject a)
    {
        return new Sequence(Speak(a, "Hi!", "WAVE"),
                            SpeakOther(a, "Welcome!", "WAVE"),
                            Speak(a, "", "THINK"),
                            SpeakOther(a, "", "THINK"),
                            SpeakOther(a, "", "HANDSUP"),
                            Speak(a, "See ya!", "WAVE"),
                            SpeakOther(a, "Bye!", "WAVE"));
    }

    protected Node MaintainEyeContactWhileConversing(GameObject a, GameObject b, Vector3 eyeHeight)
    {
        return new Race(MaintainEyeContact(a, b, eyeHeight), Converse(a, b));
    }

    protected Node MaintainEyeContactWhileConversingBoth(GameObject a, Vector3 eyeHeight)
    {
        return new Race(MaintainEyeContactBoth(a, eyeHeight), ConverseBoth(a));
    }

    protected Node ApproachAndOrient(GameObject a, GameObject b, Val<Vector3> target, Val<float> distance)
    {
        Quaternion rotA = Quaternion.LookRotation(b.transform.position - a.transform.position);
        Vector3 targetA = target.Value - (rotA * new Vector3(0, 0, distance.Value));
        Quaternion rotB = Quaternion.LookRotation(a.transform.position - b.transform.position);
        Vector3 targetB = target.Value - (rotB * new Vector3(0, 0, distance.Value));

        Val<Vector3> p1 = Val.V(() => targetA);
        Val<Vector3> p2 = Val.V(() => targetB);
        return new Sequence(new SequenceParallel(mec(a).Node_OrientTowards(p1),
                                                 mec(b).Node_OrientTowards(p2)),
                            new SequenceParallel(mec(a).Node_GoTo(p1),
                                                 mec(b).Node_GoTo(p2)),
                                                 new LeafInvoke(delegate { Debug.Log("Done walking"); }),
                            new SequenceParallel(mec(a).Node_OrientTowards(target),
                                                 mec(b).Node_OrientTowards(target)));
    }


    protected Node ApproachAndOrientTarget(GameObject a, Val<Vector3> target, Val<float> distance)
    {
        Quaternion rotation = Quaternion.LookRotation(target.Value - a.transform.position);
        Vector3 targetLoc = target.Value - (rotation * new Vector3(0, 0, distance.Value));
        Val<Vector3> targetAdjusted = Val.V(() => targetLoc);
        return new Sequence(mec(a).Node_GoTo(targetAdjusted), mec(a).Node_OrientTowards(target));
    }

    protected Node ApproachAndOrientUnderTarget(GameObject a, Val<Vector3> target, Val<float> distance)
    {
        Vector3 targetLoc = target.Value - new Vector3(0, 0, distance.Value);
        Val<Vector3> targetAdjusted = Val.V(() => targetLoc);
        return new Sequence(mec(a).Node_GoTo(targetAdjusted), mec(a).Node_OrientTowards(target));
    }

    protected Node AngryGesture(GameObject guy, GameObject guard)
    {
        Val<Vector3> target = Val.V(() => guard.transform.position);
        float distance = 2.3f;
        return new Sequence(ApproachAndOrientTarget(guy, target, distance));//, mec(guy).Play_AngryGesture());
    }

    protected System.Action Orient(GameObject a, Vector3 b)
    {
        return delegate
        {
            UnitySteeringController usc = a.GetComponent<UnitySteeringController>();
            usc.Target = b;
            usc.Stop();
        };
    }

    protected void DesignateThief()
    {
        GameObject[] shoppers = GameObject.FindGameObjectsWithTag("Shopper");
        int index = UnityEngine.Random.Range(0, shoppers.Length - 1);
        shoppers[index].tag = "Thief";
        shoppers[index].AddComponent<ThiefMeta>();
    }

    protected Node DirectShoppersNode()
    {
        GameObject[] shoppers = GameObject.FindGameObjectsWithTag("Shopper");
        GameObject[] thief = GameObject.FindGameObjectsWithTag("Thief");
        return new SequenceParallel(
                new ForEach<GameObject>(SynchronizedShopArc, shoppers),
                new ForEach<GameObject>(SynchronizedTheftArc, thief)
            );
    }
    protected Node SynchronizedShopArc(GameObject shopper)
    {
        return new SequenceParallel(new PossessionNode(UnPossessedShopArc(shopper), shopper, false),
                                    new PossessionNode(PossessedShopArc(shopper), shopper, true));
    }
    protected Node UnPossessedShopArc(GameObject shopper)
    {
        return new Sequence(
                new DecoratorForceStatus(
                    RunStatus.Success,
                    new Sequence(
                        new LeafAssert(() => IsShopperActive(shopper)),
                        new SequenceParallel(
                            new DecoratorLoop(new LeafAssert(() => IsShopperActive(shopper))),
                            RepeatedShopArc(shopper)
                        )
                    )
                ),
                new Selector(
                        new Sequence(
                            new LeafAssert(() => IsNotDismissed(shopper))
                        ),
                        new Sequence(
                            new LeafAssert(() => IsNotActive(shopper))  
                        ),
                        new Sequence(
                            mec(shopper).Node_Escape(),
                            new LeafInvoke(() => shopper.SetActive(false))
                        )
                )
            );
    }
    protected Node PossessedShopArc(GameObject shopper)
    {
        return new DecoratorLoop(new LeafInvoke(delegate {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
                Vector3 target = shopper.transform.position + v * (Quaternion.Euler(0, h * 20, 0) * shopper.transform.forward);
                Vector3 targetDirection = shopper.transform.position + 10 * (Quaternion.Euler(0, h * 40, 0) * shopper.transform.forward);
                if (mec(shopper).Character.NavGoTo(target) == RunStatus.Failure)
                    mec(shopper).Character.NavStop();
                if (mec(shopper).Character.NavTurn(targetDirection) == RunStatus.Failure)
                    mec(shopper).Character.NavOrientBehavior(OrientationBehavior.LookForward);
        }));
    }
    protected Node SynchronizedTheftArc(GameObject thief)
    {
        return new Sequence(
                new DecoratorForceStatus(
                    RunStatus.Success,
                    new SequenceParallel(
                        new DecoratorLoop(new LeafAssert(() => IsShopperActive(thief))),
                        RepeatedTheftArc(thief)
                    )
                ),
                mec(thief).Node_DropAll(),
                new ForEach<GameObject>(StopGuard, GameObject.FindGameObjectsWithTag("Guard")),
                mec(thief).Node_Escape(),
                new LeafInvoke(() => thief.SetActive(false)),
                new LeafInvoke(() => SuccessGameOver())
            );
    }
    protected RunStatus SuccessGameOver()
    {
        SceneController sc = this.gameObject.GetComponent<SceneController>();
        sc.endMessage = "And that's how the lamp was saved!";
        sc.state = SceneState.ENDING;
        return RunStatus.Success;
    }
    protected Node RepeatedShopArc(GameObject shopper)
    {
        return new DecoratorLoop(4, ShopArc(shopper));
    }
    protected Node RepeatedTheftArc(GameObject thief)
    {
        int num_iter = UnityEngine.Random.Range(1, 3);
        return new Sequence(
            new DecoratorLoop(num_iter, ShopArc(thief)),
            TheftArc(thief)
        );
    }
    protected bool IsShopperActive(GameObject shopper)
    {
        bool active = shopper.GetComponent<PossessionScript>().IsTreeControlled();
        if (!active) Debug.Log("Freed from arc");
        return active;
    }
    protected bool IsNotDismissed(GameObject shopper)
    {
        bool notDismissed = !shopper.GetComponent<PossessionScript>().IsDismissable();
        return notDismissed;
    }
    protected bool IsNotActive(GameObject shopper)
    {
        bool isNotActive = !shopper.activeSelf;
        return isNotActive;
    }
    protected Node ShopArc(GameObject shopper)
    {
        return new Sequence(
               mec(shopper).Node_ChooseNextShop(),
               mec(shopper).Node_GoToNextShop(),
               mec(shopper).Node_OrientTowardsShop(),
               ConverseBoth(shopper),
               mec(shopper).Node_LeaveShop()
        );
    }
    protected Node TheftArc(GameObject thief)
    {
        return new Sequence(
                mec(thief).Node_ChooseNextShop(),
                mec(thief).Node_GoToNextShop(),
                mec(thief).Node_OrientTowardsShop(),
                Speak(thief, "I want that lamp!", "HANDSUP"),
                SpeakOther(thief, "You need to buy it!", "HANDSUP"),
                Speak(thief, "No! I'll steal it!", "HANDSUP"),
                SpeakOther(thief, "The guards will stop you!", "HANDSUP"),
                Speak(thief, "What makes you think that? I'm not some chump!", "HANDSUP"),
                SpeakOther(thief, "Yes you are! CHUMP! CHUMP! GUARDS, HELP ME!", "HANDSUP"),
                Speak(thief, "They can't help you now!!!!", "HANDSUP"),
                SpeakOther(thief, "We'll see about that, you thief!", "HANDSUP"),
                Speak(thief, "Indeed, we shall... for now, this is mine!", "HANDSUP"),
                mec(thief).Node_ChooseTheftTarget(),
                mec(thief).Node_OrientTowardsLamp(),
                mec(thief).Node_PointAtLamp(),
                new LeafWait(600L),
                mec(thief).Node_PullLamp(),
                new LeafWait(1600L),
                new SequenceParallel(
                    new ForEach<GameObject>(ChaseThief, GameObject.FindGameObjectsWithTag("Guard")),
                    new Sequence(mec(thief).Node_Escape(),
                        new ForEach<GameObject>(StopGuard, GameObject.FindGameObjectsWithTag("Guard")),
                        new LeafInvoke(delegate { this.gameObject.GetComponent<SceneController>().state = SceneState.ENDING; })
                        ))
        );
    }
    protected Node StopGuard(GameObject guard)
    {
        return new LeafInvoke(() => mec(guard).Character.NavStop());
    }
    protected Node ChaseThief(GameObject guard)
    {
        GameObject thief = GameObject.FindGameObjectWithTag("Thief");
        return new Sequence(mec(guard).Node_Chase(thief));
    }

    protected Node d(string msg)
    {
        return new LeafInvoke(delegate { Debug.Log(msg); });
    }
}
