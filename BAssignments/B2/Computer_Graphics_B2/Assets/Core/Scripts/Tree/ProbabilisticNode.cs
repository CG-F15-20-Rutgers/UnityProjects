using TreeSharpPlus;
using System.Collections;
using System;
using System.Collections.Generic;

public class ProbabilisticNode : Node {

    private static Random RANDOM = new Random();

    private double probability;
    private Node child;

    protected Node lastTicked = null;

    public override Node LastTicked {
        get { return lastTicked; }
    }

    public ProbabilisticNode(Node child, double probability) {
        this.child = child;
        this.probability = probability;
    }

    public override void Stop() {
        if (lastTicked != null) {
            lastTicked.Stop();
        }
        base.Stop();
    }

    public override RunStatus Terminate() {
        RunStatus curStatus = StartTermination();
        if (curStatus != RunStatus.Running)
            return curStatus;

        // If we have a node active, terminate it.
        if (lastTicked != null) {
            return ReturnTermination(lastTicked.Terminate());
        }
        return ReturnTermination(RunStatus.Success);
    }

    public override void ClearLastStatus() {
        this.LastStatus = null;
        child.ClearLastStatus();
    }

    public override IEnumerable<RunStatus> Execute() {
        if (RANDOM.NextDouble() <= probability) {
            lastTicked = child;

            child.Start();
            RunStatus result;
            while ((result = child.Tick()) == RunStatus.Running) {
                yield return RunStatus.Running;
            }
            child.Stop();

            yield return result;
            yield break;
        }
        lastTicked = null;
        yield return RunStatus.Success;
        yield break;
    }
}
