﻿using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using Engine;

namespace SuperAdventureConsole
{
    public class Program
    {
        private const string PLAYER_DATA_FILE_NAME = "PlayerData.xml";
        private static Player _player;

        private static void Main(string[] args)
        {
            //load the game
            LoadGameData();

            Console.WriteLine("Type 'Help' to see a list of commands");
            Console.WriteLine(" ");

            DisplayCurrentLocation();

            // connect player events to functions that will display in the ui
            _player.PropertyChanged += Player_OnPropertyChanged;
            _player.OnMessage += Player_OnMessage;

            // infinite loop until the user types exit
            while (true)
            {
                //display a prompt so the user nkows to type something
                Console.Write(">");

                //wait for the user to type something and press the <enter> key
                string userInput = Console.ReadLine();
                // if they typed a blank line loop back and wait for input again
                if (string.IsNullOrWhiteSpace(userInput))
                {
                    continue;
                }

                //convert to lower-case to make comparison easier
                string cleanedInput = userInput.ToLower();

                //save the current game data and break out the while(true) loop
                if (cleanedInput == "exit")
                {
                    SaveGameData();
                    break;
                }

                //if the user typed something try to determine what to do
                ParseInput(cleanedInput);
            }
        }

        private static void Player_OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CurrentLocation")
            {
                DisplayCurrentLocation();

                if (_player.CurrentLocation.VendorWorkingHere != null)
                {
                    Console.WriteLine("You see a Vendor here : {0}", _player.CurrentLocation.VendorWorkingHere.Name);
                }
            }
        }

