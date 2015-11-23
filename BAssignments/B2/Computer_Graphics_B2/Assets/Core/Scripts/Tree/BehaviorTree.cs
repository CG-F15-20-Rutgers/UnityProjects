using UnityEngine;
using System.Collections;
using TreeSharpPlus;

public class BehaviorTree : MonoBehaviour {

	public GameObject follower1;
	public GameObject follower2;
	public GameObject leader;
	public Vector3 eyeHeight;
	public long gestureDuration;

	private BehaviorAgent behaviorAgent;

	// Use this for initialization
	void Start() {
		behaviorAgent = new BehaviorAgent(BuildTreeRoot());
		BehaviorManager.Instance.Register(behaviorAgent);
		behaviorAgent.StartBehavior();
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

	protected Node BuildTreeRoot() {
		return new Sequence(IntroTree());
	}

	protected Node IntroTree() {
		Val<Vector3> target = Val.V(() => (follower1.transform.position + follower2.transform.position) / 2);
		float distance = 1.0f;
		return new Sequence(ApproachAndOrient(follower1, follower2, target, distance), MaintainEyeContactWhileConversing(follower1, follower2, eyeHeight));
	}

	protected Node MaintainEyeContact(GameObject a, GameObject b, Vector3 eyeHeight) {
		Val<Vector3> p1Head = Val.V(() => a.transform.position + eyeHeight);
		Val<Vector3> p2Head = Val.V(() => b.transform.position + eyeHeight);
		return new DecoratorLoop(new SequenceParallel(mec(a).Node_HeadLook(p2Head), mec(b).Node_HeadLook(p1Head)));
	}

	protected Node Converse(GameObject a, GameObject b) {
		return new Sequence(Gesture(a, "WAVE"), Gesture(b, "CHEER"), Gesture(a, "CRY"), Gesture(b, "SHOCK"));
	}

	protected Node MaintainEyeContactWhileConversing(GameObject a, GameObject b, Vector3 eyeHeight) {
		return new Race(MaintainEyeContact(a, b, eyeHeight), Converse(a, b));
	}

	protected Node ApproachAndOrient(GameObject a, GameObject b, Val<Vector3> target, Val<float> distance) {
		Val<Vector3> p1 = Val.V(() => a.transform.position);
		Val<Vector3> p2 = Val.V(() => b.transform.position);
		return new Sequence(new SequenceParallel(mec(a).Node_GoToUpToRadius(target, distance),
		                                         mec(b).Node_GoToUpToRadius(target, distance)),
		                    new SequenceParallel(mec(a).Node_OrientTowards(p2),
		                     					 mec(b).Node_OrientTowards(p1)));
	}


}
