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

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;

            bool success = await _service.LoginAsync(email, password);

            if (success)
            {
                Preferences.Set("UserEmail", email);

                Application.Current.MainPage = new MainPage();
            }
            else
            {
                await DisplayAlert("Eroare", "Email sau parolă greșită!", "OK");
            }
        }
        private void OnGuestClicked(object sender, EventArgs e)
        {
            Preferences.Set("UserEmail", "guest@test.com");
            Application.Current.MainPage = new MainPage();
        }

        async void OnRegisterClicked(object sender, EventArgs e)
        {
            bool success = await _service.RegisterAsync(EmailEntry.Text, PasswordEntry.Text);
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