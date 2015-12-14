using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using System.Collections.Generic;

public class BehaviorTree : MonoBehaviour
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
        return new Sequence(DesignateThiefNode(),
            new DecoratorLoop(0, DirectShoppersNode()));
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
        return new Sequence(Speak(a, "Hello!", "WAVE"),
                            Speak(b, "Hi!", "WAVE"),
                            Speak(a, "Have you got the product?", "THINK"),
                            Speak(b, "I think someone already picked it up...", "THINK"),
                            Speak(b, "I'm sorry", "HANDSUP"),
                            Speak(a, "Well, I'll just go get it from someone else", "WAVE"),
                            Speak(b, "See you later friend", "WAVE"));
    }

    protected Node ConverseBoth(GameObject a)
    {
        return new Sequence(Speak(a, "Hello!", "WAVE"),
                            SpeakOther(a, "Hi!", "WAVE"),
                            Speak(a, "Have you got the product?", "THINK"),
                            SpeakOther(a, "I think someone already picked it up...", "THINK"),
                            SpeakOther(a, "I'm sorry", "HANDSUP"),
                            Speak(a, "Well, I'll just go get it from someone else", "WAVE"),
                            SpeakOther(a, "See you later friend", "WAVE"));
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
        Quaternion rotation = Quaternion.LookRotation(target.Value - a.transform.position);
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

    protected Node DesignateThiefNode()
    {
        return new LeafInvoke(
            () => DesignateThief());
    }
    protected void DesignateThief()
    {
        GameObject[] shoppers = GameObject.FindGameObjectsWithTag("Shopper");
        int index = UnityEngine.Random.Range(0, shoppers.Length - 1);
        shoppers[index].tag = "Thief";
    }

    protected Node DirectShoppersNode()
    {
        GameObject[] shoppers = GameObject.FindGameObjectsWithTag("Shopper");
        GameObject[] thief = GameObject.FindGameObjectsWithTag("Thief");
        return new Sequence(
                new ForEach<GameObject>(RepeatedShopArc, shoppers),
                new ForEach<GameObject>(RepeatedShopArc, thief)
            );
    }
    protected Node RepeatedShopArc(GameObject shopper)
    {
        return new DecoratorLoop(6, ShopArc(shopper));
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
    protected Node Shop(GameObject shopper, Val<GameObject> salesman, Val<GameObject> stallSpot)
    {
        return new Sequence(
                MaintainEyeContactWhileConversing(shopper, salesman.Value, eyeHeight),
                new LeafInvoke(
                        delegate
                        {
                            stallSpot.Value.tag = "PurchaseEmpty";
                        }
                    )
            );
    }

    protected Node d(string msg)
    {
        return new LeafInvoke(delegate { Debug.Log(msg); });
    }
}
