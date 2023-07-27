namespace Rentopoly.Data;

public class Rental
{
    public int Id { get; set; }
    public string LoanedTo { get; set; } = "";
    public DateTime LoanedOn { get; set; }
    public DateTime? ReturnedOn { get; set; }
    public List<BoardGame> BoardGames { get; } = new();
}
