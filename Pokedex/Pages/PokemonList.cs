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
            var pageResponse = (await PokeClient.GetNamedResourcePageAsync<PokeApiNet.Pokemon>(
                ItemsPerPage, (CurrentPage - 1) * ItemsPerPage));
                TotalPokemon = pageResponse.Count;
            
            //creates list of tasks for calling and get details of the pokemon
            var tasks = pageResponse.Results
                .Select(p => PokeClient.GetResourceAsync<PokeApiNet.Pokemon>(p.Name));

            //this awaits for all tasks and set data model for the page
            Pokemon = (await Task.WhenAll(tasks)).ToList();
        }

        //first value that is overwritten after first API call
        int TotalPokemon = 932;
        
        // determines whether loading spinner for next page is shown or not
        bool LoadingNextPage = false;

        //determines whether loading spinner for previous page is shown
        bool LoadingPreviousPage = false;
        int CurrentPage = 1;

        // this evaluates whether if the user is on the last page of pokemon entries
        bool OnLastPage() => (CurrentPage * ItemsPerPage) >= TotalPokemon;


        // these two methods below are helper methods that are
        // triggered from previous and next page buttons on UI
        private async Task LoadNextPage()
        {
            LoadingNextPage = true;
            CurrentPage++;
            await LoadPage();
            LoadingNextPage = false;
        }

        private async Task LoadPreviousPage()
        {
            LoadingPreviousPage = true;
            CurrentPage--;
            await LoadPage();
            LoadingPreviousPage = false;

        }
    }
}