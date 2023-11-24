using CommunityToolkit.Mvvm.ComponentModel;
using nasa_viewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nasa_viewer.ViewModels
{
    internal partial class PostViewModel : ObservableObject
    {
        [ObservableProperty]
        List<NasaAPODRoot> nasaAPODs;
    }
}
