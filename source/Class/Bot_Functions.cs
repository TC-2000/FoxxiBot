﻿// Copyright(C) 2020 - 2022 FoxxiBot
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxxiBot.Class
{
    public class Bot_Functions
    {

        string cs = @"URI=file:" + AppDomain.CurrentDomain.BaseDirectory + "\\Data\\bot.db";

        public Task CreateTables()
        {
            using var con = new SQLiteConnection(cs);
            con.Open();
            
            using var cmd = new SQLiteCommand(con);
            
            cmd.CommandText = @"CREATE TABLE gb_commands (id INTEGER PRIMARY KEY, name TEXT, response TEXT, permission INTEGER, active INTEGER)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_discord_channels (channel_id INTEGER PRIMARY KEY, channel_name TEXT, channel_type TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_discord_commands (id INTEGER PRIMARY KEY, name TEXT, response TEXT, permission INTEGER, active INTEGER)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_discord_options (parameter TEXT UNIQUE, value TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_discord_options (parameter, value) VALUES('BotChannel_Status','off')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_discord_options (parameter, value) VALUES('GreetingChannel_Status','off')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_discord_options (parameter, value) VALUES('StreamChannel_Status','off')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_discord_options (parameter, value) VALUES('AutoRole_Status','off')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_discord_plugins (name TEXT, author TEXT, date TEXT, command TEXT UNIQUE, file TEXT, active INTEGER)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_discord_roles (role_id TEXT PRIMARY KEY, role_name TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_discord_streamers (username TEXT UNIQUE, live TEXT DEFAULT 0)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_options (parameter TEXT UNIQUE, value TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_options (parameter, value) VALUES('timer_mins',15)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_options (parameter, value) VALUES('stream_status',0)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_quotes (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, text TEXT, source TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_sounds (id INTEGER PRIMARY KEY AUTOINCREMENT, value TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_ticker (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT, response TEXT, active INTEGER)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_twitch_events (id INTEGER PRIMARY KEY AUTOINCREMENT, type TEXT, user TEXT, viewers TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_twitch_notifications (id INTEGER PRIMARY KEY AUTOINCREMENT, type TEXT, user TEXT, viewers TEXT, date TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_twitch_options (parameter TEXT UNIQUE, value TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_twitch_perms (name TEXT, value TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_twitch_perms (name, value) VALUES('Viewer','Viewer')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_twitch_perms (name, value) VALUES('Moderator','Moderator')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_twitch_perms (name, value) VALUES('Global Moderator','Global Moderator')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_twitch_perms (name, value) VALUES('Broadcaster','Broadcaster')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_twitch_perms (name, value) VALUES('Twitch Admin','Twitch Admin')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO gb_twitch_perms (name, value) VALUES('Twitch Staff','Twitch Staff')";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_twitch_plugins (name TEXT, author TEXT, date TEXT, command TEXT UNIQUE, file TEXT, active INTEGER)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_twitch_timers (id INTEGER PRIMARY KEY, name TEXT, response TEXT, active INTEGER)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE gb_user (displayName TEXT UNIQUE, profileIcon TEXT, viewCount INTEGER, followCount INTEGER, currentViewers INTEGER)";
            cmd.ExecuteNonQuery();

            con.Close();
            return Task.CompletedTask;
        }

        public Task SaveConfig()
        {
            Settings.Settings objSettings = new Settings.Settings();
            objSettings.Debug = Config.Debug;
            objSettings.BotName = Config.TwitchBotName;
            objSettings.TwitchClientID = Config.TwitchClientId;
            objSettings.TwitchClientSecret = Config.TwitchClientSecret;
            objSettings.TwitchClientRedirect = Config.TwitchRedirectUri;
            objSettings.TwitchClientChannel = Config.TwitchClientChannel;
            objSettings.TwitchClientUser = Config.TwitchClientUser;
            objSettings.TwitchClientOAuth = Config.TwitchClientOAuth;
            objSettings.TwitchClientRefresh = Config.TwitchClientRefresh;

            objSettings.TwitchBroadcasterId = Config.TwitchMC_Id;
            objSettings.TwitchBroadcasterDisplayName = Config.TwitchMC_DisplayName;
            objSettings.TwitchBroadcasterOAuth = Config.TwitchMC_ClientOAuth;
            objSettings.TwitchBroadcasterRefresh = Config.TwitchMC_ClientRefresh;

            objSettings.DiscordServerId = Config.DiscordServerId;
            objSettings.DiscordToken = Config.DiscordToken;
            objSettings.DiscordPrefix = Config.DiscordPrefix;

            string objjsonData = JsonConvert.SerializeObject(objSettings);
            File.WriteAllText(@AppDomain.CurrentDomain.BaseDirectory + "Data/config.json", objjsonData);
            
            return Task.CompletedTask;
        }
    }
}