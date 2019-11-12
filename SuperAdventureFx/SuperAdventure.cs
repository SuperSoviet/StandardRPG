using Engine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SuperAdventureFx {
  public partial class SuperAdventure : Form {
    private Player _player;
    private Monster _currentMonster;

    public SuperAdventure() {
      InitializeComponent();

      _player = new Player(10, 10, 20, 0, 1);
      MoveTo(World.LocationByID(World.LOCATION_ID_HOME));
      _player.Inventory.Add(new InventoryItem
        (World.ItemByID(World.ITEM_ID_RUSTY_SWORD), 1));

      lblHitPoints.Text = _player.CurrentHitPoints.ToString();
      lblGold.Text = _player.Gold.ToString();
      lblExperience.Text = _player.ExperiencePoints.ToString();
      lblLevel.Text = _player.Level.ToString();
    }
    private void btnNorth_Click(object sender, EventArgs e) {

      MoveTo(_player.CurrentLocation.LocationToNorth);
    }


    private void btnEast_Click(object sender, EventArgs e) {
      MoveTo(_player.CurrentLocation.LocationToEast);
    }
    private void btnSouth_Click(object sender, EventArgs e) {
      MoveTo(_player.CurrentLocation.LocationToSouth);
    }
    private void btnWest_Click(object sender, EventArgs e) {
      MoveTo(_player.CurrentLocation.LocationToWest);
    }

    private void MoveTo(Location newLocation) {
      //Does the location have any requird items
      if (newLocation.ItemRequiredToEnter != null) {
        //See if the player has the required item in their inventory
        bool playerHasRequiredItem = false;

        foreach (InventoryItem ii in _player.Inventory) {
          if (ii.Details.ID == newLocation.ItemRequiredToEnter.ID) {
            // we found the required item
            playerHasRequiredItem = true;
            //exit out the foreach loop
            break;
          }
        }
        if (!playerHasRequiredItem) {
          //we didn't find the required item in their inventory, so we display a message and stop trying to move
          rtbMessages.Text += "You must have a" +
            newLocation.ItemRequiredToEnter.Name +
            "to enter this location" + Environment.NewLine;
          return;
        }
      }
      // update the player's current location
      _player.CurrentLocation = newLocation;
      //show/hide available movement buttons

      btnNorth.Visible = (newLocation.LocationToNorth != null);
      btnEast.Visible = (newLocation.LocationToEast != null);
      btnSouth.Visible = (newLocation.LocationToSouth != null);
      btnWest.Visible = (newLocation.LocationToWest != null);

      //display current location name and discription

      rtbLocation.Text = newLocation.Name + Environment.NewLine;
      rtbLocation.Text = newLocation.Description + Environment.NewLine;

      //complety heal the player

      _player.CurrentHitPoints = _player.MaximumHitPoints;
      // update hit points in ui

      lblHitPoints.Text = _player.CurrentHitPoints.ToString();
      // does the location have a quest?

      if (newLocation.QuestAvaiblableHere != null) {
        //see if the player has the quest and if they've completed it

        bool playerAlreadyHasQuest = false;
        bool playerAlreadyCompletedQuest = false;

        foreach (PlayerQuest playerQuest in _player.Quests) {
          if (playerQuest.Details.ID == newLocation.QuestAvaiblableHere.ID) {
            playerAlreadyHasQuest = true;

            if (playerQuest.IsCompleted) {
              playerAlreadyCompletedQuest = true;
            }
          }
        }
        // see if the player already has the quest
        if (playerAlreadyHasQuest) {
          // if the player has not completed the quest yet

          if (!playerAlreadyCompletedQuest) {
            // see if the player has all the items needed to completed the quest 

            bool playerHasAllItemsToCompleteQuest = true;

            foreach (QuestCompletionItem qci in
              newLocation.QuestAvaiblableHere.QuestCompletionItems) {
              bool foundItemInPlayerInventory = false;
              //check each item in the player's inventory, to see if they have it and enough of it

              foreach (InventoryItem ii in _player.Inventory) {
                //the player has this item in their inventory
                if (ii.Details.ID == qci.Details.ID) {
                  foundItemInPlayerInventory = true;
                  {
                    if (ii.Quantity < qci.Quantity) {
                      // the player does not have enough of this item to complete the quest
                      playerHasAllItemsToCompleteQuest = false;
                      // there is no reason to continue checking for the other quest completion items

                      break;
                    }
                    // we found the item so don't check the rest of the player's inventory
                    break;
                  }
                }
                // if we didnt find the required item set our variable and stop looking for the other items
                if (!foundItemInPlayerInventory) {
                  //the player does not have this item in their inventory
                  playerHasAllItemsToCompleteQuest = false;
                  // there is no reason to continue checking for the other quest completion items

                  break;
                }
              }
            }
            // the player has all items required to complete te quest
            if (playerHasAllItemsToCompleteQuest) {
              // display message
              rtbMessages.Text += Environment.NewLine;
              rtbMessages.Text += "You complete the" +
                newLocation.QuestAvaiblableHere.Name +
                "quest." + Environment.NewLine;
              // remove quest items from inventory 
              foreach (QuestCompletionItem qci in newLocation.QuestAvaiblableHere.QuestCompletionItems) {
                foreach (InventoryItem ii in _player.Inventory) {
                  if (ii.Details.ID == qci.Details.ID) {
                    // subtract the quantity from the player's inventory that was needed to complete the quest
                    ii.Quantity -= qci.Quantity;
                    break;
                  }
                }
              }
              // give quest reward
              rtbMessages.Text += "You receive:" + Environment.NewLine;
              rtbMessages.Text +=
                newLocation.QuestAvaiblableHere.RewardExperiencePoints.ToString() +
                "experience points" + Environment.NewLine;
              rtbMessages.Text +=
                newLocation.QuestAvaiblableHere.RewardGold.ToString() +
                "gold" + Environment.NewLine;
              rtbMessages.Text +=
                newLocation.QuestAvaiblableHere.RewardItem.Name +
                Environment.NewLine;
              rtbMessages.Text += Environment.NewLine;

              _player.ExperiencePoints +=
                newLocation.QuestAvaiblableHere.RewardExperiencePoints;
              _player.Gold += newLocation.QuestAvaiblableHere.RewardGold;
              // Add the reward item to the player's inventory 
              bool addedItemToPlayerInventory = false;

              foreach (InventoryItem ii in _player.Inventory) {
                if (ii.Details.ID ==
                  newLocation.QuestAvaiblableHere.RewardItem.ID) {
                  // they have the item in their inventory, so increase the quanitiy by one
                  ii.Quantity++;

                  addedItemToPlayerInventory = true;

                  break;
                }
              }
              // they didn't have the item, so we add it to their inventory qit hthe quantity 1 
              if (!addedItemToPlayerInventory) {
                _player.Inventory.Add(new InventoryItem
                  (newLocation.QuestAvaiblableHere.RewardItem, 1));
              }
              //mark the quest as completedd
              // find the quest in the player's quest list
              foreach (PlayerQuest pq in _player.Quests) {
                if (pq.Details.ID == newLocation.QuestAvaiblableHere.ID) {
                  pq.IsCompleted = true;

                  break;
                }
              }
            }
          }
        } else {
          // the player does not already have the quest 

          // display message
          rtbMessages.Text += "You receive the" +
            newLocation.QuestAvaiblableHere.Name +
            "quest." + Environment.NewLine;
          rtbMessages.Text += "To complete it, Return with : " +
            Environment.NewLine;
          foreach (QuestCompletionItem qci in
            newLocation.QuestAvaiblableHere.QuestCompletionItems) {
            if (qci.Quantity == 1) {
              rtbMessages.Text += qci.Quantity.ToString() + " " +
                qci.Details.Name + Environment.NewLine;
            } else {
              rtbMessages.Text += qci.Quantity.ToString() + " " +
                qci.Details.NamePlural + Environment.NewLine;
            }
          }
          // add the quest to the player's quest list
          rtbMessages.Text += Environment.NewLine;

          _player.Quests.Add(new PlayerQuest(newLocation.QuestAvaiblableHere));
        }
      }
      // does the location have a monster?

      if (newLocation.MonsterLivingHere != null) {
        rtbMessages.Text += "You see a" + newLocation.MonsterLivingHere.Name +
          Environment.NewLine;
        // make a new monster, using the values from the standard monster in world.cs monster list
        Monster stadardMonster = World.MonsterByID(
          newLocation.MonsterLivingHere.ID);

        _currentMonster = new Monster(stadardMonster.ID, stadardMonster.Name,
          stadardMonster.MaximumDamage, stadardMonster.RewardExperiencePoints,
          stadardMonster.RewardGold, stadardMonster.CurrentHitPoints,
          stadardMonster.MaximumDamage);

        foreach (LootItem lootItem in stadardMonster.LootTable) {

          _currentMonster.LootTable.Add(lootItem);
        }
        cboWeapons.Visible = true;
        cboPotions.Visible = true;
        btnUseWeapon.Visible = true;
        btnUsePotion.Visible = true;

      } else {
        _currentMonster = null;

        cboWeapons.Visible = false;
        cboPotions.Visible = false;
        btnUseWeapon.Visible = false;
        btnUsePotion.Visible = false;

      }
      //refresh player's inventory list
      dgvInventory.RowHeadersVisible = false;

      dgvInventory.ColumnCount = 2;
      dgvInventory.Columns[0].Name = "name";
      dgvInventory.Columns[0].Width = 197;
      dgvInventory.Columns[1].Name = "Quantity";

      dgvInventory.Rows.Clear();
      foreach (InventoryItem inventoryItem in _player.Inventory) {
        if (inventoryItem.Quantity > 0) {
          dgvInventory.Rows.Add(new[] { inventoryItem.Details.Name,
            inventoryItem.Quantity.ToString() });

        }
      }
      // Refresh player's quest list

      dgvQuests.RowHeadersVisible = false;

      dgvQuests.ColumnCount = 2;
      dgvQuests.Columns[0].Name = "Name";
      dgvQuests.Columns[0].Width = 197;
      dgvQuests.Columns[1].Name = "Done?";

      dgvQuests.Rows.Clear();

      foreach (PlayerQuest playerQuest in _player.Quests) {
        dgvQuests.Rows.Add(new[] { playerQuest.Details.Name,
        playerQuest.IsCompleted.ToString() });
      }
      // Refresh player's weapons combobox
      List<Weapon> weapons = new List<Weapon>();

      foreach (InventoryItem inventoryItem in _player.Inventory) {
        if (inventoryItem.Details is Weapon) {
          if (inventoryItem.Quantity > 0) {
            weapons.Add((Weapon)inventoryItem.Details);
          }
        }
      }
      if (weapons.Count == 0) {
        // the player doesn't have any weapons, so hide the weapon combobox and the "use" button
        cboWeapons.Visible = false;
        btnUseWeapon.Visible = false;
      } else {
        cboWeapons.DataSource = weapons;
        cboWeapons.DisplayMember = "Name";
        cboWeapons.ValueMember = "ID";

        cboWeapons.SelectedIndex = 0;
      }
      // Refresh player's potions combobox
      List<HealingPotion> healingPotions = new List<HealingPotion>();

      foreach (InventoryItem inventoryItem in _player.Inventory) {
        if (inventoryItem.Details is HealingPotion) {
          if (inventoryItem.Quantity > 0) {
            healingPotions.Add((HealingPotion)inventoryItem.Details);
          }
        }
      }
      if (healingPotions.Count == 0) {
        // the player doesn't have any potions so hide the potion combo box and use button
        cboPotions.Visible = false;
        btnUsePotion.Visible = false;

      } else {
        cboPotions.DataSource = healingPotions;
        cboPotions.DisplayMember = "Name";
        cboPotions.ValueMember = "ID";

        cboPotions.SelectedIndex = 0;
      }
    }
    private void btnUseWeapon_Click(object sender, EventArgs e) {

    }

    private void btnUsePotion_Click(object sender, EventArgs e) {

    }
  }
}