        private static void Player_OnMessage(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);
            if (e.AddExtraNewLine)
            {
                Console.WriteLine(" ");
            }
        }

        private static void ParseInput(string input)
        {
            if (input.Contains("help") || input == "?")
            {
                Console.WriteLine("Available commands");
                Console.WriteLine("==========================================");
                Console.WriteLine("Stats - Display player information");
                Console.WriteLine("Look - Get the description of your location");
                Console.WriteLine("Inventory - Display your inventory");
                Console.WriteLine("Quests - Display your quests");
                Console.WriteLine("Attack - Fight the monster");
                Console.WriteLine("Equip <weapon name> - Set your current weapon");
                Console.WriteLine("Drink <potion name> - Drink a potion");
                Console.WriteLine("Trade - Display your inventory and vendor's inventory");
                Console.WriteLine("Buy <item name> - Buy an item from the vendor");
                Console.WriteLine("Sell <item name> - Sell an item to a vendor");
                Console.WriteLine("North - Move North");
                Console.WriteLine("South - Move South");
                Console.WriteLine("East - Move East");
                Console.WriteLine("West - Move West");
                Console.WriteLine("Exit - Save the game and exit");
            }
            else if (input == "stats")
            {
                Console.WriteLine("Current hit points : {0}", _player.CurrentHitPoints);
                Console.WriteLine("Maximum hit points : {0}", _player.MaximumHitPoints);
                Console.WriteLine("Experience Points : {0}", _player.ExperiencePoints);
                Console.WriteLine("Level : {0}", _player.Level);
                Console.WriteLine("Gold : {0}", _player.Gold);
            }
            else if (input == "look")
            {
                DisplayCurrentLocation();
            }
            else if (input.Contains("north"))
            {
                if (_player.CurrentLocation.LocationToNorth == null)
                {
                    Console.WriteLine("You cannot move North");
                }
                else
                {
                    _player.MoveNorth();
                }
            }
            else if (input.Contains("east"))
            {
                if (_player.CurrentLocation.LocationToEast == null)
                {
                    Console.WriteLine("You cannot move East");
                }
                else
                {
                    _player.MoveEast();
                }
            }
            else if (input.Contains("south"))
            {
                if (_player.CurrentLocation.LocationToSouth == null)
                {
                    Console.WriteLine("You cannot move South");
                }
                else
                {
                    _player.MoveSouth();
                }
            }
            else if (input.Contains("west"))
            {
                if (_player.CurrentLocation.LocationToWest == null)
                {
                    Console.WriteLine("You Cannot move West");
                }
                else
                {
                    _player.MoveWest();
                }
            }
            else if (input == "inventory")
            {
                foreach (InventoryItem inventoryItem in _player.Inventory)
                {
                    Console.WriteLine("{0}: {1}",
                        inventoryItem.Description, inventoryItem.Quantity);
                }
            }
            else if (input == "quests")
            {
                if (_player.Quests.Count == 0)
                {
                    Console.WriteLine("You do not have any quests");
                }
                else
                {
                    foreach (PlayerQuest playerQuest in _player.Quests)
                    {
                        Console.WriteLine("{0}: {1}", playerQuest.Name,
                            playerQuest.IsCompleted ? "Completed" : "Incomplete");
                    }
                }
            }
            else if (input.Contains("attack"))
            {
                if (_player.CurrentLocation.MonsterLivingHere == null)
                {
                    Console.WriteLine("There is nothing here to attack");
                }
                else
                {
                    if (_player.CurrentWeapon == null)
                    {
                        // select the first weapon in the players inventory (or null if they do not have any weapons)
                        _player.CurrentWeapon = _player.Weapons.FirstOrDefault();
                    }

                    if (_player.CurrentWeapon == null)
                    {
                        Console.WriteLine("You do not have any weapons");
                    }
                    else
                    {
                        _player.UseWeapon(_player.CurrentWeapon);
                    }
                }
            }
            else if (input.StartsWith("equip"))
            {
                string inputWeaponName = input.Substring(6).Trim();
                if (string.IsNullOrEmpty(inputWeaponName))
                {
                    Console.WriteLine("You must enter the name of the weapon to equip");
                }
                else
                {
                    Weapon weaponToEquip = _player.Weapons.SingleOrDefault(x =>
                        x.Name.ToLower() == inputWeaponName || x.NamePlural.ToLower() == inputWeaponName);
                    if (weaponToEquip == null)
                    {
                        Console.WriteLine("You do not have the weapon : {0}", inputWeaponName);
                    }
                    else
                    {
                        _player.CurrentWeapon = weaponToEquip;
                        Console.WriteLine("You equip your {0}", _player.CurrentWeapon.Name);
                    }
                }
            }
            else if (input.StartsWith("drink "))
            {
                string inputPotionName = input.Substring(6).Trim();
                if (string.IsNullOrEmpty(inputPotionName))
                {
                    Console.WriteLine("You must enter the name of the potion to drink");
                }
                else
                {
                    HealingPotion potionToDrink = _player.Potions.SingleOrDefault(x =>
                        x.Name.ToLower() == inputPotionName || x.NamePlural.ToLower() == inputPotionName);
                    if (potionToDrink == null)
                    {
                        Console.WriteLine("You do not have the potion : {0}", inputPotionName);
                    }
                    else
                    {
                        _player.UsePotion(potionToDrink);
                    }
                }
            }
            else if (input == "trade")
            {
                if (_player.CurrentLocation.VendorWorkingHere == null)
                {
                    Console.WriteLine("There is no vendor here");
                }
                else
                {
                    Console.WriteLine("PLAYER INVENTORY");
                    Console.WriteLine("================");
                    if (_player.Inventory.Count(x => x.Price != World.UNSELLABLE_ITEM_PRICE) == 0)
                    {
                        Console.WriteLine("You do not have any items in your inventory");
                    }
                    else
                    {
                        foreach (InventoryItem inventoryItem in _player.Inventory.Where(x =>
                            x.Price != World.UNSELLABLE_ITEM_PRICE))
                        {
                            Console.WriteLine("{0} {1} Price: {2}", inventoryItem.Quantity, inventoryItem.Description,
                                inventoryItem.Price);
                        }
                    }

                    Console.WriteLine(" ");
                    Console.WriteLine("VENDOR INVENTORY");
                    Console.WriteLine("================");
                    if (_player.CurrentLocation.VendorWorkingHere.Inventory.Count == 0)
                    {
                        Console.WriteLine("The vendor does not have any items in their inventory");
                    }
                    else
                    {
                        foreach (InventoryItem inventoryItem in _player.CurrentLocation.VendorWorkingHere.Inventory)
                        {
                            Console.WriteLine("{0} {1} Price {2}", inventoryItem.Quantity, inventoryItem.Description,
                                inventoryItem.Price);
                        }
                    }
                }
            }
            else if (input.StartsWith("buy "))
            {
                if (_player.CurrentLocation.VendorWorkingHere == null)
                {
                    Console.WriteLine("There is no vendor in this location");
                }
                else
                {
                    string itemName = input.Substring(4).Trim();
                    if (string.IsNullOrEmpty(itemName))
                    {
                        Console.WriteLine("You must enter the name of the item to buy");
                    }
                    else
                    {
                        //get the InventoryItem from the traders inventory
                        InventoryItem itemToBuy =
                            _player.CurrentLocation.VendorWorkingHere.Inventory.SingleOrDefault(x =>
                                x.Details.Name.ToLower() == itemName);
                        //check if the vendor has the item
                        if (itemToBuy == null)
                        {
                            Console.WriteLine("The vendor does not have any{0}", itemName);
                        }
                        else
                        {
                            //check if the player has enough gold to buy the item
                            if (_player.Gold < itemToBuy.Price)
                            {
                                Console.WriteLine("You do not have enough gold to buy a {0}", itemToBuy.Description);
                            }
                            else
                            {
                                // succes! buy the item
                                _player.AddItemToInventory(itemToBuy.Details);
                                _player.Gold -= itemToBuy.Price;
                                Console.WriteLine("You bought one {0} for {1} gold", itemToBuy.Details.Name,
                                    itemToBuy.Price);
                            }
                        }
                    }
                }
            }
            else if (input.StartsWith("sell "))
            {
                if (_player.CurrentLocation.VendorWorkingHere == null)
                {
                    Console.WriteLine("There is no vendor at this location");
                }
                else
                {
                    string itemName = input.Substring(5).Trim();
                    if (string.IsNullOrEmpty(itemName))
                    {
                        Console.WriteLine("You must enter the name of the item to sell");
                    }

                    else
                    {
                        //get the IventoryItem from the player's inventory
                        InventoryItem itemToSell = _player.Inventory.SingleOrDefault(x =>
                            x.Details.Name.ToLower() == itemName &&
                            x.Quantity > 0 &&
                            x.Price != World.UNSELLABLE_ITEM_PRICE);
                        //check if the player has the item entered
                        if (itemToSell == null)
                        {
                            Console.WriteLine("The player cannot sell any{0}", itemName);
                        }
                        else
                        {
                            //sell the item
                            _player.RemoveItemFromInventory(itemToSell.Details);
                            _player.Gold += itemToSell.Price;

                            Console.WriteLine("You receive {0} gold for your {1}", itemToSell.Price,
                                itemToSell.Details.Name);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("I do not understand");
                Console.WriteLine("Type 'help' to see the list of available commands");
            }

            //write a blank like to keep the ui a little clean
            Console.WriteLine(" ");
        }

        private static void DisplayCurrentLocation()
        {
            Console.WriteLine("You are at {0}", _player.CurrentLocation.Name);
            if (_player.CurrentLocation.Description != " ")
            {
                Console.WriteLine(_player.CurrentLocation.Description);
            }
        }

        private static void LoadGameData()
        {
            _player = PlayerDataMapper.CreateFromDataBase();
            if (_player == null)
            {
                if (File.Exists(PLAYER_DATA_FILE_NAME))
                {
                    _player = Player.CreatePlayerFromXmlString(File.ReadAllText(PLAYER_DATA_FILE_NAME));
                }
                else
                {
                    _player = Player.CreateDefaultPlayer();
                }
            }
        }

        private static void SaveGameData()
        {
            File.WriteAllText(PLAYER_DATA_FILE_NAME, _player.ToXmlString());
            PlayerDataMapper.SaveToDatabase(_player);
        }
    }
}