using TreeSharpPlus;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class UnPossessedNode : Node
{

    private Node child;
    private GameObject gameObject;

    protected Node lastTicked = null;

    public override Node LastTicked
    {
        get { return lastTicked; }
    }

    public UnPossessedNode(Node child, GameObject gameObject)
    {
        this.child = child;
        this.gameObject = gameObject;
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
        while(!CanExecute())
        {
            yield return RunStatus.Running;
        }

        lastTicked = child;
        child.Start();

        RunStatus result;
        while (!CanExecute() || (result = child.Tick()) == RunStatus.Running)
        {
            yield return RunStatus.Running;
        }
        child.Stop();

        yield return result;
        yield break;
    }

    private bool CanExecute()
    {
        PossessionScript script = gameObject.GetComponent<PossessionScript>();
        return script == null || script.IsTreeControlled();
    }
}
