namespace TD.WebApi.Application.Common.ZoomMeeting;

public class CreateZoomMeetingTokenRequest
{
    public int Role { get; set; }
    public string MeetingNumber { get; set; }
}