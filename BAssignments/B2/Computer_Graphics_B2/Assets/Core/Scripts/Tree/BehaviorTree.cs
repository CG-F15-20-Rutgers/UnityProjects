using UnityEngine;
using System;
using System.Collections;
using TreeSharpPlus;
using System.Collections.Generic;

public class BehaviorTree : MonoBehaviour {

    public GameObject follower1;
    public GameObject follower2;
    public GameObject leader;
    public Vector3 eyeHeight;
    public GameObject egg;
    public GameObject[] zealots;
    Dictionary<GameObject, GameObject> gzMappings = new Dictionary<GameObject, GameObject>();
    public long gestureDuration;

    public GameObject guard_l1;

    private BehaviorAgent behaviorAgent;
    private int numPrayerArcs = 0;

    // Use this for initialization
    void Start() {
        behaviorAgent = new BehaviorAgent(BuildTreeRoot());
        BehaviorManager.Instance.Register(behaviorAgent);
        behaviorAgent.StartBehavior();
        numPrayerArcs = 0;
    }

    // Update is called once per frame
    void Update() {

    }

    private BehaviorMecanim mec(GameObject g) {
        return g.GetComponent<BehaviorMecanim>();
    }

    protected Node Gesture(GameObject g, string gestureName) {
        return mec(g).ST_PlayHandGesture(gestureName, gestureDuration);
    }

	protected Node Speak(GameObject g, string message, string gestureName) {
		return mec(g).ST_DisplaySpeechBubble (message, gestureName, gestureDuration);
	}

    protected Node PointAt(GameObject g, GameObject target, bool useRightHand) {
        Val<Transform> transform = Val.V<Transform>(target.transform);
        Val<Vector3> position = Val.V<Vector3>(target.transform.position);
        return new Sequence(mec(g).Node_OrientTowards(position), mec(g).Node_PointAt (transform, Val.V<bool>(useRightHand)));
    }

    protected Node BuildTreeRoot() {
        Debug.Log("Build Tree Root");
        return new Sequence(IntroTree(), MiddleArc(), PrayArc());
    }

    protected Node IntroTree() {
        Val<Vector3> target = Val.V(() => (follower1.transform.position + follower2.transform.position) / 2);
        float distance = 1.5f;
        GameObject[] guards = getChildrenForWave(getWaves()[0], true);
        return new Sequence(
            ApproachAndOrient(follower1, follower2, target, distance),
            MaintainEyeContactWhileConversing(follower1, follower2, eyeHeight),
            new SequenceParallel(
                AngryGesture(follower2, guards[0]),
                AngryGesture(follower1, guards[1])));
    }

    protected Node MaintainEyeContact(GameObject a, GameObject b, Vector3 eyeHeight) {
        Val<Vector3> p1Head = Val.V(() => a.transform.position + eyeHeight);
        Val<Vector3> p2Head = Val.V(() => b.transform.position + eyeHeight);
        return new DecoratorLoop(new SequenceParallel(mec(a).Node_HeadLook(p2Head), mec(b).Node_HeadLook(p1Head)));
    }

    protected Node Converse(GameObject a, GameObject b) {
        return new Sequence(Speak(a, "Hello!", "WAVE"),
                            Speak(b, "Hi!", "WAVE"),

                            PointAt(a, guard_l1, true),
                            new LeafWait(Val.V<long>((long)1400.0)),
                            Speak(a, "Do you know what they are guarding?", "THINK"),

                            Speak(b, "No! do you want to find out?", "HANDSUP"),

                            Speak(a, "Yes, but we will need to kill the guards", "CUTTHROAT"),

                            Speak(b, "Okay, lets do it!", "CHEER"),
                            new LeafInvoke(delegate { Debug.Log("Done"); }));
    }

    protected Node MaintainEyeContactWhileConversing(GameObject a, GameObject b, Vector3 eyeHeight) {
        return new Race(MaintainEyeContact(a, b, eyeHeight), Converse(a, b));
    }

