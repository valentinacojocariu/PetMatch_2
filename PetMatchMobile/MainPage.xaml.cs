using PetMatchMobile.Data;
using PetMatchMobile.Models;

namespace PetMatchMobile
{
    public partial class MainPage : ContentPage
    {
        private List<Animal> _animals = new();
        private int _index = 0;
        private RestService _service = new RestService();

        // Proprietate pentru Binding
        private Animal _currentAnimal;
        public Animal CurrentAnimal
        {
            get => _currentAnimal;
            set { _currentAnimal = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsAdoptable)); }
        }

        // Proprietate calculata: Putem da like doar daca NU e adoptat
        public bool IsAdoptable => CurrentAnimal != null && !CurrentAnimal.IsAdopted;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadData();
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
            Application.Current.MainPage = new LoginPage();
        }
        async void OnLikeClicked(object sender, EventArgs e)
        {
            if (!IsAdoptable) return;

            // Trimite cererea catre admin
            await _service.SendAdoptionRequest(CurrentAnimal.ID, "user@test.com"); // Aici ai pune emailul real al userului logat

            await DisplayAlert("Felicitări! 🐾", "Cererea ta a fost trimisă către admin. Vei primi o notificare când se aprobă.", "OK");
            ShowNext();
        }

        async void OnInfoClicked(object sender, EventArgs e)
        {
            await DisplayAlert($"Despre {CurrentAnimal.Name}",
                $"Descriere: {CurrentAnimal.Description}\n\nAdăpost: {CurrentAnimal.ShelterName}", "Închide");
        }

        void ShowNext()
        {
            _index++;
            if (_index < _animals.Count) CurrentAnimal = _animals[_index];
            else
            {
                CurrentAnimal = null;
                DisplayAlert("Gata!", "Ai văzut toate animalele.", "OK");
            }
        }
    }
}