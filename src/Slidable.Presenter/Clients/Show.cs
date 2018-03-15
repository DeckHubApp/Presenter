using System;

namespace Slidable.Presenter.Clients
{
    public class Show
    {
        public string Presenter { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Place { get; set; }
        public int? HighestSlideShown { get; set; }
    }
}