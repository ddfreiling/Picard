﻿using Newtonsoft.Json.Linq;

namespace Picard.Lib.JournalEntry
{
    public class MaterialDiscarded : EliteJournalEntry
    {
        public string Category { get; internal set; }
        public string Name { get; internal set; }
        public int Count { get; internal set; }

        public MaterialDiscarded(JObject json) : base(json) { }
    }
}
