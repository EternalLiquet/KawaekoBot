import "reflect-metadata";
import { Container } from "inversify";
import { TYPES } from "./types";
import { Bot } from "../bot/bot";
import { Client } from "discord.js";
import { Logger } from "typescript-logging";
import { factory } from "./log.config";
import { DbClient } from "./dbclient";
import { CommandService } from "../bot/services/command-services/command-service";
import { NewMessageHandler } from "../bot/services/event-handlers/new-message-handler";

let container = new Container();

container.bind<Bot>(TYPES.Bot).to(Bot).inSingletonScope();
container.bind<Client>(TYPES.Client).toConstantValue(new Client());
container.bind<string>(TYPES.Token).toConstantValue(process.env.TOKEN);
container.bind<string>(TYPES.DbConnectionString).toConstantValue(process.env.DBCONNECTIONSTRING)
container.bind<Logger>(TYPES.GatewayMessageLogger).toConstantValue(factory.getLogger("Gateway.MessageRecieved"));
container.bind<Logger>(TYPES.GatewayConnectionLogger).toConstantValue(factory.getLogger("GatewayConnection"));
container.bind<Logger>(TYPES.DatabaseConnectionLogger).toConstantValue(factory.getLogger("DatabaseConnection"));
container.bind<Logger>(TYPES.GatewayEventLogger).toConstantValue(factory.getLogger("Gateway.Event"));
container.bind<DbClient>(TYPES.DbClient).to(DbClient).inSingletonScope();
container.bind<CommandService>(TYPES.CommandService).to(CommandService).inSingletonScope();
container.bind<NewMessageHandler>(TYPES.NewMessageHandler).to(NewMessageHandler).inSingletonScope();

export default container;