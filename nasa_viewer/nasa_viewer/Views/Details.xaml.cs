using Android.Content;
using Android.Provider;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using Google;
using Plugin.Permissions;
using nasa_viewer.Models;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using static Android.Manifest;

namespace sdv.Views;

public partial class Details : ContentPage
{
    IFileSaver fileSaver;
    string name;
    string url;
    private CancellationToken cancellationToken;

    public Details(NasaAPODRoot nasaAPODRoot)
    {
        InitializeComponent();
        BindingContext = nasaAPODRoot;

        Title.Text = nasaAPODRoot.Title;
        name = nasaAPODRoot.Title;

        Date.Text = nasaAPODRoot.Date;

        Image.Source = nasaAPODRoot.Hdurl;
        url = nasaAPODRoot.Hdurl;

        Descr.Text = nasaAPODRoot.Explanation;
        Cr.Text = nasaAPODRoot.Copyright;
    }

    private async void DoubleTapAsync(object sender, TappedEventArgs e)
    {

        string sanitizedFileName = SanitizeFileName(name);

        string folderPath = Path.Combine(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath, "NasaViewer");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string imagePath = Path.Combine(folderPath, sanitizedFileName);

        if (!File.Exists(imagePath))
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    byte[] imageBytes = await client.GetByteArrayAsync(new Uri(url));
                    File.WriteAllBytes(imagePath, imageBytes);
                }
                await Toast.Make($"File Scaricato!").Show(cancellationToken);

            }
            catch (Exception)
            {
                await Toast.Make($"File non scaricato!").Show(cancellationToken);
                throw;
            }
        }

        else
        {
            await DisplayAlert("File già scaricato", "", "Ok");
        }
    }


    public static string CleanFileName(string fileName)
    {

        string invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()));
        string pattern = string.Format("[{0}]", Regex.Escape(new string(Path.GetInvalidFileNameChars())));


        string cleanedFileName = Regex.Replace(fileName, pattern, "_", RegexOptions.Compiled);

        return cleanedFileName;
    }

    string SanitizeFileName(string fileName)
    {

        foreach (char c in Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(c, '_');
        }

        fileName = Regex.Replace(fileName, @"[/:?\\*|""<>\.]", "_");

        return $"{fileName}.jpg";
    }
}