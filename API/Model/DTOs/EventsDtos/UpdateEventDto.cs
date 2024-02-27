namespace API.Model.DTOs.EventsDtos;

public class UpdateEventDto
{
    public Guid Id { get; set; }

    public string EventName { get; set; }

    public string EventDescription { get; set; }

    public DateTime EventDate { get; set; }

}