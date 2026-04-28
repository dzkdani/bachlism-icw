SKILL: Game Design for Decision Games

Inspired by:
- Reigns (https://store.steampowered.com/app/474750/Reigns/)
- Lapse (https://play.google.com/store/apps/details?id=com.cornago.stefano.lapse&pcampaignid=web_share)

## Stats:
- Environment
- Economy
- Trust
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
- using 2 active cards pipeline system:
  - only show 1 card at a time, but have the next card ready to be shown after the player makes a choice on the current card
  - when player makes a choice, apply the effects to the stats, then show the next card with the updated stats
  - using object pooling system to manage the card gameobjects for better performance
- deploy card decks
- show card 
- make choice
- resolve choice
- update stats
- show new cards
- make choice again

## Ending
Game Over Conditions:
- Corruption >= 100
- Public Trust <= 0
- Environment <= 0
- Economy <= 0

Winning Conditions:
- Survive for 100 game loop turns

## TBD
Special Events:


