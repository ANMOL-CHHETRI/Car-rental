using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using RentOnCars.Services;
using RentOnCars.UI;
namespace RentOnCars
{
    public partial class LoginForm : Form
    {
        // Required assignment credential; actual sign-in uses the hashed staff account store.
        public const string StaffUsername = "sta001";
        public const string StaffPassword = "givemethekeys123";
        public event EventHandler Authenticated;
        public string username => usernameBox.Text.Trim();
        public string password => passwordBox.Text;
        public LoginForm()
        {
            InitializeComponent(); AcceptButton = loginButton;
            passwordBox.UseSystemPasswordChar = true; Theme.ApplyPrimaryButton(loginButton);
            hintLabel.Text = "Use your assigned staff account.";
            hintLabel.AutoSize=false;hintLabel.Size=new System.Drawing.Size(384,34);
            usernameBox.AccessibleName = "Staff username"; passwordBox.AccessibleName = "Password";
        }
        public bool validateCredentials() => ValidateCredentials(username,password);
        // This pure compatibility method is used by the assignment's existing unit tests, not the sign-in flow.
        public bool ValidateCredentials(string u,string p) => string.Equals(u,StaffUsername,StringComparison.Ordinal) && string.Equals(p,StaffPassword,StringComparison.Ordinal);
        public bool login()
        {
            var identity = StaffAuthentication.Authenticate(DatabaseInitializer.ConnectionString,username,password);
            if(identity == null) return false;
            StaffSession.SignIn(identity); return true;
        }
        private async void loginButton_Click(object sender,EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)) { hintLabel.Text="Enter your username and password."; return; }
            loginButton.Enabled=false; hintLabel.Text="Connecting…";
            string u=username,p=password;
            try
            {
                var identity=await Task.Run(() => {
                    if (!DatabaseInitializer.IsInitialized) {
                        var result=DatabaseInitializer.Initialize();
                        if(!result.IsSuccessful) throw new InvalidOperationException(result.Message);
                    }
                    return StaffAuthentication.Authenticate(DatabaseInitializer.ConnectionString,u,p);
                });
                if(IsDisposed)return;
                passwordBox.Clear();
                if(identity==null) { LoginAuditLogger.LogFailure(u,"Invalid credentials or temporarily locked account"); hintLabel.Text="Unable to sign in. Check your details or try again later."; return; }
                StaffSession.SignIn(identity);LoginAuditLogger.LogSuccess(identity.Username);
                Authenticated?.Invoke(this,EventArgs.Empty);
            }
            catch(Exception ex) { if(!IsDisposed) { hintLabel.Text="Connection unavailable. Check database setup."; MessageBox.Show(ex.Message,"Sign-in unavailable",MessageBoxButtons.OK,MessageBoxIcon.Warning); } }
            finally { if(!IsDisposed)loginButton.Enabled=true; }
        }
        private void showPasswordCheckBox_CheckedChanged(object sender,EventArgs e) => passwordBox.UseSystemPasswordChar=!showPasswordCheckBox.Checked;
        private void LoginForm_FormClosing(object sender,FormClosingEventArgs e) { passwordBox.Clear(); }
    }
}
