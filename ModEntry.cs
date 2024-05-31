using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace NetworkChest
{
    public class ReadChest : Mod
    {
        internal Chest Chest;
        internal int Page = 0;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            helper.Events.Input.MouseWheelScrolled += this.OnMouseWheelScrolled;

            helper.ConsoleCommands.Add("list_chests", "Lists all chests on the farm.", this.ListChestsCommand);
        }

        private void ListChestsCommand(string command, string[] args)
        {
            ChestHelper.ListChests(this.Monitor);
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsWorldReady && e.Button == SButton.P)
            {
                Page = 0;
                ShowAggregatedChest();
            }
            if (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu)
            {
                if (Context.IsWorldReady && e.Button == SButton.Right)
                {
                    Page++;
                    ShowAggregatedChest();
                }
                if (Context.IsWorldReady && e.Button == SButton.Left)
                {
                    if (Page > 0)
                    {
                        Page--;
                        ShowAggregatedChest();
                    }
                }
            }
        }

        private void OnMouseWheelScrolled(object sender, MouseWheelScrolledEventArgs e)
        {
            if (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu)
            {
                if (Context.IsWorldReady && e.Delta < 0)
                {
                    Page++;
                    ShowAggregatedChest();
                }
                if (Context.IsWorldReady && e.Delta > 0)
                {
                    if (Page > 0)
                    {
                        Page--;
                        ShowAggregatedChest();
                    }
                }
            }
        }

        private void ShowAggregatedChest()
        {
            var farm = Game1.getFarm();
            List<Chest> chests = ChestHelper.GetAllChestsOnFarm(farm);

            if (chests.Count == 0)
            {
                this.Monitor.Log("No chests found on the farm.", LogLevel.Info);
                Game1.addHUDMessage(new HUDMessage("No chests found on the farm.", HUDMessage.error_type));
                return;
            }

            Dictionary<Item, int> itemDictionary = ChestHelper.AggregateChestItems(chests);

            this.Chest = CreateAggregatedChest(itemDictionary);
            OpenChest(this.Chest);
        }

        private int convertCategorySorting(int value)
        {
            if (value <= -95)
            {
                return 1;
            }
            else if (value == -4)
            {
                return -73;
            }
            else
            {
                return value;
            }
        }

        private Chest CreateAggregatedChest(Dictionary<Item, int> itemDictionary)
        {
            var sorted = itemDictionary
                .OrderBy(pair => convertCategorySorting(pair.Key.Category));

            Chest chest = new Chest(true);
            chest.SpecialChestType = Chest.SpecialChestTypes.BigChest;

            var amount = 70;
            var items = sorted.Skip(Page * amount).Take(amount + 1);
          

            foreach (var item in items)
            {
                // this.Monitor.Log(item.Key.Name + " : " + item.Key.Category, LogLevel.Info);
                chest.addItem(item.Key);
            }

            return chest;
        }

        private void OpenChest(Chest chest)
        {
            Game1.activeClickableMenu = new ItemGrabMenu(
                    inventory: chest.Items,
                    reverseGrab: false,
                    showReceivingMenu: true,
                    highlightFunction: this.CanAcceptItem,
                    behaviorOnItemSelectFunction: this.GrabItemFromPlayer,
                    message: null,
                    behaviorOnItemGrab: this.GrabItemFromContainer,
                    canBeExitedWithKey: true,
                    showOrganizeButton: false,
                    source: ItemGrabMenu.source_chest,
                    sourceItem: chest,
                    context: null
                );
        }

        private void GrabItemFromPlayer(Item item, Farmer player)
        {
            this.Chest.grabItemFromInventory(item, player);
            this.OnChanged();
        }

        private void GrabItemFromContainer(Item item, Farmer player)
        {
            this.Chest.grabItemFromChest(item, player);
            DeleteItemFromChests(item);
            this.OnChanged();
        }

        private void DeleteItemFromChests(Item removeItem)
        {

            var farm = Game1.getFarm();
            List<Chest> chests = ChestHelper.GetAllChestsOnFarm(farm);

            var itemName = removeItem.Name;
            var quantity = removeItem.Stack;

            foreach (var chest in chests)
            {
                foreach (var item in chest.Items)
                {
                    if (item.Name == itemName)
                    {
                        if (item.Stack <= quantity)
                        {
                            chest.Items.Remove(item);
                            quantity -= item.Stack;
                            this.Monitor.Log($"Removed {item.stack} {itemName} from the chest at ({chest.TileLocation.X}, {chest.TileLocation.Y}).", LogLevel.Info);
                            continue;
                        }
                        else
                        {
                            //item.Stack -= quantity;
                            this.Monitor.Log($"Took {quantity} {itemName} from the chest at ({chest.TileLocation.X}, {chest.TileLocation.Y}).", LogLevel.Info);
                            return;
                        }
                    }
                }
            }
        }

        protected virtual void OnChanged()
        {
            if (Game1.activeClickableMenu is ItemGrabMenu itemGrabMenu)
            {
                itemGrabMenu.behaviorOnItemGrab = this.GrabItemFromContainer;
                itemGrabMenu.behaviorFunction = this.GrabItemFromPlayer;
            }
        }
        public bool CanAcceptItem(Item item)
        {
            return InventoryMenu.highlightAllItems(item);
        }
    }
}

