namespace PetMatchMobile.Models
{
    public class NotificationModel
    {
        public string AnimalName { get; set; }
        public string Status { get; set; }  // "Aprobat" sau "Respins"
        public string Message { get; set; } // "Vino să îl iei mâine!"
    }
}