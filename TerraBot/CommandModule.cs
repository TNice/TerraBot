using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.Audio;
using Discord.WebSocket;
using Discord;
using TerraBot.Services;

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
        [Command("load")]
        public async Task LoadMembers()
        {
            MemberService.LoadMembers();
            await Context.Channel.SendMessageAsync("Members Loaded");
        }

        [Command("save")]
        public async Task SaveMembers()
        {
            MemberService.SaveMembers();
            await Context.Channel.SendMessageAsync("Points Service Saved To File");
        }

        [Command("print")]
        public async Task PrintPointsToConsole()
        {
            Console.WriteLine("Printing Members...");
            MemberService.PrintList();
            await Context.Channel.SendMessageAsync("Users Printed To Console");
        }

        [Command("create"), Alias("c"), Summary("Create Fake Member To List To Test(DEBUG USE ONLY)"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task CreateMem(string name, ulong id)
        {
            MemberService.AddMember(MemberService.CreateMember(name, id, 0, Context.Guild.Id));
            await Context.Channel.SendMessageAsync("Fake Member Created");
        }

        [Command("delete"), Alias("d"), Summary("DEBUG ONLY"), RequireUserPermission(GuildPermission.Administrator)]
        public async Task DeleteMem(ulong id)
        {
            MemberService.RemoveMember(MemberService.FindMember(id));
            await Context.Channel.SendMessageAsync("Member Deleted!");
        }

        [Command("add"), Alias("a"), Summary("Adds Points To User")]
        public async Task AddPoints(ulong user, double points)
        {
            var msg = Context.Message;

            var member = Context.Guild.GetUserAsync(user);
            if(member == null)
            {
                await msg.Channel.SendMessageAsync($"User With Id Of {user} Not Found!");
                return;
            }

            int i = MemberService.FindMember(user, Context.Guild.Id);
            if (i == -1)
                await msg.Channel.SendMessageAsync($"User {member.Id} Not Found In DataBase");
            MemberService.AddPoints(i, points);
        }

        [Command("remove"), Alias("rmv", "r"), Summary("Removes Points")]
        public async Task RemovePoints(ulong user, double points)
        {
            var msg = Context.Message;

            var member = Context.Guild.GetUserAsync(user);
            if (member == null)
            {
                await msg.Channel.SendMessageAsync($"User With Id Of {user} Not Found!");
                return;
            }

            int i = MemberService.FindMember(user);
            if (i == -1)
                await msg.Channel.SendMessageAsync($"User {member.Id} Not Found In DataBase");
            MemberService.AddPoints(i, (-1 * points));
            await Context.Channel.SendMessageAsync("Not Implamented Yet!");
        }
    }
}
