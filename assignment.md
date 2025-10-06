# Parser

## Doel

* Implementeer een **handgeschreven (recursive descent)** parser met correcte **precedentie** en **associativiteit**.
* Bouw een **AST** met immutabele, eenvoudige *nodes*.
* Schrijf een **PrettyPrinter** die minimale haakjes gebruikt.
* Garandeer een **round‑trip**: `Parse(PrettyPrint(Parse(input)))` is structureel gelijk aan `Parse(input)`.
* Levert **duidelijke foutmeldingen** met positie‑informatie.

## Mini‑grammatica (EBNF)

```
Expr   := Sum
Sum    := Prod (('+' | '-') Prod)*
Prod   := Pow  (('*' | '/') Pow)*
Pow    := Atom ('^' Pow)?           // rechts-associatief
Atom   := Number | '(' Expr ')'
Number := ('0' | [1-9][0-9]*) ('.' [0-9]+)?
```

**Opmerkingen**

* `^` is **rechts‑associatief**: `a^b^c` ≡ `a^(b^c)`.
* `-` komt zowel unair (bv. `-x`) als binair (bv. `a-b`) voor.
* Whitespace mag overal voorkomen en wordt genegeerd.

## Minimale eisen

1. **Parser**
   * Correcte verwerking van unair `-` vs. binair `-`.
   * Precedentie: `^` > `*`/`/` > `+`/`-`.
   * Links‑associatief voor `+`, `-`, `*`, `/`; **rechts**‑associatief voor `^`.

2. **AST‑model**
   * Knopen voor: `Number`, `UnaryMinus`, `Binary(+,-,*,/,^)`, en optioneel `Parenthesis`.
   * Immutabel en geschikt voor structurele vergelijking.

3. **Pretty‑printing**
   * Produceert een string met **minimale** haakjes (alleen waar kind‑precedentie < ouder‑precedentie of bij noodzakelijke associativiteit).

4. **Round‑trip garantie**
   * Voor elke geldige input `s`: `Parse(PrettyPrint(Parse(s)))` levert een AST dat structureel gelijk is aan `Parse(s)`.

5. **Foutafhandeling**
   * Parse‑errors bevatten `line:col`, het **verwachte token** en een korte **hint**.

6. **Whitespace**
   * Whitespace tussen tokens wordt genegeerd in numbers niet toegestaan.

## Acceptatiecriteria

* [ ] **Precedentie & associativiteit** strikt volgens de grammatica.
* [ ] **Unair minus** correct: `-x^2` parse’t als `-(x^2)` (niet `(-x)^2`).
* [ ] **Minimale haakjes**: bv. `a+b*c` blijft `a+b*c`; `a*(b+c)` houdt haakjes.
* [ ] **Round‑trip**: `Parse(PrettyPrint(Parse(s))) == Parse(s)` (structureel).
* [ ] **Idempotente pretty‑print**: `PP(Parse(PP(Parse(s)))) == PP(Parse(s))`.
* [ ] **Fouten**: bij ongeldige input een duidelijke melding met positie en verwachte symbolen.
* [ ] **Spaties**: willekeurige spaties in geldige input veranderen de AST niet.

## Voorbeelden (verwacht gedrag)
* Invoer: `-x^2 + 3*(y-1)`
  * AST representeert `(-(x^2)) + (3 * (y - 1))`.
  * PrettyPrint: `-x^2+3*(y-1)`.
* Invoer: `a/b*c` → PrettyPrint: `a/b*c` (links‑associatief voor `*`/`/`).
* Invoer: `a^(b^c)` → PrettyPrint: `a^b^c`.
