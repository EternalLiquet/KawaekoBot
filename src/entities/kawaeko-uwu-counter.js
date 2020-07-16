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
Object.defineProperty(exports, "__esModule", { value: true });
const mongodb_typescript_1 = require("mongodb-typescript");
class KawaekoBotUwUCounter {
    constructor(kawaekoBotId, guildId, userId, uwuCount) {
        this.kawaekoBotId = kawaekoBotId;
        this.guildId = guildId;
        this.userId = userId;
        this.uwuCount = uwuCount;
    }
}
__decorate([
    mongodb_typescript_1.id,
    __metadata("design:type", String)
], KawaekoBotUwUCounter.prototype, "kawaekoBotId", void 0);
exports.KawaekoBotUwUCounter = KawaekoBotUwUCounter;
//# sourceMappingURL=kawaeko-uwu-counter.js.map