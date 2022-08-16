namespace CityInfo.API.Services
{
    public class CloudMailservice : IMailService
    {
        private string _mailTo = "admin@hotmail.com";
        private string _mailFrom = "noreplay@hotmail.com";
        public void Send(string subject, string msg)
        {
            Console.WriteLine($" Mail from {_mailFrom} to {_mailTo} with {nameof(CloudMailservice)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Msg: {msg}");
        }
    }
}
