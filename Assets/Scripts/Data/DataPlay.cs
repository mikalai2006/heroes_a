[System.Serializable]
public class DataPlay
{
    public DataLevel Level;
    public GameDate GameDate;
    public DataUnit entity;

    public DataPlay()
    {
        entity = new DataUnit();
    }
}
