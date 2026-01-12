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

        // AICI AM ADĂUGAT EMAIL-UL (ca să nu mai dea eroare că lipsește)
        // Într-o aplicație finală, acesta ar veni din pagina de Login.
        private string userEmail = "test@yahoo.com";

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

        // --- AICI ESTE MODIFICAREA PRINCIPALĂ ---
        async void OnLikeClicked(object sender, EventArgs e)
        {
            // 1. Verificări de siguranță
            if (!IsAdoptable || CurrentAnimal == null) return;

            // 2. Trimite cererea catre admin
            // Folosim CurrentAnimal.ID (mare) și userEmail definit sus
            bool success = await _service.SendAdoptionRequestAsync(CurrentAnimal.ID, userEmail);

            // 3. Verificăm rezultatul
            if (success)
            {
                await DisplayAlert("Felicitări! 🐾", "Cererea ta a fost trimisă către admin. Vei primi o notificare când se aprobă.", "OK");
                ShowNext(); // Trecem la următorul doar dacă a mers
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
            {
                CurrentAnimal = null;
                DisplayAlert("Gata!", "Ai văzut toate animalele.", "OK");
            }
        }
    }
}