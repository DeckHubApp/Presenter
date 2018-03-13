using System;

namespace Slidable.Presenter.Models
{
    public class PresenterViewModel
    {
    }
    
    public class StartShow
    {
        public string Presenter { get; set; }
        public string Slug { get; set; }
        public string Title { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Place { get; set; }
    }
}