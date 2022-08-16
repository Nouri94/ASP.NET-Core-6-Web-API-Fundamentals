namespace CityInfo.API.Services
{
    public class LocalMailService : IMailService
    {
        private readonly string _mailTo = string.Empty;
        private readonly string _mailFrom = string.Empty;
        public LocalMailService(IConfiguration config)
        {
            _mailTo = config["MailSettings:mailToAdress"];
            _mailFrom = config["MailSettings:mailFromAdress"];
        }
        public void Send(string subject, string msg)
        {
            Console.WriteLine($" Mail from {_mailFrom} to {_mailTo} with {nameof(LocalMailService)}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine($"Msg: {msg}");
        }
    }
}
