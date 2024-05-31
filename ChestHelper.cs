using StardewModdingAPI;
using StardewValley.Objects;
using StardewValley;

namespace NetworkChest
{
    internal class ChestHelper
    {
        public static void ListChests(IMonitor monitor)
        {
            if (!Context.IsWorldReady)
            {
                monitor.Log("World is not ready.", LogLevel.Warn);
                return;
            }

            var farm = Game1.getFarm();
            List<Chest> chests = GetAllChestsOnFarm(farm);

            if (chests.Count == 0)
            {
                monitor.Log("No chests found on the farm.", LogLevel.Info);
                return;
            }

            Dictionary<Item, int> aggregatedItems = AggregateChestItems(chests);

            string itemInfo = "Aggregated chest items on the farm:\n";
            foreach (var item in aggregatedItems)
            {
                itemInfo += $"{item.Key.Name} x {item.Value}\n";
            }

            monitor.Log(itemInfo, LogLevel.Info);
            Game1.addHUDMessage(new HUDMessage("Check SMAPI console for aggregated chest items.", HUDMessage.newQuest_type));
        }

        public static Dictionary<Item, int> AggregateChestItems(List<Chest> chests)
        {
            Dictionary<Item, int> itemDictionary = new Dictionary<Item, int>();

            foreach (var chest in chests)
            {
                foreach (var item in chest.Items)
                {
                    if (item == null) continue;

                    if (itemDictionary.ContainsKey(item))
                    {
                        itemDictionary[item] += item.Stack;
                    }
                    else
                    {
                        itemDictionary[item] = item.Stack;
                    }
                }
            }

            return itemDictionary;
        }

        public static List<Chest> GetAllChestsOnFarm(GameLocation farm)
        {
            List<Chest> chests = new List<Chest>();

            foreach (var obj in farm.objects.Values)
            {
                if (obj is Chest chest)
                {
                    chests.Add(chest);
                }
            }

            return chests;
        }

    }
}
