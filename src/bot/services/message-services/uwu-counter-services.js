"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
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
const inversify_1 = require("inversify");
const mongodb_typescript_1 = require("mongodb-typescript");
const inversify_config_1 = require("../../../util/inversify.config");
const types_1 = require("../../../util/types");
const kawaeko_uwu_counter_1 = require("../../../entities/kawaeko-uwu-counter");
let UwUCounterService = class UwUCounterService {
    handle(message) {
        return __awaiter(this, void 0, void 0, function* () {
            if (message.guild == null)
                return;
            if (message.content.toLowerCase().includes('uwu') == false)
                return;
            var kawaekoId = `${message.guild.id}${message.author.id}`;
            var guildId = message.guild.id;
            var userId = message.author.id;
            var uwuRepo = new mongodb_typescript_1.Repository(kawaeko_uwu_counter_1.KawaekoBotUwUCounter, inversify_config_1.default.get(types_1.TYPES.DbClient).db, "UwU Counter");
            yield uwuRepo.findById(kawaekoId).then((uwuEntry) => __awaiter(this, void 0, void 0, function* () {
                if (uwuEntry == null) {
                    yield uwuRepo.insert(new kawaeko_uwu_counter_1.KawaekoBotUwUCounter(kawaekoId, guildId, userId, 1));
                }
                else {
                    yield uwuRepo.update(new kawaeko_uwu_counter_1.KawaekoBotUwUCounter(kawaekoId, guildId, userId, uwuEntry.uwuCount + 1));
                }
            }));
        });
    }
    ;
};
UwUCounterService = __decorate([
    inversify_1.injectable()
], UwUCounterService);
exports.UwUCounterService = UwUCounterService;
;
//# sourceMappingURL=uwu-counter-services.js.map