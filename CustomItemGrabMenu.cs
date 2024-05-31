using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;
using static StardewValley.Menus.ItemGrabMenu;

namespace NetworkChest
{


    public class CustomItemGrabMenu : ItemGrabMenu
    {
        private ClickableTextureComponent leftButton;
        private ClickableTextureComponent rightButton;

        public CustomItemGrabMenu(IList<Item> inventory, bool reverseGrab, bool showReceivingMenu, InventoryMenu.highlightThisItem highlightFunction,
        behaviorOnItemSelect behaviorOnItemSelectFunction, string message, behaviorOnItemSelect behaviorOnItemGrab = null, bool snapToBottom = false,
        bool canBeExitedWithKey = false, bool playRightClickSound = true, bool allowRightClick = true, bool showOrganizeButton = false, int source = 0,
        Item sourceItem = null, int whichSpecialButton = -1, object context = null, ItemExitBehavior heldItemExitBehavior = ItemExitBehavior.ReturnToPlayer,
        bool allowExitWithHeldItem = false)
        : base(inventory, reverseGrab, showReceivingMenu, highlightFunction, behaviorOnItemSelectFunction, message, behaviorOnItemGrab,
              snapToBottom, canBeExitedWithKey, playRightClickSound, allowRightClick, showOrganizeButton, source, sourceItem, whichSpecialButton,
              context, heldItemExitBehavior, allowExitWithHeldItem)
        {
            // Create arrow buttons
            this.leftButton = new ClickableTextureComponent(
                new Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 10, 48, 48),
                Game1.mouseCursors, new Rectangle(352, 495, 16, 16), 4f);
            this.rightButton = new ClickableTextureComponent(
                new Rectangle(this.xPositionOnScreen + this.width, this.yPositionOnScreen + 10, 48, 48),
                Game1.mouseCursors, new Rectangle(368, 495, 16, 16), 4f);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            // Draw arrow buttons
            this.leftButton.draw(b);
            this.rightButton.draw(b);
        }
    }
}