    protected Node ApproachAndOrient(GameObject a, GameObject b, Val<Vector3> target, Val<float> distance) {
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
                                                 new LeafInvoke(delegate{Debug.Log("Done walking");}),
                            new SequenceParallel(mec(a).Node_OrientTowards(target),
                                                 mec(b).Node_OrientTowards(target)));
    }

    // pragma mark new code.

    protected Node MiddleArc() {
        Wave[] waves = getWaves();
        return new Sequence(OpenDoorArc(waves[0]), OpenDoorArc(waves[1]), OpenDoorArc(waves[2]));
    }

    protected Node OpenDoorArc(Wave wave)
    {
        GameObject[] guards = getChildrenForWave(wave, true);
        zealots = GameObject.FindGameObjectsWithTag("Zealot");
        Debug.Log(guards.Length);
        System.Action func = delegate
        {
            Debug.Log("Creating matchups");
            for(int i = 0; i < guards.Length; i++)
            {
                Debug.Log("Added mapping");
                gzMappings.Add(guards[i], zealots[i]);
            }
        };

        GameObject pinPad = getChildrenForWave(wave, false)[0];
        Vector3 pinPadPosition = pinPad.transform.position - new Vector3(-4, 0, 3);
        Debug.Log(pinPad.transform.position);
        return new Sequence(//new LeafInvoke(func),
            new LeafInvoke(delegate {Debug.Log("Running attaack node");}),
            new SequenceParallel(
                   AttackArc(guards[0], zealots[0]),
                   AttackArc(guards[1], zealots[1])
                ),
                PressButton(zealots[1], pinPad),
            //ApproachAndOrientUnderTarget(zealots[1], Val.V(pinPadPosition),0f),
            //new LeafInvoke(delegate { Debug.Log("ATK"); }),
            new LeafInvoke(delegate { Debug.Log("Called"); zealots[1].GetComponent<IKController>().PressButton(pinPad.transform); Debug.Log("Invoked"); }),
            new LeafWait(1200),
            //new ForEach<GameObject>(AttackArc, gzMappings.Keys),
            new LeafInvoke(delegate {Debug.Log("Finished attack node.");}));
            //new LeafInvoke(func2));
        }

    protected Node AttackArc(GameObject guard, GameObject zealot) {
        Debug.Log("Attack arc");
        //GameObject zealot;
        //gzMappings.TryGetValue(guard, out zealot);
        Val<Vector3> target = Val.V(() => guard.transform.position);
        int iter = UnityEngine.Random.Range(1, 1);
        float distance = 5.0f;

        System.Action func = delegate
        {
            IKController ikc = guard.GetComponent<IKController>();
            ikc.Die(guard.transform);
        };

        // TODO: Implement FancyDeathAnimation
        return new Sequence(ApproachAndOrientUnderTarget(zealot, Val.V(guard.transform.position), 1f),
                            new DecoratorLoop(iter,
                                              new SequenceShuffle(mec(zealot).ST_PlayBodyGesture("NEW_PUNCH", 500),
                                              mec(guard).ST_PlayBodyGesture("NEW_PUNCH", 500))),
                            new LeafInvoke(func));
    }

    protected Node PressButton(GameObject zealot, GameObject pinPad)
    {
        Vector3 location = pinPad.transform.position - new Vector3(-1, 0, 0);
        location.y = 0;
        Val<Vector3> target = Val.V(() => location);
        return new Sequence(ApproachAndOrientUnderTarget(zealot, target, 1f),
                            new LeafInvoke(delegate { zealot.GetComponent<IKController>().PressButton(pinPad.transform); }));
    }

    protected Node ApproachAndOrientTarget(GameObject a, Val<Vector3> target, Val<float> distance) {
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

    protected Node AngryGesture(GameObject guy, GameObject guard) {
        Val<Vector3> target = Val.V(() => guard.transform.position);
        float distance = 2.3f;
        return new Sequence(ApproachAndOrientTarget(guy, target, distance) );//, mec(guy).Play_AngryGesture());
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

    // TODO: add an offset to the egg target?
    protected Node PrayArcFactory(GameObject guy) {
        float distance = 0.0f;
        float checkDistance = 5.0f;
        System.Func<bool> playerIsNearEgg = delegate() {
            return (guy.transform.position - egg.transform.position).sqrMagnitude <= Mathf.Pow(checkDistance, 2);
        };
        float angle = (numPrayerArcs++ * 30) + 15;
        Quaternion rotation = Quaternion.EulerAngles(0, angle, 0);
        Val<Vector3> eggTarget = Val.V(() => egg.transform.position - new Vector3(0, egg.transform.position.y, 0) + (rotation * new Vector3(0, 0, 2)));
        return new DecoratorLoop(new Sequence(new Selector(new LeafAssert(playerIsNearEgg), new Sequence(ApproachAndOrientUnderTarget(guy, eggTarget, distance), mec(guy).Node_OrientTowards(egg.transform.position - new Vector3(0, egg.transform.position.y, 0)), new LeafWait(1000))),
                                              PrayToEgg(guy)));
    }

    protected Node PrayArc() {
        egg = GameObject.FindGameObjectWithTag("Egg");
        GameObject[] guys = GameObject.FindGameObjectsWithTag("Zealot");
        return new ForEach<GameObject>(PrayArcFactory, guys);
    }

    protected Node PrayToEgg(GameObject player)
    {
        IKController ikc = player.GetComponent<IKController>();
        return new Sequence(
            new LeafInvoke(delegate {ikc.StartPrayer(player.transform, GameObject.FindGameObjectWithTag("Egg").transform);})
            );
    }

    protected Wave[] getWaves()
    {
        GameObject[] guardPosts = GameObject.FindGameObjectsWithTag("GuardPost");
        Array.Sort(guardPosts, new WaveCompare());

        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
        Array.Sort(doors, new WaveCompare());

        Wave[] waveList = new Wave[Mathf.Min(guardPosts.Length, doors.Length)];
        for (int i = 0; i < waveList.Length; i++)
        {
            waveList[i] = new Wave(guardPosts[i], doors[i]);
        }
        return waveList;
    }

    protected class WaveCompare : IComparer
    {
        int IComparer.Compare(object a, object b)
        {
            GameObject waveA = (GameObject)a;
            GameObject waveB = (GameObject)b;
            if (waveA.transform.position.z > waveB.transform.position.z) return 1;
            else if (waveA.transform.position.z < waveB.transform.position.z) return -1;
            else return 0;
        }
    }

    protected GameObject[] getChildrenForWave(Wave wave, bool isGuards)
    {
        GameObject waveGameObject;
        string tag;
        if (isGuards)
        {
            waveGameObject = wave.guardPost;
            tag = "Guard";
        }
        else
        {
            waveGameObject = wave.doors;
            tag = "PinPad";
        }
        List<GameObject> list = new List<GameObject>();
        foreach (Transform child in waveGameObject.transform)
        {
            if (child.CompareTag(tag))
            {
                list.Add(child.gameObject);
            }
        }
        return list.ToArray();
    }

    protected class Wave
    {
        public GameObject guardPost;
        public GameObject doors;

        public Wave(GameObject guardPost, GameObject doors)
        {
            this.guardPost = guardPost;
            this.doors = doors;
        }
    }


}
