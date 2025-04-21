public static class TutorialConsts
{
    //Path Selection Tutorial Text
    public const string PlanningPhaseText = "Before entering the dungeon, you first need to plan a path through the current floor. Left click on these popups to continue the tutorial.";
    public const string StartRoomText = "The ransacker starts in the room with the green dot. When you reach the room with the stair symbol you will move to the next floor of the dungeon.";
    public const string EnemyRoomText = "Rooms with an E symbol contains enemies you will have to defeat to pass through.";
    public const string EnemyPreviewText = "By clicking on an enemy or boss room you can see a visual of what enemies are in the room in order from most present to least. Hovering over an enemy in the preview will bring up a tooltip with the enemy's name and what parts you can target on them.";
    public const string TreasureRoomText = "Rooms with a treasure chest symbol have a loot chest which you can open to obtain new items.";
    public const string ShopRoomText = "Rooms with the coin symbol contain a shop where you can sell loot for gold and buy or trade for new items.";
    public const string PathSelectionText = "By left clicking and dragging your mouse starting from the starting room or where you last left off your path, you set the path of rooms the ransacker will traverse through. You can also undo your existing path by dragging backwards from the end of the path.";
    public const string LimitedMovesText = "The ransacker only has a limited number of rooms they can enter as shown above the enemy preview. The path you select must end on either the stair or boss room.";
    public const string BossRoomText = "At the last floor of the dungeon instead of the stair room will be the boss room. Bosses are much tougher to kill than normal enemies and have additional mechanics that have to be dealt with. Overcoming these mechanics allows you to deal more damage to the boss.";
    public const string RansackButtonText = "Once you've decided what path the ransacker will take through this floor, you can enter the dungeon by clicking this button below.";

    //Gameplay Tutorial Text
    public const string InventoryText = "This is the your main inventory where you can hold any items you receive from enemies. Items can be rotated by right clicking while holding it.";
    public const string HotbarText = "The secondary inventory beneath it is your hotbar. Any items you place inside of it will be used or equipped when you're in combat.";
    public const string EquipmentText = "Below is where your currently equipped items are displayed.";

    public const string CombatText = "Combat in Ransacker is in real time, with you and every enemy having a timer to indicate how long until the next time they attack.";
    public const string AttackTimerText = "Whenever your attack timer hits zero, the ransacker will hit the targeted enemy and then use or equip all the items in the hotbar.";
    public const string TargetingText = "To select which enemy you want to attack, click on a part of the enemy that is shimmering. Some enemies may have multiple parts that can be targeted. ";
    public const string WeakpointText = "Some targetable parts also have a weakness to a specific type of damage. A weapon's damage type is indicated at the top right of it's tooltip.";
    public const string WeaknessText = "If you deal a certain percent of the enemy's health on that part using that damage type, it will trigger a powerful effect on the enemy, such as an execute.";
    public const string LootQueueText = "In the top left is the loot queue. If your inventory is full, any loot that drops while still in combat will be placed in the queue.";
    public const string LootQueueText2 = "At the end of your attack timer, the loot queue will try to place the currently displayed item into your inventory if there is enough space.";

    public const string LootChestText = "Open loot chests by left clicking on them";
    public const string LootInventoryText = "Opening a chest while your inventory is full or when there are items in still in the loot queue at the end of a battle, will display the loot window. This window also doubles as a trash can for any items you leave inside. Press Escape or click leave room to exit the window.";
    
    public const string ShopGameplayText = "The shop sells three items you can buy with gold or trade for with your own items. The tooltip for items in the shop show their price and the criteria for the item you can trade for it.";
    public const string BarterOrBuyText = "Left click with no item held to buy the item with gold or trade for it by clicking with an item that meets the requirements listed.";
    public const string ShopItemTooltipText = "Holding shift while hovering a shop item will change the tooltip to the normal tooltip where you can view the stats of the respective item.";
    public const string SellingItemsText = "Left click on the merchant with an item to sell it.";
    public const string GoldHUDText = "The gold you currently have is displayed in the top right corner.";

    //Weapon Mechanics Tutorial Text
    public const string SwordText = "Swords deal slashing damage and have an above average attack speed. A shield can also be equipped when using a sword.";
    public const string SwordAndShieldText = "Shields can block a certain amount of damage as well as parry attacks, but can only be equipped when using a sword.";
    public const string SwordAndShieldText2 = "Hold down the right mouse button to block. When blocking you take 50% less damage but will build up blocking fatigue displayed on the off hand equip slot. If block fatigue bar is filled, your shield will go on a cool down and can't be used.";
    public const string SwordAndShieldText3 = "To parry an attack, release your block within half a second of an incoming attack. Performing a successful parry will stun every enemy that attacked within that time frame.";
    public const string DaggerText = "Daggers typically deal piercing damage and have a fast attack speed. Equipping two daggers will let the Ransacker attack twice when their timer runs down.";
    public const string AxeText = "Axes deal slashing damage and have an average attack speed. Attacks with an axe will hit the enemies adjacent to your current target";
    public const string HammerText = "Hammers deal blunt damage and have the slowest attack speed. Attacks will build up stacks on your current target and once 3 stacks are reached the enemy will get stunned.";
}


