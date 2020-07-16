"use strict";
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
const cross_fetch_1 = require("cross-fetch");
const kawaeko_uwu_counter_1 = require("../../../../entities/kawaeko-uwu-counter");
const inversify_config_1 = require("../../../../util/inversify.config");
const mongodb_typescript_1 = require("mongodb-typescript");
const types_1 = require("../../../../util/types");
class FunModule {
    constructor() {
        this.ModuleCommandList = [
            {
                name: 'yuumi',
                description: 'Meow adc-kun let me buff u uwu',
                help_text: `POC`,
                alias: 'meow',
                execute(message, args) {
                    return __awaiter(this, void 0, void 0, function* () {
                        message.channel.send('meow adc-kun let me buff u uwu')
                            .catch(error => {
                            Promise.reject(error);
                        });
                    });
                }
            },
            {
                name: '8ball',
                description: 'Let\'s predict the future',
                help_text: `POC`,
                alias: 'meow',
                execute(message, args) {
                    return __awaiter(this, void 0, void 0, function* () {
                        if (args.trim() == '')
                            message.channel.send('Please ask a question');
                        var uri = `https://8ball.delegator.com/magic/JSON/${encodeURIComponent(args)}`;
                        yield cross_fetch_1.default(uri)
                            .then(response => response.json())
                            .then(answer => message.channel.send(answer.magic.answer))
                            .catch(error => {
                            Promise.reject(error);
                        });
                    });
                }
            },
            {
                name: 'meme',
                description: 'Give random meme',
                help_text: 'POC',
                alias: 'meme',
                execute(message, args) {
                    return __awaiter(this, void 0, void 0, function* () {
                        var uri = `https://meme-api.herokuapp.com/gimme`;
                        console.log('here');
                        yield cross_fetch_1.default(uri)
                            .then(response => response.json())
                            .then(answer => {
                            console.log(answer.url);
                            message.channel.send(answer.url);
                        })
                            .catch(error => {
                            Promise.reject(error);
                        });
                    });
                }
            },
            {
                name: 'uwu',
                description: 'See how many times a user has said uwu',
                help_text: 'POC',
                alias: 'owo',
                execute(message, args) {
                    return __awaiter(this, void 0, void 0, function* () {
                        if (args.trim() == '') {
                            var uwuRepo = new mongodb_typescript_1.Repository(kawaeko_uwu_counter_1.KawaekoBotUwUCounter, inversify_config_1.default.get(types_1.TYPES.DbClient).db, "UwU Counter");
                            var uwuList = uwuRepo.find({});
                            var uwuCount = 0;
                            yield uwuList.forEach((entry) => {
                                if (entry.guildId == message.guild.id) {
                                    uwuCount += entry.uwuCount;
                                }
                                ;
                            });
                            message.channel.send(`${message.guild.name}'s uwu count is ${uwuCount}`);
                        }
                        else if (message.guild.members.cache.find(user => user.displayName.trim().toLowerCase() == args.trim().toLowerCase())) {
                            var guildMember = message.guild.members.cache.find(user => user.displayName.trim().toLowerCase() == args.trim().toLowerCase());
                            if (guildMember.user.bot) {
                                message.reply(`${guildMember.displayName} is a bot`);
                                return;
                            }
                            var uwuRepo = new mongodb_typescript_1.Repository(kawaeko_uwu_counter_1.KawaekoBotUwUCounter, inversify_config_1.default.get(types_1.TYPES.DbClient).db, "UwU Counter");
                            var uwuList = uwuRepo.find({});
                            var uwuCount = 0;
                            yield uwuList.forEach((entry) => {
                                if (entry.guildId == message.guild.id && entry.userId == guildMember.id) {
                                    uwuCount += entry.uwuCount;
                                }
                                ;
                            });
                            message.channel.send(`${guildMember.displayName}'s uwu count is ${uwuCount}`);
                        }
                        else {
                            message.reply(`user ${args} not found!`);
                        }
                        ;
                    });
                }
            }
        ];
    }
}
exports.FunModule = FunModule;
//# sourceMappingURL=fun-module.js.map