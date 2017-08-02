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

        [Command("join", RunMode = RunMode.Async), Summary("Joins Voice Channel")]
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

        private async Task SendAsync(IAudioClient client, string path)
        {
            var discord = client.CreateDirectPCMStream(AudioApplication.Mixed);
            await discord.FlushAsync();
        }
        
    }

    [Group("points")]
    public class PointModule : ModuleBase
    {
        public static PointService point = new PointService();
        [Command("load"), Summary("Adds All Members Of Server To Memory")]
        public async Task LoadMembers()
        {
            point.AddAllMembers(Context.Guild as SocketGuild);
            await Context.User.SendMessageAsync($"All Members In {Context.Guild.Name} Loaded To TerraBot!");
        }

        [Command("add"), Summary("Adds Points To User")]
        public async Task AddPoints(ulong user, ulong points)
        {
            var msg = Context.Message;

            var member = Context.Guild.GetUserAsync(user);
            if(member == null)
            {
                await msg.Channel.SendMessageAsync($"User With Id Of {user} Not Found!");
                return;
            }

            int i = point.FindMember(user);
            if (i == -1)
                await msg.Channel.SendMessageAsync($"User {member.Id} Not Found In DataBase");
            point.AddPoints(point.members[i], points);
        }
    }
}
