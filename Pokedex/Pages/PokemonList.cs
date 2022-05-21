using Microsoft.AspNetCore.Components;
using PokeApiNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pokedex.Pages
{
    public partial class PokemonList
    {
        // this injects pokeclient from dependency injection container
        [Inject]
        PokeApiClient? PokeClient { get; set; }

        private List<Pokemon> Pokemon = new List<Pokemon>();

        const int ItemsPerPage = 12;

        protected override async Task OnInitializedAsync()
        {
            await LoadPage();
        }

        // this method helps retrieve a pokemon from the pokemon list
        private Pokemon? GetPokemon(int index)
        {
            if (Pokemon.Count > index)
                return Pokemon[index];
            else
                return null;
        }

        private async Task LoadPage()
        {
            // gets list of pokemon resources
            var pageResponse =
                await PokeClient.GetNamedResourcePageAsync<PokeApiNet.Pokemon>(ItemsPerPage, 0);
            
            //creates list of tasks for calling and get details of the pokemon
            var tasks = pageResponse.Results
                .Select(p => PokeClient.GetResourceAsync<PokeApiNet.Pokemon>(p.Name));

            //this awaits for all tasks and set data model for the page
            Pokemon = (await Task.WhenAll(tasks)).ToList();
        }
    }
}