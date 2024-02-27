namespace API.Model.DTOs.EventsDtos;

public class CreateEventsDto
{
    public string EventName { get; set; }

    public string EventDescription { get; set; }

    public DateTime EventDate { get; set; }

}
