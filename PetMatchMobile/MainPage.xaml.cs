using Microsoft.Maui.ApplicationModel.Communication;
using PetMatchMobile.Data;
using PetMatchMobile.Models;

namespace PetMatchMobile
{
    public partial class MainPage : ContentPage
    {
        private List<Animal> _animals = new();
        private int _index = 0;
        private RestService _service = new RestService();

        private string userEmail = Preferences.Get("UserEmail", "utilizator_nelogat@test.com");

        private Animal _currentAnimal;
        public Animal CurrentAnimal
        {
            get => _currentAnimal;
            set { _currentAnimal = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsAdoptable)); }
        }

        public bool IsAdoptable => CurrentAnimal != null && !CurrentAnimal.IsAdopted;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadData();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CheckForUpdates();

            Dispatcher.StartTimer(TimeSpan.FromSeconds(5), () =>
            {
                CheckForUpdates();
                return true; 
            });
        }

        private async void CheckForUpdates()
        {
            if (userEmail.Contains("@") && !userEmail.Contains("utilizator_nelogat"))
            {
                var notif = await _service.CheckStatusAsync(userEmail);

                if (notif != null && notif.HasUpdate)
                {
                    int lastSeenId = Preferences.Get("LastNotifiedRequestId", 0);

                    if (notif.RequestId != lastSeenId)
                    {
                        await DisplayAlert("Vești Bune! 🎉", notif.Message, "Super!");

                        Preferences.Set("LastNotifiedRequestId", notif.RequestId);
                    }
                }
            }
        }

        async void LoadData()
        {
            _animals = await _service.GetAnimalsAsync();
            if (_animals.Count > 0) CurrentAnimal = _animals[0];
            else await DisplayAlert("Ups", "Nu există animale momentan.", "OK");
        }

        void OnNopeClicked(object sender, EventArgs e) => ShowNext();

        void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Remove("UserEmail");
            Application.Current.MainPage = new LoginPage();
        }

        async void OnLikeClicked(object sender, EventArgs e)
        {
            if (!IsAdoptable || CurrentAnimal == null) return;

            bool success = await _service.SendAdoptionRequestAsync(CurrentAnimal.ID, userEmail);

            if (success)
            {
                await DisplayAlert("Felicitări! 🐾", "Cererea ta a fost trimisă către admin. Vei primi o notificare când se aprobă.", "OK");
                ShowNext();
            }
            else
            {
                await DisplayAlert("Eroare", "Nu s-a putut trimite cererea. Verifică conexiunea.", "OK");
            }
        }

        async void OnInfoClicked(object sender, EventArgs e)
        {
            if (CurrentAnimal != null)
            {
                await DisplayAlert($"Despre {CurrentAnimal.Name}",
                    $"Descriere: {CurrentAnimal.Description}\n\nAdăpost: {CurrentAnimal.ShelterName}", "Închide");
            }
        }

        void ShowNext()
        {
            _index++;

            if (_index < _animals.Count)
            {
                CurrentAnimal = _animals[_index];
            }
            else
                _index = 0;

                if (_animals.Count > 0)
                {
                    CurrentAnimal = _animals[0];
                    DisplayAlert("Gata!", "Ai văzut toate animalele. Lista o va lua de la capăt!", "OK");
                }
            }
        }
    }
}