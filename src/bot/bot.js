"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
var __metadata = (this && this.__metadata) || function (k, v) {
    if (typeof Reflect === "object" && typeof Reflect.metadata === "function") return Reflect.metadata(k, v);
};
var __param = (this && this.__param) || function (paramIndex, decorator) {
    return function (target, key) { decorator(target, key, paramIndex); }
};
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const discord_js_1 = require("discord.js");
const inversify_1 = require("inversify");
const types_1 = require("../util/types");
const inversify_config_1 = require("../util/inversify.config");
const new_message_handler_1 = require("./services/event-handlers/new-message-handler");
let Bot = class Bot {
    constructor(client, token, GatewayMessageLogger, NewMessageHandler) {
        this.client = client;
        this.token = token;
        this.GatewayMessageLogger = GatewayMessageLogger;
        this.newMessageHandler = NewMessageHandler;
    }
    listen() {
        this.client.once('ready', () => __awaiter(this, void 0, void 0, function* () {
            this.client.user.setActivity("Kawaeko Bot is under development, please check back later.");
            const mongoClient = inversify_config_1.default.get(types_1.TYPES.DbClient);
            yield mongoClient.connect();
            this.commandHandler = inversify_config_1.default.get(types_1.TYPES.CommandHandler);
            this.commandList = this.commandHandler.instantiateCommands();
        }));
        this.client.on('message', (message) => __awaiter(this, void 0, void 0, function* () {
            if (message.author.bot)
                return;
            this.GatewayMessageLogger.debug(`User: ${message.author.username}\tServer: ${message.guild != null ? message.guild.name : "In DM Channel"}\tMessageRecieved: ${message.content}\tTimestamp: ${message.createdTimestamp}`);
            this.newMessageHandler.handle(message, this.commandList);
        }));
        this.client.on('error', (error) => __awaiter(this, void 0, void 0, function* () {
            this.GatewayMessageLogger.error(`Kawaeko Bot Error: ${error}`);
            var devUser = this.client.users.cache.find(user => user.id == process.env.DEVID);
            devUser.send(`**Kawaeko Bot Error**: ${JSON.stringify(error, null, 2)}`);
        }));
        return this.client.login(this.token);
    }
};
Bot = __decorate([
    inversify_1.injectable(),
    __param(0, inversify_1.inject(types_1.TYPES.Client)),
    __param(1, inversify_1.inject(types_1.TYPES.Token)),
    __param(2, inversify_1.inject(types_1.TYPES.GatewayMessageLogger)),
    __param(3, inversify_1.inject(types_1.TYPES.NewMessageHandler)),
    __metadata("design:paramtypes", [discord_js_1.Client, String, Object, new_message_handler_1.NewMessageHandler])
], Bot);
exports.Bot = Bot;
//# sourceMappingURL=bot.js.map