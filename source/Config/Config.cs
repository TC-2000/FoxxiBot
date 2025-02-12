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
using System.Collections.Generic;
using System.Text;

namespace FoxxiBot
{
    public static class Config
    {
        // Debug
        public static string Debug = "false";

        // Webserver
        public static string WebserverPort = "25000";

        // Twitch - Bot Parameters
        public static string TwitchClientId = "";
        public static string TwitchRedirectUri = "http://localhost:8080/redirect/";
        public static string TwitchClientSecret = "";

        public static string TwitchBotName = "FoxxiBot";
        public static string TwitchClientUser = "";
        public static string TwitchClientChannel = "";
        public static string TwitchClientOAuth = "";
        public static string TwitchClientRefresh = "";

        // Twitch - User Parameters
        public static string TwitchMC_Id = "";
        public static string TwitchMC_ClientOAuth = "";
        public static string TwitchMC_ClientRefresh = "";

        // Discord Parameters
        public static string DiscordServerId = "";
        public static string DiscordToken = "";
        public static string DiscordPrefix = "!";

    }
}