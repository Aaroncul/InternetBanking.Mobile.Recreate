using InternetBanking.Models;
using InternetBanking.ViewModels.Base;
using InternetBanking.ViewModels.Parameters;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace InternetBanking.ViewModels
{
    public class FindBranchViewModel : SkinnedViewModel
    {
        private Map _map;

        public Map Map
        {
            get => _map;
            set
            {
                _map = value;
                RaisePropertyChanged(() => Map);
            }
        }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                RaisePropertyChanged(() => SearchText);

                HandleSearchTextChanged();
            }
        }

        public ICommand CenterCommand => new Command(CenterMapOnPins);

        public ICommand ListViewCommand => new Command(async () =>
            await NavigationService.NavigateToAsync(null));

        private Branch SelectedBranch { get; set; }

        public override Task InitializeAsync(object navigationData)
        {
            Title = "Find a Branch";

            SelectedBranch = ((FindBranchViewModelParameter)navigationData).SelectedBranch;

            CenterMapOnPins();

            return Task.FromResult(true);
        }

        private void CenterMapOnPins()
        {
            var center = new Position(SelectedBranch.Latitude,
                    SelectedBranch.Longitude);

            var span = CalculateSpan();

            Map = new Map(MapSpan.FromCenterAndRadius(center, span))
            {
                IsShowingUser = false,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand
            };

            foreach (var branch in App.Skin.Branches)
            {
                var pin = new Pin
                {
                    Type = PinType.Place,
                    Position = new Position(branch.Latitude, branch.Longitude),
                    Label = branch.Line1,
                    Address = branch.FormattedAddress()

                };

                pin.InfoWindowClicked += async (s, args) =>
                {
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        await Xamarin.Essentials.Launcher.OpenAsync(branch.iOSMapsLink);
                    }
                };

                Map.Pins.Add(pin);
            }
        }

        private void HandleSearchTextChanged()
        {
            if (string.IsNullOrEmpty(SearchText))
            {
                return;
            }

            var branch = App.Skin.Branches.FirstOrDefault(x =>
                x.Like(SearchText));

            if (branch != null)
            {
                var position = new Position(branch.Latitude, branch.Longitude);

                Map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(0.5)));
            }
        }

        private Distance CalculateSpan()
        {
            var orderedLatitude = App.Skin.Branches.OrderByDescending(x => x.Latitude);
            var orderedLongitude = App.Skin.Branches.OrderByDescending(x => x.Longitude);

            var maxLatitude = orderedLatitude.First();
            var minLatitude = orderedLatitude.Last();

            var maxLongitude = orderedLongitude.First();
            var minLongitude = orderedLongitude.Last();

            var distanceBetweenLatitudes = DistanceBetween(
                maxLatitude.Latitude, maxLatitude.Longitude,
                minLatitude.Latitude, minLatitude.Longitude);

            var distanceBetweenLongitudes = DistanceBetween(
                maxLongitude.Latitude, maxLongitude.Longitude,
                minLongitude.Latitude, minLongitude.Longitude);

            return Distance.FromKilometers(Math.Max(distanceBetweenLatitudes, distanceBetweenLongitudes));
        }

        private double DistanceBetween(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            var theta = longitude1 - longitude2;

            var distance =
                Math.Sin(DegreesToRadians(latitude1)) *
                Math.Sin(DegreesToRadians(latitude2)) +
                Math.Cos(DegreesToRadians(latitude1)) *
                Math.Cos(DegreesToRadians(latitude2)) *
                Math.Cos(DegreesToRadians(theta));

            distance = Math.Acos(distance);
            distance = RadiansToDegrees(distance);

            return distance * 60 * 1.609344;
        }

        private double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private double RadiansToDegrees(double radians)
        {
            return radians / Math.PI * 180.0;
        }
    }
}