namespace API.Model;

public class RegistrationModel
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public Guid EventId { get; set; }

    public UsersModel Users { get; set; }

    public EventsModel Events { get; set; }

}
