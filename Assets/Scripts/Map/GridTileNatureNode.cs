
[System.Serializable]
public class GridTileNatureNode
{
    public int x;
    public int y;
    public string idNature;
    public bool isW;
    public string n; 

    public GridTileNatureNode(GridTileNode node, string _idNature, bool _isWalked, string _name)
    {
        x = node.x;
        y = node.y;
        n = _name;
        idNature = _idNature;
        isW = _isWalked;
    }

}