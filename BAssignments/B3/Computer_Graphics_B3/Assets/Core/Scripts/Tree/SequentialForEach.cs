using System;
using System.Collections.Generic;
using TreeSharpPlus;

public class SequentialForEach<T> : NodeGroup {

    private static Node[] createChildren(Func<T, Node> subtreeFactory, IEnumerable<T> participants) {
        List<Node> nodes = new List<Node>();
        foreach (T obj in participants) {
            nodes.Add(subtreeFactory.Invoke(obj));
        }
        return nodes.ToArray();
    }

    public SequentialForEach(Func<T, Node> subtreeFactory, IEnumerable<T> participants) : base(createChildren(subtreeFactory, participants)) {
    }

    public override IEnumerable<RunStatus> Execute() {
        foreach (Node node in this.Children) {
            // Move on to the next node.
            this.Selection = node;
            node.Start();

            // We're running while the current one is running.
            RunStatus result;
            while ((result = this.TickNode(node)) == RunStatus.Running)
                yield return RunStatus.Running;

            // Properly end the current node.
            node.Stop();

            // Clear the selection.
            this.Selection.ClearLastStatus();
            this.Selection = null;

            if (result == RunStatus.Failure)
            {
                yield return RunStatus.Failure;
                yield break;
            }

            yield return RunStatus.Running;
        }
        yield return RunStatus.Success;
        yield break;
    }
}