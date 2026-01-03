using System;

namespace DocumentHub.ViewModel
{
    public class WorkProgress
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? NotificationDate { get; set; }
        public Person Assigner { get; set; }
        public Person PersonInCharge { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public int Progress { get; set; }
        public DateTime? ActualCompletionDate { get; set; }

        public bool IsMonth { get; set; }
        public bool Is3Months { get; set; }
        public bool Is6Months { get; set; }
        public bool Is9Months { get; set; }
        public bool IsYear { get; set; }
    }


    public class Person
    {
        public string FullName { get; set; }
    }
}
