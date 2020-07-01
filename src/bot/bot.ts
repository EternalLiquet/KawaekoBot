import { Client, Message, Collection } from "discord.js";
import { inject, injectable } from "inversify";
import { TYPES } from "../util/types";
import { Logger } from "typescript-logging";
import { DbClient } from "../util/dbclient";
import container from "../util/inversify.config";
import { CommandHandler } from "./services/command-services/command-handler";
import { NewMessageHandler } from './services/event-handlers/new-message-handler';

@injectable()
export class Bot {
    private client: Client;
    private readonly token: string;
    private GatewayMessageLogger: Logger;
    private commandHandler: CommandHandler;
    private newMessageHandler: NewMessageHandler;
    private commandList: Collection<string, any>;

    constructor(
        @inject(TYPES.Client) client: Client,
        @inject(TYPES.Token) token: string,
        @inject(TYPES.GatewayMessageLogger) GatewayMessageLogger: Logger,
        @inject(TYPES.NewMessageHandler) NewMessageHandler: NewMessageHandler
    ) {
        this.client = client;
        this.token = token;
        this.GatewayMessageLogger = GatewayMessageLogger;
        this.newMessageHandler = NewMessageHandler;
    }

    public listen(): Promise<string> {
        this.client.once('ready', async () => {
            this.client.user.setActivity("Kawaeko Bot is under development, please check back later.");
            const mongoClient = container.get<DbClient>(TYPES.DbClient);
            await mongoClient.connect();
            this.commandHandler = container.get<CommandHandler>(TYPES.CommandHandler);
            this.commandList = this.commandHandler.instantiateCommands();
        });

        this.client.on('message', async (message: Message) => {
            if (message.author.bot) return;

            this.GatewayMessageLogger.debug(`User: ${message.author.username}\tServer: ${message.guild != null ? message.guild.name : "In DM Channel"}\tMessageRecieved: ${message.content}\tTimestamp: ${message.createdTimestamp}`);

            this.newMessageHandler.handle(message, this.commandList);
        });

        this.client.on('error', async (error: Error) => {
            this.GatewayMessageLogger.error(`Kawaeko Bot Error: ${error}`);
            var devUser = this.client.users.cache.find(user => user.id == process.env.DEVID);
            devUser.send(`**Kawaeko Bot Error**: ${JSON.stringify(error, null, 2)}`);
        });
        return this.client.login(this.token);
    }
}
