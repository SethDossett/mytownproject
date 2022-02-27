INCLUDE globals.ink

{ pokemon_name == "": -> main | -> already_chose }

=== main ===
Which pokemon do you choose?  #speaker:Boy Ty #emote:Idle
    + [Charmander]
        -> chosen("charmander")
    + [Bulbasaur]
        -> chosen("bulbasaur")
    + [Squirtle]
        -> chosen("squirtle")    
        
=== chosen(pokemon) ===
~ pokemon_name = pokemon
You chose {pokemon}! #emote:Happy
->END

=== already_chose ===
You already chose {pokemon_name}! #speaker:Boy Ty
-> END
