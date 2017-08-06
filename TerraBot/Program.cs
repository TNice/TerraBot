using System;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace TerraBot
{
    class Program
    {
        public static void Main(string[] args)
            =>  new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandService commands;
        private IServiceProvider services;

        public async Task MainAsync()
        {
            //Found in Settings.settings file
            string token = Properties.Settings.Default.Token;

            //Initalize and Setup Client and Services
            var config = new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
                MessageCacheSize = 100
            };
            client = new DiscordSocketClient(config);
            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();
            client.Log += Log;

            await InstallCommands();

            //Add Event Overides Here
            client.MessageReceived += GiveMessagePoints; ;
            client.MessageUpdated += MessageUpdated;
            client.GuildAvailable += Client_GuildAvailable;
            client.UserJoined += Client_UserJoined;
            client.UserLeft += Client_UserLeft;
            client.LeftGuild += Client_LeftGuild;
            client.Ready += () =>
            {
                Console.WriteLine("Bot Connected!");
                return Task.CompletedTask;
            };

            //Login
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();


            await Task.Delay(-1);
        }

        private Task Client_LeftGuild(SocketGuild serv)
        {
            foreach(var m in serv.Users)
            {
                int i = MemberService.FindMember(m.Id, serv.Id);
                if (i != -1)
                    MemberService.RemoveMember(i);
            }
            return Task.CompletedTask;
        }

        private Task Client_UserLeft(SocketGuildUser mem)
        {
            int i = MemberService.FindMember(mem.Id, mem.Guild.Id);
            if (i != -1)
                MemberService.RemoveMember(i);
            return Task.CompletedTask;
        }

        private Task Client_UserJoined(SocketGuildUser mem)
        {
            var temp = MemberService.CreateMember(mem.Username, mem.Id, 0, mem.Guild.Id);
            if (!MemberService.MemberExists(temp))
                MemberService.AddMember(temp);
            return Task.CompletedTask;
        }

        private Task GiveMessagePoints(SocketMessage msg)
        {
            var context = new CommandContext(client, msg as SocketUserMessage);
            var m = msg.Content;
            if (m[0] == '!')
                return Task.CompletedTask;

            double p = 0;
            if (m.Length > 75)
                p = 2;
            else if (m.Length > 30)
                p = 1;
            else if (m.Length > 25)
                p = 0.5;
            else if (m.Length > 20)
                p = 0.25;
            else if (m.Length > 15)
                p = 0.1;

            foreach(var u in msg.MentionedUsers)
            {
                p += 0.03;
            }
           
            MemberService.AddPoints(MemberService.FindMember(msg.Author.Id, context.Guild.Id), p);
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(SocketGuild serv)
        {
            if(MemberService.ListSize() == 0)
            {
                MemberService.LoadMembers();
            }
            foreach(var u in serv.Users)
            {
                var mem = MemberService.CreateMember(u.Username, u.Id, 0, serv.Id);
                if (MemberService.MemberExists(mem))
                    continue;
                MemberService.AddMember(mem);
            }
            return Task.CompletedTask;
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }

        public async Task InstallCommands()
        {
            client.MessageReceived += HandelCommands;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());           
        }

        private async Task HandelCommands(SocketMessage msgParam)
        {
            var msg = msgParam as SocketUserMessage;
            if (msg == null)
                return;

            int argPos = 0;

            if (!(msg.HasCharPrefix(Properties.Settings.Default.CmdPrefix, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos)))
                return;

            var context = new CommandContext(client, msg);
            var result = await commands.ExecuteAsync(context, argPos, services);

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);

        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
       
    }
}
