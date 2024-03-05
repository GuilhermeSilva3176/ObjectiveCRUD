namespace API.Model.DTOs.EventsDtos;

public class EventDto
{
    public Guid EventId { get; set; }

    public string EventName { get; set; }

    public string EventDescription { get; set; }

    public DateTime EventDate { get; set; }
}
