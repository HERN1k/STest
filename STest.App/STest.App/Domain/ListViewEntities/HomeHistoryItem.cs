using System;

namespace STest.App.Domain.ListViewEntities
{
    public class HomeHistoryItem(DateTime date, int questions, int assessment, string teacher)
    {
        public DateTime Date { get; set; } = date;
        public int Questions { get; set; } = questions;
        public int Assessment { get; set; } = assessment;
        public string Teacher { get; set; } = teacher;

        public string DateString => Date.ToString("yyyy-MM-dd");
    }
}