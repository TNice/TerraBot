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
            string token = Settings.Default.Token;
            PointService.LoadPoints();
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
            client.MessageUpdated += MessageUpdated;
            client.GuildAvailable += Client_GuildAvailable;
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

        private Task Client_GuildAvailable(SocketGuild arg)
        {
            PointService.AddAllMembers(arg);
            return Task.CompletedTask;
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }

        //Find Out How to pass PointService to PointModule
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

            if (!(msg.HasCharPrefix(Settings.Default.CmdPrefix, ref argPos) || msg.HasMentionPrefix(client.CurrentUser, ref argPos)))
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
