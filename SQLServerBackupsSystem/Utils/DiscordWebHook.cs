namespace SQLServerBackupsSystem.Utils
{
    public class DiscordWebHook
    {
        public static void SendMessage(string Message)
        {
            if (string.IsNullOrWhiteSpace(Program.DiscordWebHook_NotificationsURL)) return;

            var Request = new RequestBuilder(Program.DiscordWebHook_NotificationsURL).POST()
                .AddPOSTParam("username", System.Environment.MachineName)
                .AddPOSTParam("content", Message)
                .Execute();
        }
    }
}
