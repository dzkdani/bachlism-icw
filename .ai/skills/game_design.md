SKILL: Game Design for Decision Games

Inspired by:
- Reigns (https://store.steampowered.com/app/474750/Reigns/)
- Lapse (https://play.google.com/store/apps/details?id=com.cornago.stefano.lapse&pcampaignid=web_share)

## Stats:
- Environment
- Economy
- Public Trust
- Corruption
- base starting stats: random between 10 - 50

## Design guidelines:
- Player should feel the weight of their choices

## Card design guidelines:
- 2 choice per card
- Each choice affects one or multiple stats
- card had :
  - title
  - sprite image
  - description
  - choice 
- choice has:
  - description
  - list of effected stats and amounts

## Game Loop:
- draw card
- make choice
- resolve choice
- update stats
- repeat

## Ending
Game Over Conditions:
- Corruption >= 100
- Public Trust <= 0
- Environment <= 0
- Economy <= 0

Winning Conditions:
- Survive for "100 hari kerja"

## TBD
Special Events:


