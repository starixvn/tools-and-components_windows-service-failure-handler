namespace WindowsServiceFailureHandler.Models
{
    using System;

    public class FailureDetail
    {
        public DateTime LoggedTime { get; set; }

        public string ServiceName { get; set; }

        public string ErrorMessage { get; set; }
    }
}
