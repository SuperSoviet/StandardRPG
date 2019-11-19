using System;
using System.Data;
using System.Linq;
using Npgsql;
using NpgsqlTypes;

namespace Engine {
  public static class PlayerDataMapper {
    private static readonly string _connectionString =
        "Server=127.0.0.1;Username=postgres;Database=SuperAdventure;Port=5432;Password=100%Koffie;SSLMode=Prefer";

    public static Player CreateFromDataBase() {
      try {
        // this is our connection to the data base
        using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString)) {
          connection.Open();
          Player player;

          // create a sql command object that uses the connection to our data base the sql command object is where we 
          // create our sql statement
          using (NpgsqlCommand savedGameCommand = new NpgsqlCommand("SELECT * FROM SavedGame LIMIT 1", connection))
          using (NpgsqlDataReader reader = savedGameCommand.ExecuteReader()) {
            if (!reader.HasRows) {
              // there is no data in the savegame table so return null no saved player data
              return null;
            }

            // get the row/record fomr the data reader
            reader.Read();
            // get the column value for the row/record
            int currentHitPoints = (int)reader["CurrentHitPoints"];
            int maximumHitPoints = (int)reader["MaximumHitPoints"];
            int gold = (int)reader["Gold"];
            int experiencePoints = (int)reader["ExperiencePoints"];
            int currentLocationID = (int)reader["CurrentLocationID"];
            // create the player object with the saved game values
            player = Player.CreatePlayerFromDatabase(
                currentHitPoints, maximumHitPoints, gold, experiencePoints, currentLocationID);
          }

          // read the rows / records from the quest table and add them to the player
          using (NpgsqlCommand questCommand = new NpgsqlCommand("SELECT * FROM Quest", connection))
          using (NpgsqlDataReader reader = questCommand.ExecuteReader()) {
            if (reader.HasRows) {
              while (reader.Read()) {
                int questID = (int)reader["QuestID"];
                bool isCompleted = (bool)reader["IsCompleted"];
                //build the PlayerQuest item for this row
                PlayerQuest playerQuest =
                    new PlayerQuest(World.QuestByID(questID));
                playerQuest.IsCompleted = isCompleted;
                // add the PlayerQuest to the players property
                var playerQuestHas = player.Quests.FirstOrDefault(x => x.Details.ID == playerQuest.Details.ID);
                if (playerQuestHas != null) {
                  playerQuestHas.IsCompleted = isCompleted;
                } else {
                  player.Quests.Add(playerQuest);
                }
              }
            }
          }

          //read the rows/ records from the inventory table and add them to the player
          using (NpgsqlCommand inventoryCommand = new NpgsqlCommand("SELECT * FROM Inventory", connection))
          using (NpgsqlDataReader reader = inventoryCommand.ExecuteReader()) {
            if (reader.HasRows) {
              while (reader.Read()) {
                int inventoryItemID = (int)reader["InventoryItemID"];
                int quantity = (int)reader["Quantity"];

                // add the item to the players inventory
                player.AddItemToInventory(World.ItemByID(inventoryItemID), quantity);
              }
            }
          }

          //now that the player has been build from the database return it
          return player;
        }
      } catch (Exception ex) {
        Console.WriteLine(ex);
        //ignore errors if there is an error this function will return a null player
      }

