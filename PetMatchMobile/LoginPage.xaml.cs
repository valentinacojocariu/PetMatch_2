using PetMatchMobile.Data;

namespace PetMatchMobile
{
    public partial class LoginPage : ContentPage
    {
        private RestService _service = new RestService();

        public LoginPage()
        {
            InitializeComponent();
        }

        async void OnLoginClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(EmailEntry.Text) || string.IsNullOrEmpty(PassEntry.Text))
            {
                await DisplayAlert("Eroare", "Scrie email și parola", "OK");
                return;
            }

            // Acum, după modificarea din Program.cs, asta va merge!
            bool success = await _service.LoginAsync(EmailEntry.Text, PassEntry.Text);

            if (success)
            {
                // Intră în aplicație
                Application.Current.MainPage = new NavigationPage(new MainPage());
            }
            else
            {
                await DisplayAlert("Ups", "Email sau parolă greșită.", "Încearcă iar");
            }
        }

        async void OnRegisterClicked(object sender, EventArgs e)
        {
            // Facem cont direct din aplicatie (NATIV)
            bool success = await _service.RegisterAsync(EmailEntry.Text, PassEntry.Text);
            if (success)
            {
                await DisplayAlert("Succes", "Cont creat! Acum te poți loga.", "OK");
            }
            else
            {
                await DisplayAlert("Eroare", "Nu am putut crea contul. Încearcă alt email.", "OK");
            }
        }
    }
}