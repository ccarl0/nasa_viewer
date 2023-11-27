using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using nasa_viewer.Models;
using nasa_viewer.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace nasa_viewer.ViewModels
{
    public partial class PostViewModel : ObservableObject
    {
        [ObservableProperty]
        List<NasaAPODRoot> nasaAPODs;

        [ObservableProperty]
        public bool isRefreshing;

        public PostViewModel()
        {
            UpdateFeedAsync();
        }

        [RelayCommand]
        async Task UpdateFeedAsync()
        {
            var client = new HttpClient();

            try
            {
                // Get the current date
                DateTime currentDate = DateTime.Now;

                List<NasaAPODRoot> nasaAPODSBuffer = new();

                for (int i = 1; i < 31; i++)
                {
                    // Calculate the fetch date untill one month back
                    DateTime fetchDate = currentDate.AddDays(-i);

                    // Format date
                    string fetchDateString = fetchDate.ToString("yyyy-MM-dd");

                    // Send GET request to the API
                    var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.nasa.gov/planetary/apod?date={fetchDateString}&api_key=jBR6BFhvvViOOlasSXpap8dKbpYs2nyWNY4Otum9");

                    // Read response
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var res = await response.Content.ReadAsStringAsync();


                    nasaAPODSBuffer.Add(JsonSerializer.Deserialize<NasaAPODRoot>(res));
                    NasaAPODs = nasaAPODSBuffer;
                }
            }
            catch (Exception e)
            {
                await App.Current.MainPage.DisplayAlert("Error - NASA - APOD", e.Message, "Ok");
            }
            finally
            {
                client.Dispose();
                IsRefreshing = false;
            }
        }


        [RelayCommand]
        async Task PushDetailPage(NasaAPODRoot APODRoot)
        {
            await App.Current.MainPage.Navigation.PushAsync(new Details(APODRoot));
        }
    }
}
