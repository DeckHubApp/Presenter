using JetBrains.Annotations;
using MessagePack;

namespace Slidable.Presenter.Models
{
    [MessagePackObject]
    public class SlideAvailable
    {
        [Key(0)]
        public string Presenter { get; [UsedImplicitly] set; }
        [Key(1)]
        public string Slug { get; [UsedImplicitly] set; }
        [Key(2)]
        public int Number { get; [UsedImplicitly] set; }
    }
}