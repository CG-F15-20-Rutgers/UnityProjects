using TreeSharpPlus;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PossessionNode : Node
{

    private Node child;
    private GameObject gameObject;
    private bool needsToBePossessed;

    protected Node lastTicked = null;

    public override Node LastTicked
    {
        get { return lastTicked; }
    }

    public PossessionNode(Node child, GameObject gameObject, bool needsToBePossessed)
    {
        this.child = child;
        this.gameObject = gameObject;
        this.needsToBePossessed = needsToBePossessed;
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
        child.ClearLastStatus();
    }

    public override IEnumerable<RunStatus> Execute()
    {
        while(true)
        {
            if (IsObjectDismissed())
            {
                yield return RunStatus.Success;
                yield break;
            } else if (!CanExecuteForPossession())
            {
                yield return RunStatus.Running;
            } else
            {
                break;
            }
        }

        lastTicked = child;
        child.Start();

        RunStatus result;
        while(true)
        {
            if (!CanExecuteForPossession())
            {
                if (IsObjectDismissed())
                {
                    result = RunStatus.Success;
                    break;
                }
                yield return RunStatus.Running;
            }
            else
            {
                if ((result = child.Tick()) == RunStatus.Running) {
                    yield return RunStatus.Running;
                } else
                {
                    break;
                }
            }
        }
        child.Stop();

        yield return result;
        yield break;
    }

    private bool IsObjectDismissed()
    {
        PossessionScript script = gameObject.GetComponent<PossessionScript>();
        return (script != null && script.IsDismissed);
    }

    private bool CanExecuteForPossession()
    {
        PossessionScript script = gameObject.GetComponent<PossessionScript>();
        bool possessed = (script != null && script.IsPossessed);
        return (possessed == needsToBePossessed);
    }
}
