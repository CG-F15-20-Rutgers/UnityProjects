using TreeSharpPlus;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PossessionCoordinatorNode : Node
{

    private Node transitionToPossessionNode;
    private Node transitionToNoPossessionNode;
    private Node possessionNodes;
    private GameObject gameObject;

    protected Node lastTicked = null;

    public override Node LastTicked
    {
        get { return lastTicked; }
    }

    public PossessionCoordinatorNode(GameObject gameObject, Node possession, Node noPossession, Node transitionToPossession, Node transitionFromPossession)
    {
        this.gameObject = gameObject;
        this.transitionToPossessionNode = transitionToPossession;
        this.transitionToNoPossessionNode = transitionFromPossession;
        this.possessionNodes = new SequenceParallel(new PossessionNode(noPossession, gameObject, false),
                                                    new PossessionNode(possession, gameObject, true));
    }

    public override void Stop()
    {
        if (lastTicked != null)
        {
            lastTicked.Stop();
        }
        base.Stop();
    }

    public override RunStatus Terminate()
    {
        RunStatus curStatus = StartTermination();
        if (curStatus != RunStatus.Running)
            return curStatus;

        // If we have a node active, terminate it.
        if (lastTicked != null)
        {
            return ReturnTermination(lastTicked.Terminate());
        }
        return ReturnTermination(RunStatus.Success);
    }

    public override void ClearLastStatus()
    {
        this.LastStatus = null;
        transitionToPossessionNode.ClearLastStatus();
        transitionToNoPossessionNode.ClearLastStatus();
        possessionNodes.ClearLastStatus();
    }

    public override IEnumerable<RunStatus> Execute()
    {
        possessionNodes.Start();

        bool lastPossession = IsPossessed();
        RunStatus retStatus;
        while (true) {
            //if (IsObjectDismissed()) {
            //    retStatus = RunStatus.Success; break;
            //}

            // See which node we should evaluate. If a transition has occurred, we evaluate the respective transition node.
            // Otherwise, we evaluate the last ticked transition node or the possessionNode.
            bool currentPossession = IsPossessed();
            Node toEvaluate;
            if (currentPossession != lastPossession) {
                toEvaluate = (currentPossession) ? transitionToPossessionNode : transitionToNoPossessionNode;
            } else {
                toEvaluate = (lastTicked == null) ? possessionNodes : lastTicked;
            }

            if (toEvaluate != lastTicked) {
                // Terminate previous transition nodes.
                if (lastTicked != null && IsTransitionNode(lastTicked)) {
                    lastTicked.Terminate();
                }
                // Start current transition nodes.
                if (IsTransitionNode(toEvaluate)) {
                    toEvaluate.Start();
                }

                // Alright, we swap to the new node by storing it in lastTicked.
                lastTicked = toEvaluate;
            } else {
                RunStatus status = toEvaluate.Tick();
                if (status == RunStatus.Running) {
                    yield return RunStatus.Running;
                } else if (status == RunStatus.Success && IsTransitionNode(toEvaluate)) {
                    toEvaluate.Stop();
                    lastTicked = null;
                    yield return RunStatus.Running;
                } else {
                    retStatus = status; break;
                }
            }
            lastPossession = currentPossession;
        }

        possessionNodes.Stop();
        yield return retStatus;
        yield break;
    }

    private bool IsTransitionNode(Node n)
    {
        return (n == transitionToPossessionNode || n == transitionToNoPossessionNode);
    }

    private bool IsObjectDismissed()
    {
        PossessionScript script = gameObject.GetComponent<PossessionScript>();
        return (script != null && script.IsDismissed);
    }

    private bool IsPossessed()
    {
        PossessionScript script = gameObject.GetComponent<PossessionScript>();
        return (script != null && script.IsPossessed);
    }
}
