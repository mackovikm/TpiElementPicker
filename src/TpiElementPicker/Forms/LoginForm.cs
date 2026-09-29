using TpiElementPicker.Services;

namespace TpiElementPicker.Forms;

/// <summary>Správa profilů připojení (URL + způsob přihlášení).</summary>
public sealed class LoginForm : Form
{
    private readonly ProfileStore _store;

    private readonly ListBox _list = new();
    private readonly TextBox _name = new();
    private readonly TextBox _url = new();
    private readonly ComboBox _mode = new();
    private readonly ComboBox _environment = new();
    private readonly TextBox _user = new();
    private readonly TextBox _password = new() { UseSystemPasswordChar = true };
    private readonly TextBox _userSelector = new();
    private readonly TextBox _passwordSelector = new();
    private readonly TextBox _submitSelector = new();
    private readonly CheckBox _navigate = new() { Text = "Po uložení rovnou načíst URL", Checked = true };

    private ConnectionProfile? _current;

    public ConnectionProfile? SelectedProfile { get; private set; }
    public bool NavigateAfterSave => _navigate.Checked;

    public LoginForm(ProfileStore store, ConnectionProfile? preselect)
    {
        _store = store;

        Text = "Profily připojení";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ClientSize = new Size(760, 452);

        _list.SetBounds(12, 12, 220, 362);
        _list.SelectedIndexChanged += (_, _) => LoadProfile(_list.SelectedItem as ConnectionProfile);

        var btnNew = new Button { Text = "Nový", Bounds = new Rectangle(12, 380, 105, 28) };
        btnNew.Click += (_, _) => CreateProfile();

        var btnDelete = new Button { Text = "Smazat", Bounds = new Rectangle(127, 380, 105, 28) };
        btnDelete.Click += (_, _) => DeleteProfile();

        var x = 250;
        var labelWidth = 150;
        var fieldX = x + labelWidth;
        var fieldWidth = 480 - labelWidth + 20;
        var y = 14;

        Controls.Add(MakeLabel("Název:", x, y));
        _name.SetBounds(fieldX, y - 3, fieldWidth, 23);
        y += 32;

        Controls.Add(MakeLabel("URL:", x, y));
        _url.SetBounds(fieldX, y - 3, fieldWidth, 23);
        y += 32;

        Controls.Add(MakeLabel("Způsob přihlášení:", x, y));
        _mode.SetBounds(fieldX, y - 3, fieldWidth, 23);
        _mode.DropDownStyle = ComboBoxStyle.DropDownList;
        _mode.Items.AddRange(new object[]
        {
            "Ruční (session drží WebView2)",
            "HTTP Basic",
            "Vyplnit přihlašovací formulář"
        });
        _mode.SelectedIndexChanged += (_, _) => UpdateSelectorState();
        y += 32;

        Controls.Add(MakeLabel("Prostředí:", x, y));
        _environment.SetBounds(fieldX, y - 3, fieldWidth, 23);
        _environment.DropDownStyle = ComboBoxStyle.DropDownList;
        _environment.Items.AddRange(new object[]
        {
            "Neurčeno",
            "Dev (hnědý pruh)",
            "Preprod (růžový pruh)",
            "Produkce (modrý pruh)"
        });
        y += 32;

        Controls.Add(MakeLabel("Uživatel:", x, y));
        _user.SetBounds(fieldX, y - 3, fieldWidth, 23);
        y += 32;

        Controls.Add(MakeLabel("Heslo:", x, y));
        _password.SetBounds(fieldX, y - 3, fieldWidth, 23);
        y += 40;

        Controls.Add(MakeLabel("CSS selektor – uživatel:", x, y));
        _userSelector.SetBounds(fieldX, y - 3, fieldWidth, 23);
        y += 32;

        Controls.Add(MakeLabel("CSS selektor – heslo:", x, y));
        _passwordSelector.SetBounds(fieldX, y - 3, fieldWidth, 23);
        y += 32;

        Controls.Add(MakeLabel("CSS selektor – tlačítko:", x, y));
        _submitSelector.SetBounds(fieldX, y - 3, fieldWidth, 23);
        y += 36;

        _navigate.SetBounds(fieldX, y, 300, 22);

        var btnOk = new Button
        {
            Text = "Uložit a zavřít",
            DialogResult = DialogResult.OK,
            Bounds = new Rectangle(ClientSize.Width - 250, 380, 115, 28)
        };
        btnOk.Click += (_, _) => SaveCurrent();

        var btnCancel = new Button
        {
            Text = "Zrušit",
            DialogResult = DialogResult.Cancel,
            Bounds = new Rectangle(ClientSize.Width - 127, 380, 115, 28)
        };

        Controls.AddRange(new Control[]
        {
            _list, btnNew, btnDelete, _name, _url, _mode, _environment, _user, _password,
            _userSelector, _passwordSelector, _submitSelector, _navigate, btnOk, btnCancel
        });

        AcceptButton = btnOk;
        CancelButton = btnCancel;

        RefreshList();

        if (preselect is not null && _store.Profiles.Contains(preselect))
            _list.SelectedItem = preselect;
        else if (_list.Items.Count > 0)
            _list.SelectedIndex = 0;
        else
            CreateProfile();
    }

