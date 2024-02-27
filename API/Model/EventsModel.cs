namespace API.Model;

public class EventsModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public string EventName { get; set; }

    public string EventDescription { get; set; }

    public DateTime EventDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public UsersModel User { get; set; }

    public List<RegistrationModel> Registration { get; set; }
}
