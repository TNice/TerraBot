using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Audio;
using Discord.WebSocket;
using Discord;

namespace TerraBot
{
    class CommandModule : ModuleBase
    {

        [Command("ping"), Summary("Pings The Bot")]
        public async Task PingCommand()
        {
            await ReplyAsync("pong!");
        }

        [Command("join"), Summary("Joins A Voice Channel")]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            channel = channel ?? (msg.Author as IGuildUser)?.VoiceChannel;

        }
    }
}
