namespace API.Model.DTOs.RegistrationDtos;

public class RegistrationDto
{
    public Guid Id { get; set; }

    public string UserName { get; set; }

    public string EventName { get; set; }

    public DateTime EventDate { get; set; }
}
