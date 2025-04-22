
public class Player
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public Player(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
