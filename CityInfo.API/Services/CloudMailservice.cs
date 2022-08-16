namespace CityInfo.API.Services
{
    public class CloudMailservice : IMailService
    {
        private readonly string _mailTo = string.Empty;
        private readonly string _mailFrom = string.Empty;
        public CloudMailservice(IConfiguration config)
        {
            _mailFrom = config["MailSettings:mailToAdress"];
            _mailFrom = config["MailSettings:mailFromAdress"];
        }
        public void Send(string subject, string msg)
        {
            Console.WriteLine($" Mail from {_mailFrom} to {_mailTo} with {nameof(CloudMailservice)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Msg: {msg}");
        }
    }
}
