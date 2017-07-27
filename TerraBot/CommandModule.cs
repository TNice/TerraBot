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
    public class TestingModule : ModuleBase
    {

        [Command("ping"), Summary("Pings The Bot")]
        public async Task PingCommand()
        {
            await ReplyAsync("pong!");
        }
        
    }


    public class AudioModule : ModuleBase
    {

        static IAudioClient audioClient;

        [Command("play ", RunMode = RunMode.Async), Summary("Joins Voice Channel")]
        public async Task JoinCmd(IVoiceChannel channel = null)
        {
            var msg = Context.Message;
          
            channel = channel ?? (msg.Author as IGuildUser)?.VoiceChannel;
            if (channel == null) { await msg.Channel.SendMessageAsync("User must be in a voice channel, or a voice channel must be passed as an argument."); return; }

            audioClient = await channel.ConnectAsync();
        }

        [Command("stop"), Summary("Leaves Voice Channel")]
        public async Task LeaveCmd()
        {
            await audioClient.StopAsync();
        }

    }
}
