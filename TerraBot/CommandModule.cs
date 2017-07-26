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
    public class CommandModule : ModuleBase
    {

        [Command("ping"), Summary("Pings The Bot")]
        public async Task PingCommand()
        {
            await ReplyAsync("pong!");
        }

        [Command("join", RunMode = RunMode.Async), Summary("Joins A Voice Channel")]
        public async Task JoinChannel(IVoiceChannel channel = null)
        {
            var msg = Context.Message;
            channel = channel ?? (msg.Author as IGuildUser)?.VoiceChannel;
            if (channel == null) await msg.Channel.SendMessageAsync("User Must Be In A Voice Channel, or A Voice Channel Must Be Specified");

            var audioClient = await channel.ConnectAsync();
            
        }
    }
}