      return null;
    }

    public static void SaveToDatabase(Player player) {
      try {
        using (NpgsqlConnection connection = new NpgsqlConnection(_connectionString)) {
          //open the connection so we can perform sql commands
          connection.Open();
          // insert/update data in savedgame table
          using (NpgsqlCommand existingRowCountCommand = connection.CreateCommand()) {
            existingRowCountCommand.CommandType = CommandType.Text;
            existingRowCountCommand.CommandText = "SELECT count(*) FROM SavedGame";

            // use executescalar when your query will return one value
            long existingRowCount = (long)existingRowCountCommand.ExecuteScalar();
            if (existingRowCount == 0) {
              //there is no existing row so do an insert
              using (NpgsqlCommand insertSavedGame = connection.CreateCommand()) {
                insertSavedGame.CommandType = CommandType.Text;
                insertSavedGame.CommandText = "INSERT INTO SavedGame" +
                                              " (CurrentHitPoints, MaximumHitPoints," +
                                              " Gold, ExperiencePoints, CurrentLocationID)" +
                                              " VALUES" +
                                              " (@CurrentHitPoints, @MaximumHitPoints, @Gold," +
                                              " @ExperiencePoints, @CurrentLocationID)";
                // pass the values from the player object to the SQL query using parameters
                insertSavedGame.Parameters.Add(
                    "@CurrentHitPoints", NpgsqlDbType.Integer);
                insertSavedGame.Parameters["@CurrentHitPoints"].Value =
                    player.CurrentHitPoints;
                insertSavedGame.Parameters.Add(
                    "@MaximumHitPoints", NpgsqlDbType.Integer);
                insertSavedGame.Parameters["@MaximumHitPoints"].Value =
                    player.MaximumHitPoints;
                insertSavedGame.Parameters.Add(
                    "@Gold", NpgsqlDbType.Integer);
                insertSavedGame.Parameters["@Gold"].Value =
                    player.Gold;
                insertSavedGame.Parameters.Add(
                    "@ExperiencePoints", NpgsqlDbType.Integer);
                insertSavedGame.Parameters["@ExperiencePoints"].Value =
                    player.ExperiencePoints;
                insertSavedGame.Parameters.Add(
                    "CurrentLocationID", NpgsqlDbType.Integer);
                insertSavedGame.Parameters["@CurrentLocationID"].Value =
                    player.CurrentLocation.ID;
                //perform the sql command
                //use excutenonquery because this query does not return any results
                insertSavedGame.ExecuteNonQuery();
              }
            } else {
              // there is an existing row so do an update
              using (NpgsqlCommand updateSavedGame = connection.CreateCommand()) {
                updateSavedGame.CommandType = CommandType.Text;
                updateSavedGame.CommandText =
                    "UPDATE SavedGame" +
                    " SET CurrentHitPoints = @CurrentHitPoints," +
                    " MaximumHitPoints = @MaximumHitPoints," +
                    " Gold = @Gold," +
                    " ExperiencePoints = @ExperiencePoints, " +
                    " CurrentLocationID = @CurrentLocationID";
                //pass the values from the player object  to the sql query using parameters
                // using parameters helps make your program more secure
                //it will prevent sql injection attacks
                updateSavedGame.Parameters.Add(
                    "@CurrentHitPoints", NpgsqlDbType.Integer);
                updateSavedGame.Parameters["@CurrentHitPoints"].Value =
                    player.CurrentHitPoints;
                updateSavedGame.Parameters.Add(
                    "@MaximumHitPoints", NpgsqlDbType.Integer);
                updateSavedGame.Parameters["@MaximumHitPoints"].Value =
                    player.MaximumHitPoints;
                updateSavedGame.Parameters.Add(
                    "@Gold", NpgsqlDbType.Integer);
                updateSavedGame.Parameters["@Gold"].Value =
                    player.Gold;
                updateSavedGame.Parameters.Add(
                    "@ExperiencePoints", NpgsqlDbType.Integer);
                updateSavedGame.Parameters["@ExperiencePoints"].Value =
                    player.ExperiencePoints;
                updateSavedGame.Parameters.Add(
                    "@CurrentLocationID", NpgsqlDbType.Integer);
                updateSavedGame.Parameters["@CurrentLocationID"].Value =
                    player.CurrentLocation.ID;
                // perform the sql command
                // use executeNonQuary because this query does not return any results
                updateSavedGame.ExecuteNonQuery();
              }
            }
          }
          // the quest and inventory tables might have more or less rows in 
          // the database than what the player has in their properties
          // so when we save the players game we will delete all the old rows
          // and add in all new rows
          // this is easier than trying to add/delete/updata each individual rows

          // delete existing quest rows
          using (NpgsqlCommand deleteQuestsCommand = connection.CreateCommand()) {
            deleteQuestsCommand.CommandType = CommandType.Text;
            deleteQuestsCommand.CommandText = "DELETE FROM Quest";

            deleteQuestsCommand.ExecuteNonQuery();
          }

          // insert Quest rows from the player object
          foreach (PlayerQuest pq in player.Quests) {
            using (NpgsqlCommand insertQuestCommand = connection.CreateCommand()) {
              insertQuestCommand.CommandType = CommandType.Text;
              insertQuestCommand.CommandText =
                  "INSERT INTO Quest (QuestID, IsCompleted) VALUES (@QuestID, @IsCompleted)";
              insertQuestCommand.Parameters.Add(
                  "@QuestID", NpgsqlDbType.Integer);
              insertQuestCommand.Parameters["@QuestID"].Value =
                  pq.Details.ID;
              insertQuestCommand.Parameters.Add(
                  "@IsCompleted", NpgsqlDbType.Boolean);
              insertQuestCommand.Parameters["@IsCompleted"].Value =
                  pq.IsCompleted;

              insertQuestCommand.ExecuteNonQuery();
            }
          }

          //delete existing Inventory rows
          using (NpgsqlCommand deleteInventoryCommand = connection.CreateCommand()) {
            deleteInventoryCommand.CommandType = CommandType.Text;
            deleteInventoryCommand.CommandText = "DELETE FROM Inventory";

            deleteInventoryCommand.ExecuteNonQuery();
          }

          //insert Inventory rows, from the player object
          foreach (InventoryItem inventoryItem in player.Inventory) {
            using (NpgsqlCommand insertInventoryCommand = connection.CreateCommand()) {
              insertInventoryCommand.CommandType = CommandType.Text;
              insertInventoryCommand.CommandText = "INSERT INTO Inventory (InventoryItemID, Quantity)" +
                                                   " VALUES (@InventoryItemID, @Quantity)";
              insertInventoryCommand.Parameters.Add(
                  "@InventoryItemID", NpgsqlDbType.Integer);
              insertInventoryCommand.Parameters["@InventoryItemID"].Value = inventoryItem.Details.ID;
              insertInventoryCommand.Parameters.Add(
                  "@Quantity", NpgsqlDbType.Integer);
              insertInventoryCommand.Parameters["@Quantity"].Value = inventoryItem.Quantity;
              insertInventoryCommand.ExecuteNonQuery();
            }
          }
        }
      } catch (Exception exception) {
        // we are going to ignore errors for now
      }
    }
  }
}