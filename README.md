# ParseAndRecreation

I started off trying something different.  

Using QuickPulse to parse things, ... fun. Also added some useful methods to QuickPulse itself which I'm sure will come in handy later.

But, I did not manage to translate the Dijkstra Algo to a flow, so *that one* basically lives in the `ParseState` class and the FlowParser ended up being an over-engineered for loop, ... with no real benefit.

It looks cool and it's clever, but the RegularParser does the same thing and you don't need to know about Monads to understand how it works.

So yeah, cool, ... not always the right way to go.

Left the FlowParser here as a reminder/warning to my future self and maybe, one day, if I do manage to pull `ParseState` into `Parser`, it might actually be worth the extra cognitive load.

But for now my advice, just implement the [Shunting yard algorithm](https://en.wikipedia.org/wiki/Shunting_yard_algorithm) in the simplest way possible.


|                | Files | Lines | Blanks | Comments | Code | Complexity |
|----------------|-------|-------|--------|----------|------|------------|
| RegularParser  |   3   |  212  |   27   |     0    |  185 |     43     |
| RP Definitions |  10   |   60  |   19   |     0    |   41 |      0     |
| RP Total       |  13   |  272  |   46   |     0    |  226 |     43     | 
| Paarser        |   8   |  489  |   77   |    68    |  344 |     91     |