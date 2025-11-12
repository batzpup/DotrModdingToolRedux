namespace DotrModdingTool2IMGUI.ChangelogDiffCheckers;

public class DeckSnapshot
{
    public List<Deck> Decks;

    public DeckSnapshot(List<Deck> decks)
    {
        Decks = decks.Select(d => CloneDeck(d)).ToList();
    }

    Deck CloneDeck(Deck original)
    {
        Deck clone = new Deck {
            DeckLeader = new DeckCard(original.DeckLeader.CardConstant, original.DeckLeader.Rank),
            CardList = original.CardList
                .Select(c => new DeckCard(c.CardConstant, c.Rank))
                .ToList()
        };
        return clone;
    }
}

public class DeckDiffChecker : IDiffChecker<DeckSnapshot>
{
    public DiffResult CompareSnapshots(DeckSnapshot oldSnap, DeckSnapshot currentSnapshot)
    {
        DiffResult result = new DiffResult { Name = "Decks" };
        for (int i = 0; i < currentSnapshot.Decks.Count; i++)
        {
            var oldDeck = oldSnap.Decks[i];
            var newDeck = currentSnapshot.Decks[i];
            string title = $"{Deck.NamePrefix(i)} - {newDeck.DeckLeader.Name.Current}'s deck changes:";
            if (oldDeck.DeckLeader.CardConstant.Index != newDeck.DeckLeader.CardConstant.Index)
            {
                result.Add(title, $"Leader: {oldDeck.DeckLeader.CardConstant.Name} → {newDeck.DeckLeader.CardConstant.Name}");
            }
            if (oldDeck.DeckLeader.Rank != newDeck.DeckLeader.Rank)
            {
                result.Add(title, $"Leader Rank: {oldDeck.DeckLeader.Rank} → {newDeck.DeckLeader.Rank}");
            }
            //If deck has changed
            if (!oldDeck.Bytes.SequenceEqual(newDeck.Bytes))
            {
                var oldCounts = oldDeck.CardList
                    .GroupBy(c => c.CardConstant.Index)
                    .ToDictionary(g => g.Key, g => new { Name = g.First().CardConstant.Name, Count = g.Count() });

                var newCounts = newDeck.CardList
                    .GroupBy(c => c.CardConstant.Index)
                    .ToDictionary(g => g.Key, g => new { Name = g.First().CardConstant.Name, Count = g.Count() });

                var allIds = oldCounts.Keys.Union(newCounts.Keys);

                var added = new List<string>();
                var removed = new List<string>();

                foreach (var id in allIds)
                {
                    var oldCnt = oldCounts.TryGetValue(id, out var o) ? o.Count : 0;
                    var newCnt = newCounts.TryGetValue(id, out var n) ? n.Count : 0;
                    var delta = newCnt - oldCnt;

                    if (delta > 0)
                    {
                        var name = newCounts[id].Name;
                        added.Add($"{delta}× {name}");
                    }
                    else if (delta < 0)
                    {
                        var name = oldCounts[id].Name;
                        removed.Add($"{-delta}× {name}");
                    }
                }

                if (removed.Count > 0)
                {
                    result.Add(title, "Removed:\n    " + string.Join("\n    ", removed.OrderBy(s => s)));
                }
                if (added.Count > 0)
                {
                    result.Add(title, "Added:\n    " + string.Join("\n    ", added.OrderBy(s => s)));
                }
            }

        }

        return result;
    }
}