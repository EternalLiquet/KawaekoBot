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
            }
        ];
    }
}
exports.FunModule = FunModule;
//# sourceMappingURL=fun-module.js.map