﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Picard
{
    public partial class WelcomeLoginForm : Form
    {
        private InaraApi api;

        private bool autoLogin;

        public string user
        {
            get
            {
                return usernameBox.Text;
            }
            private set { }
        }

        public string pass
        {
            get
            {
                return passwordBox.Text;
            }
            private set { }
        }

        private WelcomeLoginForm()
        {
        }

        public WelcomeLoginForm(InaraApi api)
        {
            this.api = api;
            autoLogin = false;
            InitializeComponent();
        }

        public void loginWithCredentials(string user, string pass)
        {
            usernameBox.Text = user;
            passwordBox.Text = pass;
            autoLogin = true;
        }

        private async void doLogin()
        {
            LoginFormPanel.Hide();
            LoggingInPanel.Show();
            StatusLabel.Text = "Logging Into Inara.cz...";

            await api.Authenticate(usernameBox.Text, passwordBox.Text);

            StatusLabel.Text = "Waiting...";
            LoggingInPanel.Hide();
            LoginFormPanel.Show();

            if (api.isAuthenticated)
            {
                Close();
            }
            else
            {
                errorLabel.Text = api.lastError;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            doLogin();
        }

        private void WelcomeLoginForm_Load(object sender, EventArgs e)
        {
            if (autoLogin)
            {
                doLogin();
            }
        }
    }
}