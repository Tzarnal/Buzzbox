Buzzbox
=============

HS counterpart to [mtgencode](https://github.com/billzorn/mtgencode). Data management utilities for creating Hearthstone cards with neural networks.

Inspired by scfdivine on [reddit](https://www.reddit.com/comments/3cyi15/) who brought using RNNs to generate cards to my attention. Further credit to the contributors of [mtg-rnn](https://github.com/billzorn/mtg-rnn), in turn based on [char-rnn](https://github.com/karpathy/char-rnn). Also to billzorn for [mtgencode](https://github.com/billzorn/mtgencode) and [his](https://github.com/billzorn/torch-rnn) fork of [torch-rnn](https://github.com/jcjohnson/torch-rnn), the successor to char-rnn. A lot of the work in these tools was coordinated in this thread on the mtgsalvation forums: 

[Generating Magic cards using deep, recurrent neural networks](http://www.mtgsalvation.com/forums/creativity/custom-card-creation/612057-generating-magic-cards-using-deep-recurrent-neural)

Much like mtgencode the purpose of this project is to wrangle text between tools and human readable formats. The original input to encode comes from [HearthstoneJSON](https://hearthstonejson.com/) maintained by the excellent people of the [HearthSim](https://hearthsim.info/) community. This is encoded into an input format that is easier for mtg-rnn or torch-rnn to understand by Buzzbox-Encode. Once the rnn has produced output it can be returned to json data or a more human readable format through Buzzbox-Decode. Buzzbox-Stream can be used to encode and stream multiple different files into billzorns fork of torch-rnn. Inflater is a simple tool that can be used to duplicate ( or tripple, or even more ) the cards in a hearthstoneJSON file.

Requirements
=============
This is a C# project, to run .net executables on linux you will need to [Install Mono](http://www.mono-project.com/docs/getting-started/install/linux/) or [.net Core](https://www.microsoft.com/net/core#linuxubuntu_). My current setup uses Mono Complete, I suspect .net core works but have not tested it. You will need a hearthstoneJSON file of collectable cards. You can get the most recent english copy from here:

[https://api.hearthstonejson.com/v1/latest/enUS/cards.collectible.json](https://api.hearthstonejson.com/v1/latest/enUS/cards.collectible.json)

Buzzbox is used to interact with mtg-rnn and torch-rnn. As such it is best if you are somewhat familiar with Linux systems in general and Bash. You will need to install [Torch](http://torch.ch/docs/getting-started.html). To install either mtg-rnn or torch-rnn I would refer you to their respective [Installation](https://github.com/billzorn/mtg-rnn#requirements) [Instructions](https://github.com/billzorn/torch-rnn#installation).

In the case that you wish to speed up the learning through CUDA or OpenCL and are having trouble installing their various requirements you may be able to use [this](https://github.com/hughperkins/distro-cl) convenient distro maintained by hughperkins that'll help installing CUDA/OpenCL related torch packages like cutorch and cunn. 

Usage
=============

### Buzzbox-Encode.exe

```
Encodes a hearthstonejson file to a mtg-rnn usable corpus txt file.


  -i, --input         Required. Path to input file to be Encoded, must be in
                      hearthstonejson format.

  -o, --output        (Default: output.txt) Output file path.

  -e, --encoding      (Default: MtgEncoderFormat) Which encoding format to use.
                      Supported formats are scfdivineFormat and
                      MtgEncoderFormat.

  --shuffle-fields    (Default: False) Shuffles the fields of the output in
                      supported Encoding Formats.

  --shuffle-cards     (Default: False) Shuffles the the cards, randomizing the
                      order of output.

  --verbose           (Default: False) Output additional information. Exclusive
                      with the --silent option.

  --silent            (Default: False) Never output anything but error
                      messages. Exclusive with the --verbose option.

```

The supported encodings are:

Encoding         | Description
-----------------|------------
MtgEncoderFormat | Format: `|3type|4class|5race/tribe|6rarity|7manacost|8attack|9health/durablity|2text|1name|`
                 | Example: `|3minion|4paladin|5none|6legendary|7&^^^^^^^^|8&^^^^^^|9&^^^^^^|2$DV$. $T$. $DR$: equip a &^^^^^/&^^^ ashbringer.|1tirion fordring|`
scfdivineFormat  | Format: `Name @ Class | Race/Tribe | Type | Rarity | ManaCost | Attack/Health || Text &`
                 | Example: `Tirion Fordring @ Paladin |  | Minion | L | 8 | 6/6 || $DV$. $T$. $DR$: Equip a 5/3 Ashbringer. &`
                 
With --shuffle-fields the MtgEncoderFormat will randomize the order of fields for every single card.

With --shuffle-cards Buzzbox-Encode will randomize the order of the cards in the output.

### Buzzbox-Decode.exe

```
Decodes mtg-rnn output. Into a hearthstone api json file.


  -i, --input        Path to input file to be Decoded.

  -o, --output       (Default: output.json) Output file path.

  -e, --encoding     (Default: MtgEncoderFormat) Which format to decode from.
                     Supported formats are scfdivineFormat and MtgEncoderFormat.

  --set              (Default: HS-RNN) The Set ( CORE, BRM, OG, etc) this card
                     belongs too.

  --source           (Default: hs-rnn) A costum atttribute, not in the
                     hearhstoneapi json. Intended to store what generated the
                     card.

  --image            (Default: Default) What image gets put on the card.

  --simple-output    (Default: False) Instead of json data the output file will
                     be simple text details.

  --verbose          (Default: False) Output additional information. Exclusive
                     with the --silent option.

  --silent           (Default: False) Never output anything but error messages.
                     Exclusive with the --verbose option.
```

Buzzbox-Decode supports the same formats as Encode. 

The options --set, --source and --image can be used to supply additional information about the decoded cards. 

By default Buzzbox-Decode produces HearthstoneJSON .json files but with --simple-output it will give simple human readable text like this: 

```
[Common] Mage Spell: Flamecannon - 2 mana - Deal $4 damage to a random enemy minion.

[Common] Hunter Weapon: Glaivezooka - 2/2 for 2 - Battlecry: Give a random friendly minion +1 attack.

[Common] Neutral - Minion: Stoneskin Gargoyle - 1/4 for 3 - At the start of your turn, restore this minion to full health.

[Common] Neutral Beast Minion: Dire Wolf Alpha - 2/2 for 2 - Adjacent minions have +1 attack.
``` 

With --verbose Buzzbox-Decode will lists of all the cards it finds as it processes them, or prints off why it couldn't proces them like so:

```
Could not find the card type.
Dud: '|4neutral|5dragon|6legendary|7&^^^^^^^^^|8&^^^^^^^^|9&^^^^^^^^|2$B$: add &^^ random spells to your hand (from your opponent's class).|1nefarian|'
```

When it finishes decoding Buzzbox-Decode will print a breakdown of the cards it found: 

```
Found 877 cards out of a potential 891.
253 Spells, 26 Weapons and 598 Minions.
   Neutral:     363
   Hunter:      58
   Rogue:       57
   Paladin:     57
   Priest:      57
   Shaman:      57
   Druid:       57
   Mage:        57
   Warrior:     57
   Warlock:     57
```

### Buzzbox-Stream.exe

Streams cards from hearthstone json files, or streams plain txt files to posix file handles provided through command line arguments. Buzzbox-Stream is intended to be used with Billzorns torch-rnn fork streaming option. On windows it will simply write to plaintext files in the current directory. 

````
  -i, --input                  Path to input file to be streamed, must be in
                               hearthstonejson format or a simple text file.
                               Exclusive with input-directory.

  -d, --input-directory        Path to directory of files to be streamed, must
                               be in hearthstonejson format or a simple text
                               file. Exclusive with file input.

  --loop-data                  (Default: False) Keep streaming data untill the
                               stream is closed by torch-rnn.

  --shuffle                    (Default: False) Shuffles the fields of the
                               output if the input is json data. Exclusive with
                               the --alternate-shuffling option.

  --alternate-shuffling        (Default: False) Streams two copies of all
                               input, if they are json data only one of the
                               copies will be shuffled. Exclusive with the
                               --shuffle option.

  --flavor-text                (Default: False) Include flavortext field.

  --name-replacement           Replace some cardnames with different ones from
                               a provided NameCollection.json file.

  --name-replacement-chance    (Default: 50) Chance name replacement will
                               occur, out of a hundred.

  --verbose                    (Default: False) Output additional information.
                               Exclusive with the --silent option.

  --silent                     (Default: False) Never output anything but error
                               messages. Exclusive with the --verbose option.

  --help                       Display this help screen.
  ````

The --name-replacement option was intended to improve the amount of names the RNN would learn but lack of stable patterns mostly seemed to confuse it a lot. I had better results just making a larger hearthstonejson file with duplicates of cards that had different names.

At the moment the various streams crash with an exception when torch-rnn closes them. I have not yet figured out how to elegantly close these. 

### Examples

To generate the standard encoding for mtg-rnn to learn from, run:

`./Buzzbox-Encode.exe -i "cards.collectible.json" -o "encoded.cards.txt"`

Of course, this requires that you've downloaded the hearthstoneJSON file to the same directory as Buzzbox-Encode.exe

If you wanted to convert that output back to json data you'd run:

`./Buzzbox-Decode.exe -i "encoded.cards.txt" -o "decoded.cards.json"`

If instead you wanted to convert encoded.cards.txt to human readable text you'd run: 

`./Buzzbox-Decode.exe -i "encoded.cards.txt" -o "decoded.cards.json" --simple-output`

Tutorial
=============
I'll refer you to the excellent tutorial written in [mtgencode](https://github.com/billzorn/mtgencode#tutorial) or the more technical documentation of [mtg-rnn](https://github.com/billzorn/mtg-rnn#usage) itself.

Details Of The Format 
=============

### scfdivine style

Quoting scfdivine from [this](https://www.reddit.com/r/hearthstone/comments/3cyi15/hearthstone_cards_as_created_by_a_neural_network/ct0j1mt) reddit post
```
The original formatting I used looks like this. I took some advice from the linked thread and tried to keep stats as close together as possible.

Core Rager @ Hunter | Beast | Minion | R | 4 | 4/4 || [b]Battlecry:[/b] If your hand is empty, gain +3/+3. &

I don't know if using special symbols like @ to end the name and & at the end of the line really helped, but it can't hurt. At the end, I tried also replacing keywords with shorter strings like this:

Tirion Fordring @ Paladin | | Minion | L | 8 | 6/6 || $V$. $T$. $A$ Equip a 5/3 Ashbringer. &

To the network, it shouldn't matter whether it says "Divine Shield" or "$V$", but shorter strings may be easier to recognize. I also noticed that there is some inconsistency in how the Blizzard cards are written. For example, some cards have [b]Battlecry[/b]: and others have [b]Battlecry:[/b], or even [b]Battlecry: [/b]. This method let me standardize everything at the same time.
```

### Mtgencode style

Fields are surrounded and seperated by | and identified with a number :
* 1: Name
* 2: Text
* 3: Type ( Minion/Spell/Weapon )
* 4: Class or Neutral
* 5: Race/Tribe or None
* 6: Rarity
* 7: Mana Cost
* 8: Attack
* 9: Health/Durability
* 10: Flavortext

All decimal numbers are in represented in unary, numbers over 20 are not special cased. All text is lowercased except for keyword tokens, those are all uppercase. 

The flavortext field is optional. 

### Common elements

Individual cards are separated by two newlines because that is the seperator mtg-rnn expects. Markup like `[x]`, `<b></b>` and `<i></i>` is removed, at best it does nothing at worst it confuses the network. Newlines are replaced by spaces. Hearthstone cards are simple enough they don't need newlines to clarify how to read them, however blizzard likes to insert newlines manually at times to make the text flowe nicer. Keywords like Battlecry and Deathrattle are replaced by token strings like $DR$, this makes the network better at using them and makes it easier to reinsert markup like `<b></b>`. 