    private static Label MakeLabel(string text, int x, int y)
        => new Label { Text = text, AutoSize = true, Location = new Point(x, y) };

    private void RefreshList()
    {
        _list.Items.Clear();
        foreach (var profile in _store.Profiles)
            _list.Items.Add(profile);
    }

    private void CreateProfile()
    {
        SaveCurrent();

        var profile = new ConnectionProfile { Name = "Nový profil" };
        _store.Profiles.Add(profile);
        RefreshList();
        _list.SelectedItem = profile;
    }

    private void DeleteProfile()
    {
        if (_current is null) return;
        _store.Profiles.Remove(_current);
        _current = null;
        RefreshList();
        if (_list.Items.Count > 0) _list.SelectedIndex = 0;
    }

    private void LoadProfile(ConnectionProfile? profile)
    {
        SaveCurrent();
        _current = profile;
        SelectedProfile = profile;

        if (profile is null) return;

        _name.Text = profile.Name;
        _url.Text = profile.Url;
        _mode.SelectedIndex = (int)profile.Mode;
        _environment.SelectedIndex = (int)profile.Environment;
        _user.Text = profile.UserName;
        _password.Text = profile.Password;
        _userSelector.Text = profile.UserSelector;
        _passwordSelector.Text = profile.PasswordSelector;
        _submitSelector.Text = profile.SubmitSelector;

        UpdateSelectorState();
    }

    private void UpdateSelectorState()
    {
        var isForm = _mode.SelectedIndex == (int)LoginMode.FormAutoFill;
        _userSelector.Enabled = isForm;
        _passwordSelector.Enabled = isForm;
        _submitSelector.Enabled = isForm;

        var needsCredentials = _mode.SelectedIndex != (int)LoginMode.Manual;
        _user.Enabled = needsCredentials;
        _password.Enabled = needsCredentials;
    }

    private void SaveCurrent()
    {
        if (_current is null) return;

        _current.Name = _name.Text.Trim();
        _current.Url = _url.Text.Trim();
        _current.Mode = (LoginMode)Math.Max(0, _mode.SelectedIndex);
        _current.Environment = (TpiEnvironment)Math.Max(0, _environment.SelectedIndex);
        _current.UserName = _user.Text.Trim();
        _current.Password = _password.Text;
        _current.UserSelector = _userSelector.Text.Trim();
        _current.PasswordSelector = _passwordSelector.Text.Trim();
        _current.SubmitSelector = _submitSelector.Text.Trim();

        SelectedProfile = _current;
    }
}
