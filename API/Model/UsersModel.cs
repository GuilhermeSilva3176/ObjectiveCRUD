namespace API.Model;

public class UsersModel
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<EventsModel> Events { get; set; }

    public List<RegistrationModel> Registration { get; set; }
}
