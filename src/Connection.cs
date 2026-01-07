public class Connection
{
    private int id;
    private BoardItem dep;
    private BoardItem fin;
    private string    style;
    
    // Constructor
    public Connection(int id, BoardItem dep, BoardItem fin, string style)
	{
		this.id = id;
		this.dep = dep;
		this.fin = fin;
		this.style = style;
	}
}
