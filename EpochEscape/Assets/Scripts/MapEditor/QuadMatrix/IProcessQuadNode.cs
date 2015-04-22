public interface IProcessQuadNode<T> where T : QuadMatrix<T>.Node.NodeData
{
    void ProcessQuadNode(QuadMatrix<T>.Node quadNode);
}
