using System;
using MessagePack;

namespace DeckHub.Presenter.Models
{
    [MessagePackObject]
    public class StartShow
    {
        [Key(0)]
        public string Place { get; set; }
        [Key(1)]
        public string Presenter { get; set; }
        [Key(2)]
        public string Slug { get; set; }
        [Key(3)]
        public string Title { get; set; }
        [Key(4)]
        public DateTimeOffset Time { get; set; }
    }
}