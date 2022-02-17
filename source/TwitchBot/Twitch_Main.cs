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

using System;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

using FoxxiBot;
using Newtonsoft.Json;
using System.IO;
using TwitchLib.PubSub;
using System.Data.SQLite;
using System.Threading;
using System.Media;
using System.Runtime.InteropServices;

namespace FoxxiBot.TwitchBot
{

    public class Twitch_Main
    {
        public static TwitchClient client;
        TwitchPubSub pubsub;

        private Timer oauthTimer = null;
        private Timer liveTimer = null;

        string cs = @"URI=file:" + AppDomain.CurrentDomain.BaseDirectory + "\\Data\\bot.db";

        bool streamStatus;
        int current_row = 1;

        public Twitch_Main()
        {
            ConnectionCredentials credentials = new ConnectionCredentials(Config.TwitchClientUser, Config.TwitchClientOAuth);
            var clientOptions = new ClientOptions
            {
                MessagesAllowedInPeriod = 750,
                ThrottlingPeriod = TimeSpan.FromSeconds(30)
            };
            WebSocketClient customClient = new WebSocketClient(clientOptions);
            client = new TwitchClient(customClient);
            client.Initialize(credentials, Config.TwitchClientChannel);

            pubsub = new TwitchPubSub();

            client.OnLog += Client_OnLog;
            client.OnJoinedChannel += Client_OnJoinedChannel;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnNewSubscriber += Client_OnNewSubscriber;
            client.OnConnected += Client_OnConnected;
            client.OnIncorrectLogin += Client_OnIncorrectLogin;
            client.OnRaidNotification += Client_OnRaidNotification;

            pubsub.OnPubSubServiceConnected += Pubsub_OnPubSubServiceConnected;
            pubsub.OnListenResponse += Pubsub_OnListenResponse;
            pubsub.OnStreamUp += Pubsub_OnStreamUp;
            pubsub.OnStreamDown += Pubsub_OnStreamDown;
            pubsub.OnFollow += PubSub_OnFollow;

            client.Connect();
            pubsub.Connect();

            // Check Twitch Plugins
            Twitch_Jint plugins = new Twitch_Jint();
            plugins.loadPlugins();

            // Start Bot Timer
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 900000;
            timer.Elapsed += timer_Elapsed;
            timer.Start();

            // Start Live Check Timer
            liveTimer = new Timer(streamLiveCallBack, null, 0, 60000);

            // Start OAuth Timer
            oauthTimer = new Timer(OauthCallback, null, 0, 1800000);
        }

        private void streamLiveCallBack(object state)
        {
            // Check Bot Owners Twitch Channel
            streamStatus = Twitch_GetData.streamStatus().GetAwaiter().GetResult();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            // if stream live, let timers play
            if (streamStatus == true)
            {
                using var con = new SQLiteConnection(cs);
                con.Open();

                string stm = $"SELECT * FROM gb_twitch_timers WHERE rowid = {current_row} AND active = 1";

                using var cmd = new SQLiteCommand(stm, con);
                using SQLiteDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows == true)
                {
                    while (rdr.Read())
                    {
                        // Send Message to Twitch
                        client.SendMessage(Config.TwitchClientChannel, (string)rdr["response"]);

                        // Add 1 to current_row
                        current_row = current_row + 1;
                    }
                }
                else
                {
                    // Reset the current row count
                    current_row = 1;
                }

                con.Close();

                // If Discord is Active & Stream Live
                if (Config.DiscordToken != null)
                {

                }

            }
        }

        private static void refreshBotOauth()
        {

            if (Config.TwitchClientRefresh.Length == 0 || Config.TwitchMC_ClientRefresh.Length == 0)
            {
                return;
            }

            var api = new TwitchLib.Api.TwitchAPI();
            api.Settings.ClientId = Config.TwitchClientId;

            // Get New Auth Token
            var bot_authToken = api.Auth.RefreshAuthTokenAsync(Config.TwitchClientRefresh, Config.TwitchClientSecret).GetAwaiter().GetResult();
            Config.TwitchClientOAuth = bot_authToken.AccessToken;
            Config.TwitchClientRefresh = bot_authToken.RefreshToken;


            // Get New Auth Token
            var broadcast_authToken = api.Auth.RefreshAuthTokenAsync(Config.TwitchMC_ClientRefresh, Config.TwitchClientSecret).GetAwaiter().GetResult();
            Config.TwitchMC_ClientOAuth = broadcast_authToken.AccessToken;
            Config.TwitchMC_ClientRefresh = broadcast_authToken.RefreshToken;

            // Save the new JSON Data
            Class.Bot_Functions functions = new Class.Bot_Functions();
            functions.SaveConfig().GetAwaiter().GetResult();
        }

        private static void OauthCallback(object state)
        {
            // Announce OAuth Check
            Console.WriteLine(DateTime.Now + ": " + Config.TwitchBotName + " - OAuth Check");

            // Check Twitch Tokens
            refreshBotOauth();
        }

        private void Pubsub_OnListenResponse(object sender, TwitchLib.PubSub.Events.OnListenResponseArgs e)
        {
            if (e.Successful)
            {
                Console.WriteLine(DateTime.Now + ": " + Config.TwitchBotName + $" - {e.Topic}");
            }
            else
            {
                Console.WriteLine(DateTime.Now + ": " + Config.TwitchBotName +$" - { e.Response.Error}");
            }
        }

        private void Pubsub_OnPubSubServiceConnected(object sender, EventArgs e)
        {
            pubsub.ListenToVideoPlayback(Config.TwitchMC_Id);
            pubsub.ListenToFollows(Config.TwitchMC_Id);
            pubsub.ListenToWhispers(Config.TwitchMC_Id);

            // pubsub.ListenToBitsEventsV2(Config.TwitchMC_Id);
            // pubsub.ListenToChannelPoints(Config.TwitchMC_Id);
            
            pubsub.SendTopics(Config.TwitchMC_ClientOAuth);
            Console.WriteLine(DateTime.Now + ": " + Config.TwitchBotName + " - PubSub Connected");
        }

        private void Client_OnRaidNotification(object sender, OnRaidNotificationArgs e)
        {
            // Mention it in Chat            
            client.SendMessage(e.Channel, "We just got raided by " + e.RaidNotification.DisplayName.ToString() + " for " + e.RaidNotification.MsgParamViewerCount.ToString() + " viewers!");
            Console.WriteLine(DateTime.Now + ": " + Config.TwitchBotName + " - We just got raided by " + e.RaidNotification.DisplayName.ToString() + " for " + e.RaidNotification.MsgParamViewerCount.ToString() + " viewers!");

            // Save Data to Events & Notifications
            SQLite.twitchSQL TwitchSQL = new SQLite.twitchSQL();
            TwitchSQL.saveEvent("Raid", e.RaidNotification.DisplayName.ToString(), e.RaidNotification.MsgParamViewerCount.ToString());
            TwitchSQL.saveNotification("Raid", e.RaidNotification.DisplayName.ToString(), e.RaidNotification.MsgParamViewerCount.ToString(), DateTime.Now.ToString());
        }

        private void PubSub_OnFollow(object sender, TwitchLib.PubSub.Events.OnFollowArgs e)
        {
            // Save Data to Events & Notifications
            SQLite.twitchSQL TwitchSQL = new SQLite.twitchSQL();
            TwitchSQL.saveEvent("Follower", e.DisplayName.ToString(), "0");
            TwitchSQL.saveNotification("Follower", e.DisplayName.ToString(), "0", DateTime.Now.ToString());

            client.SendMessage(Config.TwitchClientChannel, "We have a new Foxy follower, welcome to " + e.DisplayName.ToString());
            Console.WriteLine(DateTime.Now + ": " + Config.TwitchBotName + " - We have a new follower: " + e.DisplayName.ToString());
        }

        private void Pubsub_OnStreamUp(object sender, TwitchLib.PubSub.Events.OnStreamUpArgs e)
        {
            Console.WriteLine(DateTime.Now + ": " + Config.TwitchBotName + " - " + $"Stream just went up! Play delay: {e.PlayDelay}, server time: {e.ServerTime}");
        }

        private void Pubsub_OnStreamDown(object sender, TwitchLib.PubSub.Events.OnStreamDownArgs e)
        {
            Console.WriteLine(DateTime.Now + ": " + Config.TwitchBotName + " - " + $"Stream just went down! Server time: {e.ServerTime}");
        }

        private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs e)
        {
            Console.WriteLine(e.Exception);
        }

        private void Client_OnLog(object sender, OnLogArgs e)
        {
            Console.WriteLine($"{e.DateTime.ToString()}: {Config.TwitchBotName} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine(DateTime.Now + ": " + Config.TwitchBotName + " - " + $"Connected to {Config.TwitchClientChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            Console.WriteLine(DateTime.Now + ": " + Config.TwitchBotName + " - Hey and thanks for coming along, we'll be going live shortly!!");
            client.SendMessage(e.Channel, $"Hey and thanks for coming along, we'll be going live shortly!!");
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {

            // split into args
            var twitchMsg = e.ChatMessage.Message.Split(" ");

            if (e.ChatMessage.IsBroadcaster || e.ChatMessage.IsModerator)
            {
                if (twitchMsg[0].Equals("!command"))
                {
                    // Should I Implement???
                }
            }

            if (e.ChatMessage.IsBroadcaster || e.ChatMessage.IsModerator || e.ChatMessage.IsVip)
            {
                if (twitchMsg[0].Equals("!so"))
                {
                    // split into args
                    var split = e.ChatMessage.Message.Replace("@", "").Split(" ");

                    try
                    {
                        var data = Twitch_GetData.ShoutOut(split[1]).GetAwaiter().GetResult();
                        client.SendMessage(e.ChatMessage.Channel, data);
                        return;
                    }
                    catch
                    {
                        var data = Twitch_GetData.displayNametoUserID(split[1]).GetAwaiter().GetResult();

                        var value = Twitch_GetData.ShoutOut(data).GetAwaiter().GetResult();
                        client.SendMessage(e.ChatMessage.Channel, value);
                        return;
                    }
                    finally
                    {
                        Console.WriteLine("!so user not found!");
                    }
                }

            }

            // Get Users Age
            if (twitchMsg[0].Equals("!age"))
            {
               var data = Twitch_GetData.getAccountAge(e.ChatMessage.UserId).GetAwaiter().GetResult();
               client.SendMessage(e.ChatMessage.Channel, e.ChatMessage.DisplayName + " your account was created " + data.ToString() + " ago");
            }

            // Play Sound File
            if (twitchMsg[0].Equals("!sound") || twitchMsg[0].Equals("!audio") || twitchMsg[0].Equals("!play"))
            {

                using var con = new SQLiteConnection(cs);
                con.Open();

                string stm = "SELECT * FROM gb_sounds WHERE name = '" + twitchMsg[1] + "' AND active = 1";

                using var cmd = new SQLiteCommand(stm, con);
                using SQLiteDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows == true)
                {

                    while (rdr.Read())
                    {

                        // Windows Implementation
                        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            using (var soundPlayer = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "\\Files\\Sounds\\" + rdr["file"]))
                            {
                                soundPlayer.Play();
                            }
                        }

                        client.SendMessage(e.ChatMessage.Channel, "Playing the Sound File: " + rdr["name"]);

                    }

                }

                con.Close();
            }


            if (e.ChatMessage.Message.Contains("!"))
            {
                using var con = new SQLiteConnection(cs);
                con.Open();

                string stm = "SELECT * FROM gb_commands WHERE name = '" + twitchMsg[0] + "' AND active = 1";

                using var cmd = new SQLiteCommand(stm, con);
                using SQLiteDataReader rdr = cmd.ExecuteReader();

                if (rdr.HasRows == true)
                {

                    while (rdr.Read())
                    {
                        Twitch_Variables variables = new Twitch_Variables();
                        var var_string = variables.convertVariables(e.ChatMessage.Message, (string)rdr["response"], e.ChatMessage.DisplayName);
                        client.SendMessage(e.ChatMessage.Channel, var_string);

                    }

                }
                else
                {              
                    // Check if Plugin
                    Twitch_Jint plugin = new Twitch_Jint();
                    plugin.ExecPlugin(e.ChatMessage.Channel, e, client, e.ChatMessage.Message);
                }

                con.Close();
            }
        }

        private void Client_OnNewSubscriber(object sender, OnNewSubscriberArgs e)
        {
            if (e.Subscriber.SubscriptionPlan == SubscriptionPlan.Prime)
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the foxy clan! You just earned 500 points! So kind of you to use your Twitch Prime on this channel!");
            else
                client.SendMessage(e.Channel, $"Welcome {e.Subscriber.DisplayName} to the foxy clan! You just earned 500 points!");
        }

        public void SendChatMessage(string message)
        {
            client.SendMessage(Config.TwitchClientChannel, message);
        }

    }
}